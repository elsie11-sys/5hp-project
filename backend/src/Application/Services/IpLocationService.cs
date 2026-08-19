using System.Collections.Concurrent;
using System.Net;
using Application.Interfaces;
using IP2Region.Net.Abstractions;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// IP 归属地查询服务实现（基于 <c>ip2region</c> 离线库）
/// <para>
/// 处理策略：
/// 1) 空值 / 非法 IP → 返回 null（让 Controller 跳过）
/// 2) loopback（::1 / 127.0.0.1）→ "本机"
/// 3) 私网 IP（10/8、172.16/12、192.168/16、IPv6 ULA 等）→ "内网 IP"
/// 4) 公网 IP → 用 <c>ip2region</c> 离线查归属地，结果缓存 24h；
///    查不到 / 异常 → 兜底 "未知"
/// </para>
/// <para>
/// ip2region.xdb 启动时全量加载到内存（CachePolicy.Content），单次查询 ~50ns，
/// 比在线 API 快几个数量级，且不依赖网络。xdb 文件 11MB，随项目发布。
/// </para>
/// </summary>
public class IpLocationService : IIpLocationService
{
    /// <summary>key = ip, value = (location, expireAtUtc)</summary>
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    private readonly ISearcher _searcher;
    private readonly ILogger<IpLocationService>? _logger;

    public IpLocationService(ISearcher searcher, ILogger<IpLocationService>? logger = null)
    {
        _searcher = searcher;
        _logger = logger;
    }

    public async Task<string?> GetLocationAsync(string? ip, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ip)) return null;

        // 1) IPv6 loopback（::1）转 IPv4 127.0.0.1 统一处理
        var normalizedIp = NormalizeIp(ip);
        if (normalizedIp == null) return null;

        // 2) 快捷判断
        if (IPAddress.IsLoopback(IPAddress.Parse(normalizedIp)))
            return "本机";
        if (IsPrivateIp(normalizedIp))
            return "内网 IP";

        // 3) 缓存命中
        if (_cache.TryGetValue(normalizedIp, out var entry) && entry.ExpireAtUtc > DateTime.UtcNow)
        {
            return entry.Location;
        }

        // 4) 公网 IP 用 ip2region 查归属地
        //    IP2Region.Net 的 Searcher.Search 是同步的（~50ns 全内存查询），
        //    用 Task.Run 包一下避免阻塞 ASP.NET 请求线程
        string? location;
        try
        {
            var raw = await Task.Run(() => _searcher.Search(normalizedIp), cancellationToken);
            location = FormatLocation(raw);
            if (string.IsNullOrEmpty(location))
            {
                _logger?.LogDebug("[IpLocationService] {Ip} 离线库未命中", normalizedIp);
                location = "未知";
            }
        }
        catch (Exception ex)
        {
            _logger?.LogDebug(ex, "[IpLocationService] {Ip} 查询异常", normalizedIp);
            location = "未知";
        }

        // 5) 写缓存（包括"未知"也缓存，避免对未知 IP 反复打库）
        _cache[normalizedIp] = new CacheEntry(location, DateTime.UtcNow.Add(CacheTtl));
        return location;
    }

    // ============== 私有辅助 ==============

    /// <summary>
    /// 把 xdb 返回的 "中国|0|北京市|北京市|电信|CN" 格式转换成 "中国 北京市 电信"。
    /// <para>
    /// ip2region v4.x xdb 段顺序：国家|区域|省份|城市|ISP|国家码（英文 2 字母）
    /// 处理：
    ///   - "0" / 空字符串过滤
    ///   - 连续重复段去重（如"北京市 北京市"）
    ///   - 2 字母大写国家码（CN/US/JP...）一律去掉，跟中文国家名重复
    /// </para>
    /// </summary>
    private static string FormatLocation(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var parts = raw.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                       .Where(p => p != "0" && !string.IsNullOrWhiteSpace(p))
                       .ToList();
        if (parts.Count == 0) return string.Empty;

        var deduped = new List<string>();
        foreach (var p in parts)
        {
            // 跳过 2 字母大写国家码（CN / US / JP 等）
            if (p.Length == 2 && p.All(char.IsUpper)) continue;
            // 跳过和上一段完全相同（去重）
            if (deduped.Count > 0 && deduped[^1] == p) continue;
            deduped.Add(p);
        }
        return string.Join(" ", deduped);
    }

    private static string? NormalizeIp(string ip)
    {
        ip = ip.Trim();
        if (ip == "::1") return "127.0.0.1";
        return ip;
    }

    /// <summary>是否为私网 IP（10/8、172.16/12、192.168/16、IPv6 ULA fc00::/7、link-local）</summary>
    private static bool IsPrivateIp(string ip)
    {
        if (!IPAddress.TryParse(ip, out var addr)) return false;

        if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            var bytes = addr.GetAddressBytes();
            if (bytes[0] == 10) return true;
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return true;
            if (bytes[0] == 192 && bytes[1] == 168) return true;
            if (bytes[0] == 169 && bytes[1] == 254) return true; // link-local
            return false;
        }

        if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            var bytes = addr.GetAddressBytes();
            if ((bytes[0] & 0xfe) == 0xfc) return true;             // fc00::/7
            if (bytes[0] == 0xfe && (bytes[1] & 0xc0) == 0x80) return true; // fe80::/10
            return false;
        }

        return false;
    }

    private sealed class CacheEntry
    {
        public string Location { get; }
        public DateTime ExpireAtUtc { get; }
        public CacheEntry(string location, DateTime expireAtUtc)
        {
            Location = location;
            ExpireAtUtc = expireAtUtc;
        }
    }
}

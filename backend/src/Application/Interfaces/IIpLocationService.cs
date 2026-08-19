namespace Application.Interfaces;

/// <summary>
/// IP 归属地查询服务
/// <para>
/// 输入一个 IP 地址，返回它对应的"地理位置"字符串（例如"北京 北京市"、"内网 IP"、"本机"）。
/// 用于登录日志、访问统计等场景的"登录地点"字段。
/// </para>
/// </summary>
public interface IIpLocationService
{
    /// <summary>
    /// 异步查询 IP 归属地。返回 null 表示无法解析。
    /// </summary>
    /// <param name="ip">客户端 IP（IPv4 / IPv6）</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<string?> GetLocationAsync(string? ip, CancellationToken cancellationToken = default);
}

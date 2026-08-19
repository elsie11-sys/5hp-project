using Application.Interfaces;
using Application.Services;
using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// IP 归属地服务（基于 ip2region 离线库）的 DI 扩展
/// </summary>
public static class IpLocationServiceCollectionExtensions
{
    /// <summary>
    /// 注册 IP 归属地查询服务
    /// </summary>
    /// <param name="services">DI 容器</param>
    /// <param name="xdbPath">ip2region xdb 文件绝对路径（推荐放在 AppContext.BaseDirectory 下）</param>
    /// <param name="cachePolicy">
    /// xdb 缓存策略：
    ///   - <see cref="CachePolicy.Content"/>：全量加载到内存（~11MB），单次查询 ~50ns，最快；推荐生产
    ///   - <see cref="CachePolicy.VectorIndex"/>：只缓存 vector index，省内存但查询 ~4.4μs
    ///   - <see cref="CachePolicy.File"/>：完全文件 IO，~6.7μs
    /// </param>
    public static IServiceCollection AddIpLocation(
        this IServiceCollection services,
        string xdbPath,
        CachePolicy cachePolicy = CachePolicy.Content)
    {
        if (string.IsNullOrWhiteSpace(xdbPath))
            throw new ArgumentException("xdb 路径不能为空", nameof(xdbPath));

        // ISearcher 是个无状态查询器，Singleton 注册省内存
        services.AddSingleton<ISearcher>(_ => new Searcher(cachePolicy, xdbPath));
        services.AddSingleton<IIpLocationService, IpLocationService>();
        return services;
    }
}

using Application.DTOs;

namespace Application.Interfaces;

/// <summary>
/// 登录日志服务接口
/// </summary>
public interface ILoginLogService
{
    /// <summary>
    /// 分页查询登录日志（按 id 倒序，最新在前）
    /// </summary>
    Task<(List<LoginLogDto> Items, int Total)> GetPagedAsync(LoginLogQuery query);

    /// <summary>
    /// 按查询条件拉全量（用于导出 Excel）
    /// </summary>
    Task<List<LoginLogDto>> GetAllAsync(LoginLogQuery query);

    /// <summary>
    /// 写入一条登录日志（前端登录成功/失败时调用）
    /// </summary>
    Task<LoginLogDto> CreateAsync(LoginLogCreateRequest request);

    /// <summary>
    /// 批量删除
    /// </summary>
    Task<int> BatchDeleteAsync(IReadOnlyList<long> ids);

    /// <summary>
    /// 清空全部
    /// </summary>
    Task<int> ClearAsync();

    /// <summary>
    /// 账户解锁（按用户名）
    /// </summary>
    Task<string> UnlockAsync(string userName);
}

using Application.DTOs;

namespace Application.Interfaces;

/// <summary>
/// 操作日志服务接口
/// </summary>
public interface IOperationLogService
{
    /// <summary>
    /// 分页查询操作日志（按 id 倒序，最新在前）
    /// </summary>
    Task<(List<OperationLogDto> Items, int Total)> GetPagedAsync(OperationLogQuery query);

    /// <summary>
    /// 按查询条件拉全量（用于导出 Excel）
    /// </summary>
    Task<List<OperationLogDto>> GetAllAsync(OperationLogQuery query);

    /// <summary>
    /// 写入一条操作日志
    /// </summary>
    Task<OperationLogDto> CreateAsync(OperationLogCreateRequest request);

    /// <summary>
    /// 批量删除
    /// </summary>
    Task<int> BatchDeleteAsync(IReadOnlyList<long> ids);

    /// <summary>
    /// 清空全部
    /// </summary>
    Task<int> ClearAsync();
}

/// <summary>
/// 创建操作日志请求体
/// </summary>
public class OperationLogCreateRequest
{
    public string Operator { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string? Content { get; set; }
    public string? Ip { get; set; }
    /// <summary>1=成功 0=失败</summary>
    public int Status { get; set; } = 1;
    public int? CostMs { get; set; }
    public string? RequestUrl { get; set; }
    public string? RequestMethod { get; set; }
    public string? Method { get; set; }
    public string? RequestParams { get; set; }
    public string? ResponseParams { get; set; }
    public string? ErrorMsg { get; set; }
    public string? UserAgent { get; set; }
    public string? Location { get; set; }
    /// <summary>业务上下文 JSON（Controller 在过程中通过 HttpContext.Items 注入）</summary>
    public string? BizData { get; set; }
}

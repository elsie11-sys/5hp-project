using System.Text.Json.Serialization;

namespace Application.DTOs;

/// <summary>
/// 操作日志 DTO（前端展示用，与前端 OperationLogDto 字段一一对应）。
/// </summary>
public class OperationLogDto
{
    public long Id { get; set; }
    public string Operator { get; set; } = string.Empty;
    /// <summary>操作类型：login/logout/create/update/delete/export/import/query/other</summary>
    public string Type { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string? Content { get; set; }
    public string? Ip { get; set; }
    /// <summary>1=成功 0=失败</summary>
    public int Status { get; set; } = 1;
    public int? CostMs { get; set; }
    public string? RequestUrl { get; set; }
    /// <summary>GET / POST / PUT / DELETE</summary>
    public string? RequestMethod { get; set; }
    public string? Method { get; set; }
    public string? RequestParams { get; set; }
    public string? ResponseParams { get; set; }
    public string? ErrorMsg { get; set; }
    public string? UserAgent { get; set; }
    public string? Location { get; set; }
    /// <summary>业务上下文 JSON（由 Controller 按需塞，Filter 默认汇总）</summary>
    public string? BizData { get; set; }

    /// <summary>
    /// 创建时间（输出截到秒：yyyy-MM-dd HH:mm:ss）
    /// </summary>
    [JsonConverter(typeof(DateTimeTextConverter))]
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// 操作日志查询参数
/// </summary>
public class OperationLogQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Operator { get; set; }
    public string? Type { get; set; }
    public string? Module { get; set; }
    public string? Ip { get; set; }
    /// <summary>1=成功 0=失败</summary>
    public int? Status { get; set; }
    /// <summary>开始日期 YYYY-MM-DD（包含）</summary>
    public string? StartDate { get; set; }
    /// <summary>结束日期 YYYY-MM-DD（包含）</summary>
    public string? EndDate { get; set; }
}

/// <summary>
/// 批量删除请求
/// </summary>
public class OperationLogBatchDeleteRequest
{
    public List<long> Ids { get; set; } = new();
}

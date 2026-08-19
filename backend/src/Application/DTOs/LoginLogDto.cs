using System.Text.Json.Serialization;

namespace Application.DTOs;

/// <summary>
/// 登录日志 DTO（前端展示用，与前端 LoginLogDto 字段一一对应）。
/// </summary>
public class LoginLogDto
{
    public long Id { get; set; }

    /// <summary>用户名</summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>登录地址（IP）</summary>
    public string? Ip { get; set; }

    /// <summary>登录地点</summary>
    public string? Location { get; set; }

    /// <summary>操作系统</summary>
    public string? Os { get; set; }

    /// <summary>浏览器</summary>
    public string? Browser { get; set; }

    /// <summary>1=成功 0=失败</summary>
    public int Status { get; set; } = 1;

    /// <summary>描述（登录成功 / 失败原因）</summary>
    public string? Message { get; set; }

    /// <summary>登录时间（输出截到秒：yyyy-MM-dd HH:mm:ss）</summary>
    [JsonConverter(typeof(DateTimeTextConverter))]
    public DateTime? LoginTime { get; set; }
}

/// <summary>
/// 登录日志查询参数
/// </summary>
public class LoginLogQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Ip { get; set; }
    public string? UserName { get; set; }
    /// <summary>1=成功 0=失败</summary>
    public int? Status { get; set; }
    /// <summary>开始日期 YYYY-MM-DD（包含）</summary>
    public string? StartDate { get; set; }
    /// <summary>结束日期 YYYY-MM-DD（包含）</summary>
    public string? EndDate { get; set; }
}

/// <summary>
/// 创建登录日志请求体（前端登录成功/失败时调用）
/// </summary>
public class LoginLogCreateRequest
{
    /// <summary>用户名（必填）</summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>登录地址（IP，可选；后端会从 HttpContext 自动补）</summary>
    public string? Ip { get; set; }

    /// <summary>登录地点（可选）</summary>
    public string? Location { get; set; }

    /// <summary>操作系统（可选）</summary>
    public string? Os { get; set; }

    /// <summary>浏览器（可选）</summary>
    public string? Browser { get; set; }

    /// <summary>1=成功 0=失败</summary>
    public int Status { get; set; } = 1;

    /// <summary>描述（登录成功 / 失败原因）</summary>
    public string? Message { get; set; }

    /// <summary>登录时间（可选；不传时由后端填当前时间）</summary>
    public DateTime? LoginTime { get; set; }
}

/// <summary>
/// 批量删除请求
/// </summary>
public class LoginLogBatchDeleteRequest
{
    public List<long> Ids { get; set; } = new();
}

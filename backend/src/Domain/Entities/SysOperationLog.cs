using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 系统操作日志实体
/// <para>
/// 对应表 <c>sys_operation_log</c>。每条记录表示一次"前端 → 后端"的 HTTP 调用：
/// 操作人、模块、类型、状态、耗时、请求/响应内容、客户端 IP 等。
/// 写日志是"系统内"行为，sys_ 前缀符合 RBAC 字典表命名规范。
/// </para>
/// <para>
/// 状态约定（与前端 OperationLogDto.status 对齐）：1 = 成功，0 = 失败。
/// </para>
/// </summary>
[Table("sys_operation_log")]
public class SysOperationLog
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 操作人账号/姓名
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    /// 操作类型：login / logout / create / update / delete / export / import / query / other
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 操作模块：系统管理 / 学生档案 / 数据采集 ...
    /// </summary>
    [MaxLength(50)]
    public string? Module { get; set; }

    /// <summary>
    /// 操作内容（自然语言描述）
    /// </summary>
    [MaxLength(500)]
    public string? Content { get; set; }

    /// <summary>
    /// 客户端 IP
    /// </summary>
    [MaxLength(50)]
    public string? Ip { get; set; }

    /// <summary>
    /// 状态：1=成功 0=失败
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 耗时（毫秒）
    /// </summary>
    public int? CostMs { get; set; }

    /// <summary>
    /// 请求地址，如 /system/role/changeStatus
    /// </summary>
    [MaxLength(200)]
    public string? RequestUrl { get; set; }

    /// <summary>
    /// 请求方式：GET / POST / PUT / DELETE
    /// </summary>
    [MaxLength(10)]
    public string? RequestMethod { get; set; }

    /// <summary>
    /// 操作方法（Java/Controller 全限定方法名 + 括号）
    /// </summary>
    [MaxLength(200)]
    public string? Method { get; set; }

    /// <summary>
    /// 请求参数（JSON 字符串）
    /// </summary>
    public string? RequestParams { get; set; }

    /// <summary>
    /// 返回参数（JSON 字符串）
    /// </summary>
    public string? ResponseParams { get; set; }

    /// <summary>
    /// 错误信息（status=0 时记录）
    /// </summary>
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 浏览器 User-Agent
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// 登录地点（IP 归属地，可选）
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }

    /// <summary>
    /// 业务上下文 JSON（Controller 按需塞）
    /// <para>
    /// 由具体 Controller 在执行过程中通过 <c>HttpContext.Items["BizData"]</c> 设置，
    /// OperationLogFilter 在 finally 块读取并写入本列。
    /// 用于存储"这次操作影响到的业务实体"摘要，例如：
    ///   - 删除角色：<c>{"roleId":2,"roleName":"省级管理员"}</c>
    ///   - 更新学生：<c>{"studentId":100,"studentNo":"2024001","changes":["status","phone"]}</c>
    /// 未设置时由 Filter 写入一个默认摘要（含 action 名/参数数量/响应概要类型等）。
    /// </para>
    /// </summary>
    public string? BizData { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}

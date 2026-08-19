using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 系统登录日志实体
/// <para>
/// 对应表 <c>sys_login_log</c>。每条记录表示一次"用户尝试登录系统"的行为，
/// 包含用户名、登录 IP、归属地、操作系统、浏览器、登录状态、描述、登录时间等。
/// </para>
/// <para>
/// 状态约定（与前端 LoginLogDto.status 对齐）：1 = 成功，0 = 失败。
/// </para>
/// </summary>
[Table("sys_login_log")]
public class SysLoginLog
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 登录地址（IP）
    /// </summary>
    [MaxLength(50)]
    public string? Ip { get; set; }

    /// <summary>
    /// 登录地点
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [MaxLength(50)]
    public string? Os { get; set; }

    /// <summary>
    /// 浏览器
    /// </summary>
    [MaxLength(50)]
    public string? Browser { get; set; }

    /// <summary>
    /// 登录状态：1=成功 0=失败
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 描述（登录成功 / 失败原因）
    /// </summary>
    [MaxLength(500)]
    public string? Message { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime? LoginTime { get; set; }
}

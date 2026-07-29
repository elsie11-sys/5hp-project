using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 系统用户实体
/// </summary>
[Table("sys_user")] // 映射到 PostgreSQL 中的 sys_user 表
public class SysUser
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 用户名（登录账号）
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 姓名（真实姓名）
    /// </summary>
    [MaxLength(50)]
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 用户性别（0:未知, 1:男, 2:女）
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 角色ID（关联角色表）
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// 所属机构ID（关联机构/部门表）
    /// </summary>
    public long OrgId { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 账号状态（1:正常/启用, 0:停用/禁用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 密码哈希（前端新增用户时传入明文，后端负责哈希后落库）
    /// </summary>
    [MaxLength(255)]
    public string? PasswordHash { get; set; }
}
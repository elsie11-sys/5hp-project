using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 系统角色实体（RBAC 核心）
/// </summary>
[Table("sys_role")]
public class SysRole
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 角色名称（显示名）
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色编号（业务标识，唯一，如 ROLE_NATIONAL）
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 权限字符（控制器/注解中使用的权限标识，如 admin / system:user:list）
    /// </summary>
    [MaxLength(100)]
    public string? Permission { get; set; }

    /// <summary>
    /// 角色等级（1~5: 国家级/省级/市级/区县级/学校级）
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 状态（1:启用 0:停用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 数据权限范围：1=全部 / 2=自定义 / 3=本部门 / 4=本部门及以下 / 5=仅本人
    /// （值与 sys_dict 中 DictType=role_based_data_permissions 的 ItemValue 一一对应）
    /// </summary>
    [MaxLength(32)]
    public string DataScope { get; set; } = "1";

    /// <summary>
    /// 备注/描述
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

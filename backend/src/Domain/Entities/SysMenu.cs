using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 菜单/权限项实体（RBAC 权限点）
/// Type: 1=目录 2=菜单 3=按钮
/// </summary>
[Table("sys_menu")]
public class SysMenu
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 权限字符（英文标识）
    /// </summary>
    [MaxLength(50)]
    public string? Code { get; set; }

    /// <summary>
    /// 图标（emoji）
    /// </summary>
    [MaxLength(20)]
    public string? Icon { get; set; }

    /// <summary>
    /// 类型：1=目录 2=菜单 3=按钮
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（1:启用 0:停用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 路由路径
    /// </summary>
    [MaxLength(200)]
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [MaxLength(200)]
    public string? Component { get; set; }

    /// <summary>
    /// 权限标识（如：system:user:list）
    /// </summary>
    [MaxLength(100)]
    public string? Permission { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

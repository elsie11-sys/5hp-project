using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 组织机构实体（部门/机构）
/// </summary>
[Table("sys_org")]
public class SysOrg
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 机构名称
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 机构编码
    /// </summary>
    [MaxLength(50)]
    public string? Code { get; set; }

    /// <summary>
    /// 机构级别：国家级/省级/市级/区县级/学校级
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// 父级机构ID（顶级为 null）
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 排序（数值越小越靠前）
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态（1:正常, 0:停用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 字典项（sys_dict_item）：字典的明细项。
/// 一个字典可以包含多个字典项，通过 DictType 作为外键关联。
/// 主键为 DictType + ItemValue（字典类型 + 字典键值）。
/// </summary>
[Table("sys_dict_item")]
public class SysDictItem
{
    /// <summary>字典类型（外键，关联 sys_dict.DictType）</summary>
    [Required]
    [MaxLength(100)]
    public string DictType { get; set; } = string.Empty;

    /// <summary>字典键值（主键之一，如 "0"、"1"、"2"）</summary>
    [Required]
    [MaxLength(100)]
    public string ItemValue { get; set; } = string.Empty;

    /// <summary>字典标签（显示文本，如 "男"、"女"、"未知"）</summary>
    [Required]
    [MaxLength(100)]
    public string ItemLabel { get; set; } = string.Empty;

    /// <summary>字典编码（可选，用于系统内部标识）</summary>
    // [MaxLength(100)]
    // public string? ItemCode { get; set; }

    /// <summary>排序（数字越小越靠前）</summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>状态：1=正常 / 0=停用</summary>
    public int Status { get; set; } = 1;

    /// <summary>备注</summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>创建时间（UTC）</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>更新时间（UTC）</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>关联的字典（导航属性）</summary>
    [ForeignKey("DictType")]
    public virtual SysDict? Dict { get; set; }
}

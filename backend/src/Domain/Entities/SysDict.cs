using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 字典（sys_dict）：字典列表本身。
/// 一个字典有唯一的 DictType（如 sys_user_sex），
/// 对应的字典项（key-value 对）放在 sys_dict_item 表。
/// </summary>
[Table("sys_dict")]
public class SysDict
{
    /// <summary>字典编号（主键）</summary>
    [Key]
    public long Id { get; set; }

    /// <summary>字典名称（如"用户性别"）</summary>
    [Required]
    [MaxLength(100)]
    public string DictName { get; set; } = string.Empty;

    /// <summary>字典类型（如 sys_user_sex，业务代码全局唯一）</summary>
    [Required]
    [MaxLength(100)]
    public string DictType { get; set; } = string.Empty;

    /// <summary>状态：1=正常 / 0=停用</summary>
    public int Status { get; set; } = 1;

    /// <summary>归属类型：SYSTEM=系统内置 / USER=用户自定义</summary>
    [MaxLength(20)]
    public string OwnerType { get; set; } = "USER";

    /// <summary>备注</summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>创建时间（UTC）</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>更新时间（UTC）</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>字典项列表</summary>
    public ICollection<SysDictItem> DictItems { get; set; } = new List<SysDictItem>();
}

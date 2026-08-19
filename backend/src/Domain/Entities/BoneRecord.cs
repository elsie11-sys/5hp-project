using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生骨骼健康档案
/// </summary>
[Table("bone_record")]
public class BoneRecord
{
    [Key]
    public long Id { get; set; }

    public long StudentId { get; set; }

    public DateTime CheckDate { get; set; }

    /// <summary>骨密度：正常 / 偏低 / 骨质疏松</summary>
    [MaxLength(50)]
    public string BoneDensity { get; set; } = string.Empty;

    /// <summary>骨龄（如 12岁）</summary>
    [MaxLength(20)]
    public string BoneAge { get; set; } = string.Empty;

    /// <summary>维生素 D：充足 / 良好 / 不足</summary>
    [MaxLength(50)]
    public string VitaminD { get; set; } = string.Empty;

    /// <summary>钙水平：正常 / 偏低</summary>
    [MaxLength(50)]
    public string CalciumLevel { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生骨骼健康档案
/// 字段含义见 VisionRecord.cs 头注
/// </summary>
[Table("student_bone_record")]
public class BoneRecord
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long StudentId { get; set; }

    [MaxLength(20)]
    public string StudentNo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string StudentName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Grade { get; set; } = string.Empty;

    [MaxLength(50)]
    public string ClassName { get; set; } = string.Empty;

    public DateTime CheckDate { get; set; }

    /// <summary>骨密度数值（如 0.85）</summary>
    [MaxLength(20)]
    public string BoneDensity { get; set; } = string.Empty;

    /// <summary>骨密度等级：正常 / 偏低 / 骨质疏松</summary>
    [MaxLength(50)]
    public string BoneLevel { get; set; } = string.Empty;

    /// <summary>骨龄（如 8岁）</summary>
    [MaxLength(20)]
    public string BoneAge { get; set; } = string.Empty;

    /// <summary>维生素 D：充足 / 良好 / 不足</summary>
    [MaxLength(50)]
    public string VitaminD { get; set; } = string.Empty;

    /// <summary>钙水平：正常 / 偏低</summary>
    [MaxLength(50)]
    public string CalciumLevel { get; set; } = string.Empty;

    public long RecorderId { get; set; }

    [MaxLength(50)]
    public string RecorderName { get; set; } = string.Empty;

    public DateTime RecordTime { get; set; }

    [MaxLength(20)]
    public string Source { get; set; } = "manual";

    [MaxLength(100)]
    public string? DeviceSn { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "pending";

    public long? ReviewerId { get; set; }

    [MaxLength(50)]
    public string? ReviewerName { get; set; }

    public DateTime? ReviewTime { get; set; }

    [MaxLength(500)]
    public string? ReviewRemark { get; set; }

    [MaxLength(500)]
    public string? Remark { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生健康体重档案
/// 字段含义见 VisionRecord.cs 头注
/// </summary>
[Table("weight_record")]
public class WeightRecord
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

    /// <summary>身高（cm）</summary>
    [MaxLength(20)]
    public string Height { get; set; } = string.Empty;

    /// <summary>体重（kg）</summary>
    [MaxLength(20)]
    public string Weight { get; set; } = string.Empty;

    /// <summary>BMI 值（保留 1 位小数）</summary>
    [MaxLength(20)]
    public string Bmi { get; set; } = string.Empty;

    /// <summary>BMI 等级：偏瘦 / 正常 / 超重 / 肥胖</summary>
    [MaxLength(50)]
    public string BmiLevel { get; set; } = string.Empty;

    /// <summary>腰围（cm）</summary>
    [MaxLength(20)]
    public string WaistCircumference { get; set; } = string.Empty;

    /// <summary>臀围（cm）</summary>
    [MaxLength(20)]
    public string HipCircumference { get; set; } = string.Empty;

    /// <summary>腰臀比（如 0.77）</summary>
    [MaxLength(20)]
    public string Whr { get; set; } = string.Empty;

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

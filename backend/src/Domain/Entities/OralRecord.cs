using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生口腔健康档案
/// 字段含义见 VisionRecord.cs 头注
/// </summary>
[Table("student_oral_record")]
public class OralRecord
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

    /// <summary>乳牙龋数量（颗）</summary>
    public int DecayedBabyTeeth { get; set; }

    /// <summary>恒牙龋数量（颗）</summary>
    public int DecayedPermanentTeeth { get; set; }

    /// <summary>替牙情况：替牙期 / 恒牙期 / 乳牙期</summary>
    [MaxLength(50)]
    public string ToothStage { get; set; } = string.Empty;

    /// <summary>颌面发育：正常 / 地包天 / 龅牙 / 其他</summary>
    [MaxLength(50)]
    public string JawDevelopment { get; set; } = string.Empty;

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

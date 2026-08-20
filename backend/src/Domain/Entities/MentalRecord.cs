using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生心理健康档案
/// 字段含义见 VisionRecord.cs 头注
/// </summary>
[Table("student_mental_record")]
public class MentalRecord
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

    /// <summary>焦虑评分（0-21 等）</summary>
    public int AnxietyScore { get; set; }

    /// <summary>抑郁评分</summary>
    public int DepressionScore { get; set; }

    /// <summary>学习焦虑：正常 / 轻度 / 中度 / 重度</summary>
    [MaxLength(50)]
    public string LearningAnxiety { get; set; } = string.Empty;

    /// <summary>人际敏感：正常 / 轻度 / 中度 / 重度</summary>
    [MaxLength(50)]
    public string InterpersonalSensitivity { get; set; } = string.Empty;

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

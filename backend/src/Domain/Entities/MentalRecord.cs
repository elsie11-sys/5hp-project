using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生心理健康档案
/// </summary>
[Table("mental_record")]
public class MentalRecord
{
    [Key]
    public long Id { get; set; }

    public long StudentId { get; set; }

    public DateTime CheckDate { get; set; }

    /// <summary>压力水平：轻度 / 中度 / 重度</summary>
    [MaxLength(50)]
    public string StressLevel { get; set; } = string.Empty;

    /// <summary>睡眠质量：良好 / 一般 / 较差</summary>
    [MaxLength(50)]
    public string SleepQuality { get; set; } = string.Empty;

    /// <summary>情绪状态：稳定 / 波动 / 焦虑 等</summary>
    [MaxLength(50)]
    public string MoodStatus { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

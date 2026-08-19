using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生健康体重档案
/// </summary>
[Table("weight_record")]
public class WeightRecord
{
    [Key]
    public long Id { get; set; }

    public long StudentId { get; set; }

    public DateTime CheckDate { get; set; }

    /// <summary>身高（如 165cm）</summary>
    [MaxLength(20)]
    public string Height { get; set; } = string.Empty;

    /// <summary>体重（如 52kg）</summary>
    [MaxLength(20)]
    public string Weight { get; set; } = string.Empty;

    /// <summary>BMI 值（保留 1 位小数）</summary>
    [MaxLength(20)]
    public string Bmi { get; set; } = string.Empty;

    /// <summary>BMI 等级：偏瘦 / 正常 / 超重 / 肥胖</summary>
    [MaxLength(50)]
    public string BmiLevel { get; set; } = string.Empty;

    /// <summary>腰围（如 68cm）</summary>
    [MaxLength(20)]
    public string WaistCircumference { get; set; } = string.Empty;

    /// <summary>臀围</summary>
    [MaxLength(20)]
    public string HipCircumference { get; set; } = string.Empty;

    /// <summary>腰臀比（如 0.77）</summary>
    [MaxLength(20)]
    public string Whr { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生口腔健康档案
/// </summary>
[Table("oral_record")]
public class OralRecord
{
    [Key]
    public long Id { get; set; }

    public long StudentId { get; set; }

    public DateTime CheckDate { get; set; }

    /// <summary>口腔状态：良好 / 轻微龋齿 / 中度龋齿 / 重度龋齿 等</summary>
    [MaxLength(50)]
    public string ToothStatus { get; set; } = string.Empty;

    /// <summary>龋齿数量（颗）</summary>
    public int CavityCount { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

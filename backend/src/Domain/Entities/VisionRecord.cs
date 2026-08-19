using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生视力健康档案
/// 同一学生有多条历史记录，按 check_date 倒序，最新一条即为"当前状态"
/// </summary>
[Table("vision_record")]
public class VisionRecord
{
    [Key]
    public long Id { get; set; }

    /// <summary>所属学生 ID（关联 student_archive.id）</summary>
    public long StudentId { get; set; }

    /// <summary>检查日期</summary>
    public DateTime CheckDate { get; set; }

    /// <summary>左眼视力（如 4.8）</summary>
    [MaxLength(20)]
    public string LeftEye { get; set; } = string.Empty;

    /// <summary>右眼视力</summary>
    [MaxLength(20)]
    public string RightEye { get; set; } = string.Empty;

    /// <summary>视力等级：正常/轻度近视/中度近视/高度近视</summary>
    [MaxLength(50)]
    public string VisionLevel { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

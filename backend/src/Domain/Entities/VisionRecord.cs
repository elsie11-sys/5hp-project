using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生视力健康档案
/// 同一学生有多条历史记录，按 check_date 倒序，最新一条即为"当前状态"
///
/// 数据采集与审核流程：班主任/校医录入 → 校医审核 → 区县教育局抽检
/// status: pending(待审核) / approved(已审核) / abnormal(异常) / spot(待抽检)
/// source: manual(单个录入) / excel(Excel 批量导入) / device(设备自动采集)
/// </summary>
[Table("student_vision_record")]
public class VisionRecord
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>所属学生 ID（关联 student_archive.id）</summary>
    public long StudentId { get; set; }

    // ---- 学生冗余信息（便于列表展示与统计，无需 join）----
    [MaxLength(20)]
    public string StudentNo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string StudentName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Grade { get; set; } = string.Empty;

    [MaxLength(50)]
    public string ClassName { get; set; } = string.Empty;

    // ---- 业务数据 ----
    /// <summary>检查日期（业务发生时间）</summary>
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

    // ---- 录入 ----
    public long RecorderId { get; set; }

    [MaxLength(50)]
    public string RecorderName { get; set; } = string.Empty;

    /// <summary>系统录入时间</summary>
    public DateTime RecordTime { get; set; }

    /// <summary>数据来源：manual / excel / device</summary>
    [MaxLength(20)]
    public string Source { get; set; } = "manual";

    /// <summary>设备 SN（设备采集时填写）</summary>
    [MaxLength(100)]
    public string? DeviceSn { get; set; }

    // ---- 审核 ----
    /// <summary>状态：pending(待审核) / approved(已审核) / abnormal(异常) / spot(待抽检)</summary>
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

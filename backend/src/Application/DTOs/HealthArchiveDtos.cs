namespace Application.DTOs;

// ============================================================
// 通用 DTO：分页 / 统计 / 审核 / 设备采集
// ============================================================

/// <summary>统一分页查询条件（5 维度共用）</summary>
public class CollectQueryDto
{
    /// <summary>页码（从 1 开始，默认 1）</summary>
    public int Page { get; set; } = 1;

    /// <summary>每页大小（默认 10）</summary>
    public int PageSize { get; set; } = 10;

    /// <summary>状态筛选：all / pending / approved / abnormal / spot</summary>
    public string Status { get; set; } = "all";

    /// <summary>关键字（学号 / 姓名）</summary>
    public string? Keyword { get; set; }

    /// <summary>年级（可选）</summary>
    public string? Grade { get; set; }

    /// <summary>班级（可选）</summary>
    public string? ClassName { get; set; }
}

/// <summary>统一分页响应</summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

/// <summary>状态统计（5 维度通用）</summary>
public class CollectStatsDto
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Abnormal { get; set; }
    public int SpotCheck { get; set; }
    public int DeviceIngested { get; set; }
    public int ExcelImported { get; set; }
}

/// <summary>状态变更请求（审核 / 驳回 / 抽检）</summary>
public class StatusChangeRequest
{
    /// <summary>新状态：approved / abnormal / spot / pending</summary>
    public string Status { get; set; } = "pending";

    /// <summary>审核人 ID（可空，由后端从 Token 取）</summary>
    public long? ReviewerId { get; set; }

    /// <summary>审核人姓名</summary>
    public string? ReviewerName { get; set; }

    /// <summary>审核意见</summary>
    public string? ReviewRemark { get; set; }
}

/// <summary>设备自动采集请求（5 维度通用）</summary>
public class DeviceIngestRequest<T>
{
    /// <summary>设备 SN，必填</summary>
    public string DeviceSn { get; set; } = string.Empty;

    /// <summary>采集时间（设备本地时间）</summary>
    public DateTime? CollectedAt { get; set; }

    /// <summary>采集人（通常为操作设备的老师/校医）</summary>
    public long? RecorderId { get; set; }
    public string? RecorderName { get; set; }

    /// <summary>具体采集数据列表</summary>
    public List<T> Items { get; set; } = new();
}

/// <summary>设备采集响应</summary>
public class DeviceIngestResponse
{
    public int Total { get; set; }
    public int Success { get; set; }
    public int Failed { get; set; }
    public List<string> Errors { get; set; } = new();
}

// ============================================================
// 视力健康 DTO
// ============================================================
public class VisionRecordDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string CheckDate { get; set; } = string.Empty;
    public string LeftEye { get; set; } = string.Empty;
    public string RightEye { get; set; } = string.Empty;
    public string VisionLevel { get; set; } = string.Empty;
    public long RecorderId { get; set; }
    public string RecorderName { get; set; } = string.Empty;
    public string RecordTime { get; set; } = string.Empty;
    public string Source { get; set; } = "manual";
    public string? DeviceSn { get; set; }
    public string Status { get; set; } = "pending";
    public long? ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public string? ReviewTime { get; set; }
    public string? ReviewRemark { get; set; }
    public string? Remark { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ============================================================
// 口腔健康 DTO
// ============================================================
public class OralRecordDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string CheckDate { get; set; } = string.Empty;
    public int DecayedBabyTeeth { get; set; }
    public int DecayedPermanentTeeth { get; set; }
    public string ToothStage { get; set; } = string.Empty;
    public string JawDevelopment { get; set; } = string.Empty;
    public long RecorderId { get; set; }
    public string RecorderName { get; set; } = string.Empty;
    public string RecordTime { get; set; } = string.Empty;
    public string Source { get; set; } = "manual";
    public string? DeviceSn { get; set; }
    public string Status { get; set; } = "pending";
    public long? ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public string? ReviewTime { get; set; }
    public string? ReviewRemark { get; set; }
    public string? Remark { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ============================================================
// 心理健康 DTO
// ============================================================
public class MentalRecordDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string CheckDate { get; set; } = string.Empty;
    public int AnxietyScore { get; set; }
    public int DepressionScore { get; set; }
    public string LearningAnxiety { get; set; } = string.Empty;
    public string InterpersonalSensitivity { get; set; } = string.Empty;
    public long RecorderId { get; set; }
    public string RecorderName { get; set; } = string.Empty;
    public string RecordTime { get; set; } = string.Empty;
    public string Source { get; set; } = "manual";
    public string? DeviceSn { get; set; }
    public string Status { get; set; } = "pending";
    public long? ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public string? ReviewTime { get; set; }
    public string? ReviewRemark { get; set; }
    public string? Remark { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ============================================================
// 健康体重 DTO
// ============================================================
public class WeightRecordDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string CheckDate { get; set; } = string.Empty;
    public string Height { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string Bmi { get; set; } = string.Empty;
    public string BmiLevel { get; set; } = string.Empty;
    public string WaistCircumference { get; set; } = string.Empty;
    public string HipCircumference { get; set; } = string.Empty;
    public string Whr { get; set; } = string.Empty;
    public long RecorderId { get; set; }
    public string RecorderName { get; set; } = string.Empty;
    public string RecordTime { get; set; } = string.Empty;
    public string Source { get; set; } = "manual";
    public string? DeviceSn { get; set; }
    public string Status { get; set; } = "pending";
    public long? ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public string? ReviewTime { get; set; }
    public string? ReviewRemark { get; set; }
    public string? Remark { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ============================================================
// 骨骼健康 DTO
// ============================================================
public class BoneRecordDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string CheckDate { get; set; } = string.Empty;
    public string BoneDensity { get; set; } = string.Empty;
    public string BoneLevel { get; set; } = string.Empty;
    public string BoneAge { get; set; } = string.Empty;
    public string VitaminD { get; set; } = string.Empty;
    public string CalciumLevel { get; set; } = string.Empty;
    public long RecorderId { get; set; }
    public string RecorderName { get; set; } = string.Empty;
    public string RecordTime { get; set; } = string.Empty;
    public string Source { get; set; } = "manual";
    public string? DeviceSn { get; set; }
    public string Status { get; set; } = "pending";
    public long? ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public string? ReviewTime { get; set; }
    public string? ReviewRemark { get; set; }
    public string? Remark { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

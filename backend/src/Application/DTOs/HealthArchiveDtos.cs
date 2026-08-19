namespace Application.DTOs;

// ============================================================
// 视力健康
// ============================================================
public class VisionRecordDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }

    /// <summary>检查日期（ISO yyyy-MM-dd）</summary>
    public string CheckDate { get; set; } = string.Empty;

    public string LeftEye { get; set; } = string.Empty;
    public string RightEye { get; set; } = string.Empty;
    public string VisionLevel { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ============================================================
// 口腔健康
// ============================================================
public class OralRecordDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string CheckDate { get; set; } = string.Empty;
    public string ToothStatus { get; set; } = string.Empty;
    public int CavityCount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ============================================================
// 心理健康
// ============================================================
public class MentalRecordDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string CheckDate { get; set; } = string.Empty;
    public string StressLevel { get; set; } = string.Empty;
    public string SleepQuality { get; set; } = string.Empty;
    public string MoodStatus { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ============================================================
// 健康体重
// ============================================================
public class WeightRecordDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string CheckDate { get; set; } = string.Empty;
    public string Height { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string Bmi { get; set; } = string.Empty;
    public string BmiLevel { get; set; } = string.Empty;
    public string WaistCircumference { get; set; } = string.Empty;
    public string HipCircumference { get; set; } = string.Empty;
    public string Whr { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ============================================================
// 骨骼健康
// ============================================================
public class BoneRecordDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string CheckDate { get; set; } = string.Empty;
    public string BoneDensity { get; set; } = string.Empty;
    public string BoneAge { get; set; } = string.Empty;
    public string VitaminD { get; set; } = string.Empty;
    public string CalciumLevel { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

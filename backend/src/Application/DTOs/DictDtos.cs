namespace Application.DTOs;

/// <summary>
/// 字典项 DTO（前后端交互的载体）
/// </summary>
public class DictItemDto
{
    public string Id { get; set; } = string.Empty;
    public string DictType { get; set; } = string.Empty;
    //public string? ItemCode { get; set; }
    public string ItemLabel { get; set; } = string.Empty;
    public string ItemValue { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
    public int Status { get; set; } = 1;
    public string? Remark { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 字典 DTO（前后端交互的载体）
/// 时间字段统一用 UTC 字符串（ISO 8601），前端拿到再格式化显示
/// </summary>
public class DictDto
{
    public long Id { get; set; }
    public string DictName { get; set; } = string.Empty;
    public string DictType { get; set; } = string.Empty;
    public int Status { get; set; } = 1;
    public string OwnerType { get; set; } = "USER";
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>状态文本（前端展示用 "正常" / "停用"）</summary>
    public string StatusText => Status == 1 ? "正常" : "停用";

    /// <summary>归属类型文本</summary>
    public string OwnerTypeText => OwnerType == "SYSTEM" ? "系统" : "用户";

    /// <summary>字典项列表</summary>
    public List<DictItemDto> DictItems { get; set; } = new();
}

/// <summary>
/// 字典分页查询条件
/// </summary>
public class DictQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? DictName { get; set; }
    public string? DictType { get; set; }
    public int? Status { get; set; }
    public string? OwnerType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

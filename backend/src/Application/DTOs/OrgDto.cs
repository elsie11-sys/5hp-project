namespace Application.DTOs;

/// <summary>
/// 组织 DTO（用于前端展示和接收新增/编辑数据）
/// </summary>
public class OrgDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string Level { get; set; } = string.Empty;
    public long? ParentId { get; set; }
    public int Sort { get; set; }
    public int Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Remark { get; set; }
    public List<OrgDto>? Children { get; set; }
}

/// <summary>
/// 组织查询参数
/// </summary>
public class OrgQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Name { get; set; }
    public int? Status { get; set; }
}

/// <summary>
/// 批量删除请求
/// </summary>
public class OrgBatchDeleteRequest
{
    public List<long> Ids { get; set; } = new();
}

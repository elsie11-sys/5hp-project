namespace Application.DTOs;

/// <summary>
/// 角色 DTO（用于前端展示和接收新增/编辑数据）
/// </summary>
public class RoleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UserCount { get; set; }
    public List<long>? MenuIds { get; set; }
}

/// <summary>
/// 角色查询参数
/// </summary>
public class RoleQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public string? Name { get; set; }
    public string? Code { get; set; }
    public int? Level { get; set; }
    public int? Status { get; set; }
}

/// <summary>
/// 角色表单（用于新增/编辑）
/// </summary>
public class RoleForm
{
    public long? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Status { get; set; } = 1;
    public string? Remark { get; set; }
}

/// <summary>
/// 批量删除请求
/// </summary>
public class RoleBatchDeleteRequest
{
    public List<long> Ids { get; set; } = new();
}

/// <summary>
/// 分配菜单权限请求
/// </summary>
public class AssignMenuPermissionRequest
{
    public List<long> MenuIds { get; set; } = new();
}

/// <summary>
/// 菜单 DTO
/// </summary>
public class MenuDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Icon { get; set; }
    public int Type { get; set; }
    public long? ParentId { get; set; }
    public int Sort { get; set; }
    public int Status { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Permission { get; set; }
    public int IsExternal { get; set; }
    public string? RouteParams { get; set; }
    public int IsKeepAlive { get; set; }
    public int IsVisible { get; set; }
    public string? Remark { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<MenuDto>? Children { get; set; }
}

/// <summary>
/// 菜单表单（用于新增/编辑）
/// </summary>
public class MenuForm
{
    public long? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Icon { get; set; }
    public int Type { get; set; } = 1;
    public long? ParentId { get; set; }
    public int Sort { get; set; } = 0;
    public int Status { get; set; } = 1;
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Permission { get; set; }
    public int IsExternal { get; set; } = 0;
    public string? RouteParams { get; set; }
    public int IsKeepAlive { get; set; } = 1;
    public int IsVisible { get; set; } = 1;
    public string? Remark { get; set; }
}

/// <summary>
/// 菜单查询参数
/// </summary>
public class MenuQuery
{
    public string? Name { get; set; }
    public int? Status { get; set; }
}

/// <summary>
/// 批量删除菜单请求
/// </summary>
public class MenuBatchDeleteRequest
{
    public List<long> Ids { get; set; } = new();
}

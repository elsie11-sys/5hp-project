namespace Application.DTOs;

/// <summary>
/// Excel 导入行。属性名直接用中文（MiniExcel 默认按属性名匹配列名）
/// Controller 端用 dynamic 拿数据，再手动映射到这个 DTO，避免 Application 层依赖 MiniExcel
/// </summary>
public class UserImportRow
{
    public string? Username { get; set; }
    public string? RealName { get; set; }
    public string? Gender { get; set; }      // "男" / "女" / "未知"
    public string? Role { get; set; }        // 中文角色名
    public string? Org { get; set; }         // 中文机构名
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Status { get; set; }      // "启用" / "禁用"
}

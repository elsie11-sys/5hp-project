namespace Application.DTOs;

// 用于前端展示或接收新增/编辑的数据
public class UserDto
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public int Gender { get; set; }
    public long RoleId { get; set; }
    public long OrgId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Status { get; set; }

    /// <summary>
    /// 明文密码（仅创建时使用，后端会哈希后存储为 PasswordHash）
    /// </summary>
    public string? Password { get; set; }
}
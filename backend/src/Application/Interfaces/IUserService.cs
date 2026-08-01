using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IUserService
{
    // 获取全量用户（用于下拉框等）
    Task<List<UserDto>> GetAllUsersAsync();

    // 【新增】分页查询用户（用于前端表格）
    Task<(List<UserDto> Items, int Total)> GetPagedUsersAsync(int page, int pageSize, string? keyword, string? orgIds = null);

    // 根据 ID 获取用户
    Task<UserDto?> GetUserByIdAsync(long id);

    // 创建用户
    Task<UserDto> CreateUserAsync(UserDto userDto);

    // 更新用户
    Task<UserDto> UpdateUserAsync(UserDto userDto);

    // 删除单个用户
    Task<bool> DeleteUserAsync(long id);

    // 【完善】重置密码：把 PasswordHash 置为 newPassword（生产请先 BCrypt 哈希）
    Task<bool> ResetPasswordAsync(long id, string newPassword);

    // 【完善】批量删除：返回实际删除的数量
    Task<int> BatchDeleteAsync(IReadOnlyList<long> ids);

    // 【完善】批量创建：每行单独校验 + 跳过重复用户，返回 ImportResult
    Task<ImportResult> BulkCreateAsync(IReadOnlyList<UserImportRow> rows);

    // 【新增】导出用户数据（支持按 keyword / orgIds 过滤），返回全量 DTO 列表
    Task<List<UserDto>> ExportUsersAsync(string? keyword = null, string? orgIds = null);
}


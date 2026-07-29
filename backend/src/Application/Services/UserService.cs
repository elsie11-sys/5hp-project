using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services;

public class UserService : IUserService
{
    // 优化：改为注入接口 IApplicationDbContext，而不是具体的 AppDbContext
    private readonly IApplicationDbContext _context;

    public UserService(IApplicationDbContext context)
    {
        _context = context;
    }

    // 保留原方法用于下拉框等需要全量数据的场景
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .AsNoTracking() 
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                RealName = u.RealName,
                Gender = u.Gender,
                RoleId = u.RoleId,
                OrgId = u.OrgId,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                Status = u.Status
            })
            .ToListAsync();
    }

    // 新增分页查询方法
    public async Task<(List<UserDto> Items, int Total)> GetPagedUsersAsync(int page, int pageSize, string? keyword)
    {
        var query = _context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(u => u.Username.Contains(keyword) || u.RealName.Contains(keyword));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                RealName = u.RealName,
                Gender = u.Gender,
                RoleId = u.RoleId,
                OrgId = u.OrgId,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                Status = u.Status
            })
            .ToListAsync();

        return (items, total);
    }

    public async Task<UserDto> CreateUserAsync(UserDto userDto)
    {
        var user = new SysUser
        {
            Username = userDto.Username,
            RealName = userDto.RealName,
            Gender = userDto.Gender,
            RoleId = userDto.RoleId,
            OrgId = userDto.OrgId,
            PhoneNumber = userDto.PhoneNumber,
            Email = userDto.Email,
            Status = userDto.Status,
            // 简化处理：明文落库（生产请用 BCrypt 哈希）。如果 Password 为空，给个默认值
            PasswordHash = string.IsNullOrWhiteSpace(userDto.Password)
                ? "123456" // 默认密码
                : userDto.Password
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        userDto.Id = user.Id;
        return userDto;
    }

    public async Task<UserDto> UpdateUserAsync(UserDto userDto)
    {
        // 注意：DbSet 本身自带 FindAsync 方法，所以这里不需要修改
        var user = await _context.Users.FindAsync(userDto.Id);
        
        if (user == null) throw new KeyNotFoundException($"找不到 ID 为 {userDto.Id} 的用户");

        user.RealName = userDto.RealName;
        user.Gender = userDto.Gender;
        user.RoleId = userDto.RoleId;
        user.OrgId = userDto.OrgId;
        user.PhoneNumber = userDto.PhoneNumber;
        user.Email = userDto.Email;
        user.Status = userDto.Status;

        await _context.SaveChangesAsync();
        return userDto;
    }

    public async Task<bool> DeleteUserAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================
    // 完善：重置密码 / 批量删除 / 批量导入
    // ============================================================

    public async Task<bool> ResetPasswordAsync(long id, string newPassword)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;
        user.PasswordHash = newPassword; // 生产请 BCrypt
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> BatchDeleteAsync(IReadOnlyList<long> ids)
    {
        if (ids == null || ids.Count == 0) return 0;
        var users = await _context.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
        _context.Users.RemoveRange(users);
        await _context.SaveChangesAsync();
        return users.Count;
    }

    public async Task<ImportResult> BulkCreateAsync(IReadOnlyList<UserImportRow> rows)
    {
        var result = new ImportResult { TotalRows = rows.Count };

        if (rows == null || rows.Count == 0) return result;

        var validEntities = new List<SysUser>();
        // 用一个 HashSet 收集本批次内出现过的用户名（防同文件重复）
        var seenUsernames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            int rowNum = i + 2; // +1 跳过表头, +1 转成 1-based 行号

            // 1. 必填校验
            if (string.IsNullOrWhiteSpace(row.Username))
            {
                result.Errors.Add(new ImportError { Row = rowNum, Message = "用户名不能为空" });
                continue;
            }
            if (string.IsNullOrWhiteSpace(row.RealName))
            {
                result.Errors.Add(new ImportError { Row = rowNum, Message = "姓名不能为空" });
                continue;
            }
            if (string.IsNullOrWhiteSpace(row.Password))
            {
                result.Errors.Add(new ImportError { Row = rowNum, Message = "初始密码不能为空" });
                continue;
            }

            // 2. 性别映射（默认 0=未知）
            int gender = row.Gender switch
            {
                "男" => 1,
                "女" => 2,
                _    => 0,
            };

            // 3. 角色映射（中文 → ID）
            if (!RoleNameToId.TryGetValue(row.Role ?? "", out var roleId))
            {
                result.Errors.Add(new ImportError
                {
                    Row = rowNum,
                    Message = $"角色 '{row.Role}' 不在允许列表中（{string.Join("/", RoleNameToId.Keys)}）"
                });
                continue;
            }

            // 4. 机构映射
            if (!OrgNameToId.TryGetValue(row.Org ?? "", out var orgId))
            {
                result.Errors.Add(new ImportError
                {
                    Row = rowNum,
                    Message = $"机构 '{row.Org}' 不在允许列表中（{string.Join("/", OrgNameToId.Keys)}）"
                });
                continue;
            }

            // 5. 状态映射（默认 1=启用）
            int status = row.Status switch
            {
                "禁用" or "停用" => 0,
                _                => 1,
            };

            // 6. 同文件内重复
            if (!seenUsernames.Add(row.Username.Trim()))
            {
                result.Errors.Add(new ImportError
                {
                    Row = rowNum,
                    Message = $"用户名 '{row.Username}' 在本文件中重复"
                });
                continue;
            }

            // 7. 数据库内重复
            var exists = await _context.Users.AnyAsync(u => u.Username == row.Username);
            if (exists)
            {
                result.Errors.Add(new ImportError
                {
                    Row = rowNum,
                    Message = $"用户名 '{row.Username}' 已存在"
                });
                continue;
            }

            // 全部通过 → 加入待插入列表
            validEntities.Add(new SysUser
            {
                Username    = row.Username.Trim(),
                RealName    = row.RealName.Trim(),
                Gender      = gender,
                RoleId      = roleId,
                OrgId       = orgId,
                PhoneNumber = row.PhoneNumber?.Trim() ?? string.Empty,
                Email       = row.Email?.Trim() ?? string.Empty,
                Status      = status,
                PasswordHash = row.Password, // 生产请 BCrypt
            });
        }

        // 8. 一次性插入
        if (validEntities.Count > 0)
        {
            _context.Users.AddRange(validEntities);
            await _context.SaveChangesAsync();
        }

        result.SuccessCount = validEntities.Count;
        result.FailedCount  = result.Errors.Count;
        return result;
    }

    // ============================================================
    // 临时静态映射（前后端共用语义，等 sys_role / sys_org 表上线后改为下拉接口）
    // ============================================================
    private static readonly Dictionary<string, long> RoleNameToId = new()
    {
        ["国家管理员"] = 1, ["省级管理员"] = 2, ["市级管理员"] = 3, ["县级管理员"] = 4,
        ["学校管理员"] = 5, ["校医"]       = 6, ["班主任"]     = 7,
    };

    private static readonly Dictionary<string, long> OrgNameToId = new()
    {
        ["国家教育部"]     = 101, ["江苏省教育厅"]   = 102, ["浙江省教育厅"]   = 103,
        ["南京市教育局"]   = 104, ["苏州市教育局"]   = 105, ["鼓楼区教育局"]   = 106,
        ["南京市第一中学"] = 201, ["南京市金陵中学"] = 202,
    };

    public async Task<UserDto?> GetUserByIdAsync(long id)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                RealName = u.RealName,
                Gender = u.Gender,
                RoleId = u.RoleId,
                OrgId = u.OrgId,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
                Status = u.Status
            })
            .FirstOrDefaultAsync();
    }
}

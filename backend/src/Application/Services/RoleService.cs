using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

/// <summary>
/// 角色管理服务实现
/// </summary>
public class RoleService : IRoleService
{
    /// <summary>
    /// 合法的数据权限范围（与 sys_dict 中 DictType=role_based_data_permissions 的 ItemValue 一致：1~5）
    /// </summary>
    private static readonly HashSet<string> ValidDataScopes = new(StringComparer.Ordinal)
    {
        "1", "2", "3", "4", "5"
    };

    private readonly IApplicationDbContext _context;

    public RoleService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<RoleDto> Items, int Total)> GetPagedRolesAsync(RoleQuery query)
    {
        var q = _context.Roles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
            q = q.Where(r => r.Name.Contains(query.Name));
        if (!string.IsNullOrWhiteSpace(query.Code))
            // 兼容前端单个搜索框：同时匹配「角色编号」和「权限字符」
            q = q.Where(r => r.Code.Contains(query.Code) || (r.Permission != null && r.Permission.Contains(query.Code)));
        if (query.Level.HasValue)
            q = q.Where(r => r.Level == query.Level.Value);
        if (query.Status.HasValue)
            q = q.Where(r => r.Status == query.Status.Value);

        var total = await q.CountAsync();

        var items = await q
            .OrderBy(r => r.Level)
            .ThenBy(r => r.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => MapToDto(r))
            .ToListAsync();

        // 填充用户数
        if (items.Count > 0)
        {
            var roleIds = items.Select(i => i.Id).ToList();
            var userCounts = await _context.Users
                .Where(u => roleIds.Contains(u.RoleId))
                .GroupBy(u => u.RoleId)
                .Select(g => new { RoleId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RoleId, x => x.Count);

            foreach (var item in items)
            {
                item.UserCount = userCounts.GetValueOrDefault(item.Id, 0);
            }
        }

        return (items, total);
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .OrderBy(r => r.Level)
            .ThenBy(r => r.Id)
            .Select(r => MapToDto(r))
            .ToListAsync();

        if (roles.Count > 0)
        {
            var roleIds = roles.Select(r => r.Id).ToList();
            var userCounts = await _context.Users
                .Where(u => roleIds.Contains(u.RoleId))
                .GroupBy(u => u.RoleId)
                .Select(g => new { RoleId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RoleId, x => x.Count);

            foreach (var item in roles)
            {
                item.UserCount = userCounts.GetValueOrDefault(item.Id, 0);
            }
        }

        return roles;
    }

    public async Task<RoleDto?> GetRoleByIdAsync(long id)
    {
        var role = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        if (role == null) return null;
        var dto = MapToDto(role);
        dto.UserCount = await _context.Users.CountAsync(u => u.RoleId == id);
        return dto;
    }

    public async Task<RoleDto> CreateRoleAsync(RoleForm form)
    {
        if (string.IsNullOrWhiteSpace(form.Name))
            throw new ArgumentException("角色名称不能为空");
        if (string.IsNullOrWhiteSpace(form.Code))
            throw new ArgumentException("角色编号不能为空");
        var dataScope = NormalizeDataScope(form.DataScope);

        // 校验 Code 唯一性
        var codeExists = await _context.Roles.AnyAsync(r => r.Code == form.Code);
        if (codeExists)
            throw new InvalidOperationException($"角色编号 {form.Code} 已存在");

        var role = new SysRole
        {
            Name = form.Name.Trim(),
            Code = form.Code.Trim(),
            Permission = string.IsNullOrWhiteSpace(form.Permission) ? null : form.Permission.Trim(),
            Level = form.Level,
            Status = form.Status,
            DataScope = dataScope,
            Remark = form.Remark,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return MapToDto(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(long id, RoleForm form)
    {
        if (string.IsNullOrWhiteSpace(form.Name))
            throw new ArgumentException("角色名称不能为空");
        if (string.IsNullOrWhiteSpace(form.Code))
            throw new ArgumentException("角色编号不能为空");

        var role = await _context.Roles.FindAsync(id);
        if (role == null)
            throw new KeyNotFoundException($"找不到 ID 为 {id} 的角色");

        // 校验 Code 唯一性（排除自身）
        var codeExists = await _context.Roles.AnyAsync(r => r.Code == form.Code && r.Id != id);
        if (codeExists)
            throw new InvalidOperationException($"角色编号 {form.Code} 已存在");

        role.Name = form.Name.Trim();
        role.Code = form.Code.Trim();
        role.Permission = string.IsNullOrWhiteSpace(form.Permission) ? null : form.Permission.Trim();
        role.Level = form.Level;
        role.Status = form.Status;
        role.DataScope = NormalizeDataScope(form.DataScope);
        role.Remark = form.Remark;
        role.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(role);
    }

    private static string NormalizeDataScope(string? input)
    {
        // 空值兜底为 "1"（全部数据权限）
        if (string.IsNullOrWhiteSpace(input)) return "1";
        var trimmed = input.Trim();
        if (!ValidDataScopes.Contains(trimmed))
        {
            // 历史脏值（之前版本误把 ALL/DEPT/DEPT_AND_BELOW/SELF 等英文 enum 写进来）
            // 都会落在这里：打 warning 但不阻塞业务，fallback 到 "1"。
            Console.WriteLine(
                $"[RoleService.NormalizeDataScope] 收到非法的 dataScope={input!}, 已 fallback 到 1");
            return "1";
        }
        return trimmed;
    }

    public async Task<bool> DeleteRoleAsync(long id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return false;

        // 检查是否有用户关联
        var hasUsers = await _context.Users.AnyAsync(u => u.RoleId == id);
        if (hasUsers)
            throw new InvalidOperationException("该角色下存在用户，无法删除");

        // 删除关联的菜单权限
        var roleMenus = _context.RoleMenus.Where(rm => rm.RoleId == id);
        _context.RoleMenus.RemoveRange(roleMenus);

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(int DeletedCount, string Message)> BatchDeleteRolesAsync(IReadOnlyList<long> ids)
    {
        if (ids == null || ids.Count == 0)
            return (0, "请选择要删除的角色");

        // 检查是否有用户关联
        var idsList = ids.ToList();
        var rolesWithUsers = await _context.Users
            .Where(u => idsList.Contains(u.RoleId))
            .Select(u => u.RoleId)
            .Distinct()
            .ToListAsync();

        if (rolesWithUsers.Count > 0)
            return (0, "选中的角色中存在关联用户，无法删除");

        // 删除关联的菜单权限
        var roleMenus = _context.RoleMenus.Where(rm => idsList.Contains(rm.RoleId));
        _context.RoleMenus.RemoveRange(roleMenus);

        var roles = await _context.Roles.Where(r => idsList.Contains(r.Id)).ToListAsync();
        _context.Roles.RemoveRange(roles);
        await _context.SaveChangesAsync();

        return (roles.Count, $"已删除 {roles.Count} 个角色");
    }

    public async Task AssignMenuPermissionAsync(long roleId, IReadOnlyList<long> menuIds)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
            throw new KeyNotFoundException($"找不到 ID 为 {roleId} 的角色");

        // 删除现有权限
        var existing = _context.RoleMenus.Where(rm => rm.RoleId == roleId);
        _context.RoleMenus.RemoveRange(existing);

        // 新增权限
        if (menuIds != null && menuIds.Count > 0)
        {
            var newMenus = menuIds.Distinct()
                .Select(mid => new SysRoleMenu
                {
                    RoleId = roleId,
                    MenuId = mid,
                    CreatedAt = DateTime.UtcNow
                });
            _context.RoleMenus.AddRange(newMenus);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<long>> GetRoleMenuIdsAsync(long roleId)
    {
        return await _context.RoleMenus
            .AsNoTracking()
            .Where(rm => rm.RoleId == roleId)
            .Select(rm => rm.MenuId)
            .ToListAsync();
    }

    public async Task<DataPermissionDto> GetDataPermissionAsync(long roleId)
    {
        var role = await _context.Roles.AsNoTracking()
            .Where(r => r.Id == roleId)
            .Select(r => new { r.DataScope })
            .FirstOrDefaultAsync();

        if (role == null)
            throw new KeyNotFoundException($"找不到 ID 为 {roleId} 的角色");

        // TODO: 2=自定义 时返回关联的部门 ID 列表（目前先返回空，等建 sys_role_data_scope_dept 表后再补）
        return new DataPermissionDto
        {
            DataScope = string.IsNullOrWhiteSpace(role.DataScope) ? "1" : role.DataScope,
            DeptIds = new List<long>()
        };
    }

    public async Task AssignDataPermissionAsync(long roleId, AssignDataPermissionRequest request)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
            throw new KeyNotFoundException($"找不到 ID 为 {roleId} 的角色");

        role.DataScope = NormalizeDataScope(request?.DataScope);
        role.UpdatedAt = DateTime.UtcNow;

        // TODO: CUSTOM 时同步 sys_role_data_scope_dept 关联表
        await _context.SaveChangesAsync();
    }

    public async Task<List<RoleDto>> ExportRolesAsync(RoleQuery query)
    {
        var q = _context.Roles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
            q = q.Where(r => r.Name.Contains(query.Name));
        if (!string.IsNullOrWhiteSpace(query.Code))
            q = q.Where(r => r.Code.Contains(query.Code) || (r.Permission != null && r.Permission.Contains(query.Code)));
        if (query.Level.HasValue)
            q = q.Where(r => r.Level == query.Level.Value);
        if (query.Status.HasValue)
            q = q.Where(r => r.Status == query.Status.Value);

        return await q
            .OrderBy(r => r.Level)
            .ThenBy(r => r.Id)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    private static RoleDto MapToDto(SysRole r)
    {
        return new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Code = r.Code,
            Permission = r.Permission,
            Level = r.Level,
            Status = r.Status,
            DataScope = string.IsNullOrWhiteSpace(r.DataScope) ? "1" : r.DataScope,
            Remark = r.Remark,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}

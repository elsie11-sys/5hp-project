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
            q = q.Where(r => r.Code.Contains(query.Code));
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
            throw new ArgumentException("角色权限字符不能为空");

        // 校验 Code 唯一性
        var exists = await _context.Roles.AnyAsync(r => r.Code == form.Code);
        if (exists)
            throw new InvalidOperationException($"权限字符 {form.Code} 已存在");

        var role = new SysRole
        {
            Name = form.Name.Trim(),
            Code = form.Code.Trim(),
            Level = form.Level,
            Status = form.Status,
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
            throw new ArgumentException("角色权限字符不能为空");

        var role = await _context.Roles.FindAsync(id);
        if (role == null)
            throw new KeyNotFoundException($"找不到 ID 为 {id} 的角色");

        // 校验 Code 唯一性（排除自身）
        var exists = await _context.Roles.AnyAsync(r => r.Code == form.Code && r.Id != id);
        if (exists)
            throw new InvalidOperationException($"权限字符 {form.Code} 已存在");

        role.Name = form.Name.Trim();
        role.Code = form.Code.Trim();
        role.Level = form.Level;
        role.Status = form.Status;
        role.Remark = form.Remark;
        role.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(role);
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

    private static RoleDto MapToDto(SysRole r)
    {
        return new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Code = r.Code,
            Level = r.Level,
            Status = r.Status,
            Remark = r.Remark,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}

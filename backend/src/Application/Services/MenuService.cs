using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Services;

/// <summary>
/// 菜单管理服务实现
/// </summary>
public class MenuService : IMenuService
{
    private readonly IApplicationDbContext _context;

    public MenuService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuDto>> GetMenuTreeAsync(MenuQuery? query = null)
    {
        IQueryable<SysMenu> menus = _context.Menus.AsNoTracking();

        // 应用查询条件
        if (!string.IsNullOrWhiteSpace(query?.Name))
        {
            menus = menus.Where(m => m.Name.Contains(query.Name));
        }
        if (query?.Status.HasValue == true)
        {
            menus = menus.Where(m => m.Status == query.Status.Value);
        }

        var allMenus = await menus
            .OrderBy(m => m.Sort)
            .ThenBy(m => m.Id)
            .Select(m => MapToDto(m))
            .ToListAsync();

        // 构建树结构
        var tree = new List<MenuDto>();
        var dict = allMenus.ToDictionary(m => m.Id);

        foreach (var menu in allMenus)
        {
            if (menu.ParentId == null || !dict.ContainsKey(menu.ParentId.Value))
            {
                tree.Add(menu);
            }
            else
            {
                var parent = dict[menu.ParentId.Value];
                parent.Children ??= new List<MenuDto>();
                parent.Children.Add(menu);
            }
        }

        return tree;
    }

    public async Task<List<MenuDto>> GetAllMenusAsync()
    {
        return await _context.Menus
            .AsNoTracking()
            .OrderBy(m => m.Sort)
            .ThenBy(m => m.Id)
            .Select(m => MapToDto(m))
            .ToListAsync();
    }

    public async Task<MenuDto?> GetMenuByIdAsync(long id)
    {
        var menu = await _context.Menus
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return menu == null ? null : MapToDto(menu);
    }

    public async Task<MenuDto> CreateMenuAsync(MenuForm form)
    {
        var entity = new SysMenu
        {
            Name = form.Name,
            Code = form.Code,
            Icon = form.Icon,
            Type = form.Type,
            ParentId = form.ParentId,
            Sort = form.Sort,
            Status = form.Status,
            Path = form.Path,
            Component = form.Component,
            Permission = form.Permission,
            IsExternal = form.IsExternal,
            RouteParams = form.RouteParams,
            IsKeepAlive = form.IsKeepAlive,
            IsVisible = form.IsVisible,
            Remark = form.Remark,
            // 必须用 DateTime.UtcNow（Npgsql 强制要求 DateTime.Kind=Utc 才能写入 timestamptz）
            CreatedAt = DateTime.UtcNow,
        };

        _context.Menus.Add(entity);
        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<MenuDto> UpdateMenuAsync(long id, MenuForm form)
    {
        var entity = await _context.Menus
            .FirstOrDefaultAsync(m => m.Id == id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"菜单 ID {id} 不存在");
        }

        entity.Name = form.Name;
        entity.Code = form.Code;
        entity.Icon = form.Icon;
        entity.Type = form.Type;
        entity.ParentId = form.ParentId;
        entity.Sort = form.Sort;
        entity.Status = form.Status;
        entity.Path = form.Path;
        entity.Component = form.Component;
        entity.Permission = form.Permission;
        entity.IsExternal = form.IsExternal;
        entity.RouteParams = form.RouteParams;
        entity.IsKeepAlive = form.IsKeepAlive;
        entity.IsVisible = form.IsVisible;
        entity.Remark = form.Remark;
        // 必须用 DateTime.UtcNow（Npgsql 强制要求 DateTime.Kind=Utc 才能写入 timestamptz）
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<bool> DeleteMenuAsync(long id)
    {
        var entity = await _context.Menus
            .FirstOrDefaultAsync(m => m.Id == id);

        if (entity == null)
        {
            return false;
        }

        // 检查是否有子菜单
        var hasChildren = await _context.Menus
            .AnyAsync(m => m.ParentId == id);

        if (hasChildren)
        {
            // 级联删除子菜单
            await DeleteChildrenAsync(id);
        }

        _context.Menus.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task DeleteChildrenAsync(long parentId)
    {
        var children = await _context.Menus
            .Where(m => m.ParentId == parentId)
            .ToListAsync();

        foreach (var child in children)
        {
            // 递归删除子菜单的子菜单
            await DeleteChildrenAsync(child.Id);
            _context.Menus.Remove(child);
        }
    }

    public async Task<(int DeletedCount, string Message)> BatchDeleteMenusAsync(IReadOnlyList<long> ids)
    {
        var deletedCount = 0;

        foreach (var id in ids)
        {
            try
            {
                var result = await DeleteMenuAsync(id);
                if (result) deletedCount++;
            }
            catch
            {
                // 忽略单个删除失败
            }
        }

        return (deletedCount, $"成功删除 {deletedCount} 个菜单");
    }

    private static MenuDto MapToDto(SysMenu m)
    {
        return new MenuDto
        {
            Id = m.Id,
            Name = m.Name,
            Code = m.Code,
            Icon = m.Icon,
            Type = m.Type,
            ParentId = m.ParentId,
            Sort = m.Sort,
            Status = m.Status,
            Path = m.Path,
            Component = m.Component,
            Permission = m.Permission,
            IsExternal = m.IsExternal,
            RouteParams = m.RouteParams,
            IsKeepAlive = m.IsKeepAlive,
            IsVisible = m.IsVisible,
            Remark = m.Remark,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        };
    }
}

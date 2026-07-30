using Application.DTOs;

namespace Application.Interfaces;

/// <summary>
/// 角色管理服务接口
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 分页查询角色列表
    /// </summary>
    Task<(List<RoleDto> Items, int Total)> GetPagedRolesAsync(RoleQuery query);

    /// <summary>
    /// 获取所有角色
    /// </summary>
    Task<List<RoleDto>> GetAllRolesAsync();

    /// <summary>
    /// 根据ID获取单个角色
    /// </summary>
    Task<RoleDto?> GetRoleByIdAsync(long id);

    /// <summary>
    /// 创建角色
    /// </summary>
    Task<RoleDto> CreateRoleAsync(RoleForm form);

    /// <summary>
    /// 更新角色
    /// </summary>
    Task<RoleDto> UpdateRoleAsync(long id, RoleForm form);

    /// <summary>
    /// 删除角色
    /// </summary>
    Task<bool> DeleteRoleAsync(long id);

    /// <summary>
    /// 批量删除角色
    /// </summary>
    Task<(int DeletedCount, string Message)> BatchDeleteRolesAsync(IReadOnlyList<long> ids);

    /// <summary>
    /// 分配菜单权限
    /// </summary>
    Task AssignMenuPermissionAsync(long roleId, IReadOnlyList<long> menuIds);

    /// <summary>
    /// 获取角色已分配的菜单ID列表
    /// </summary>
    Task<List<long>> GetRoleMenuIdsAsync(long roleId);
}

/// <summary>
/// 菜单管理服务接口
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// 获取菜单树
    /// </summary>
    Task<List<MenuDto>> GetMenuTreeAsync();

    /// <summary>
    /// 获取所有菜单（扁平列表）
    /// </summary>
    Task<List<MenuDto>> GetAllMenusAsync();

    /// <summary>
    /// 根据ID获取单个菜单
    /// </summary>
    Task<MenuDto?> GetMenuByIdAsync(long id);

    /// <summary>
    /// 创建菜单
    /// </summary>
    Task<MenuDto> CreateMenuAsync(MenuForm form);

    /// <summary>
    /// 更新菜单
    /// </summary>
    Task<MenuDto> UpdateMenuAsync(long id, MenuForm form);

    /// <summary>
    /// 删除菜单
    /// </summary>
    Task<bool> DeleteMenuAsync(long id);

    /// <summary>
    /// 批量删除菜单
    /// </summary>
    Task<(int DeletedCount, string Message)> BatchDeleteMenusAsync(IReadOnlyList<long> ids);
}

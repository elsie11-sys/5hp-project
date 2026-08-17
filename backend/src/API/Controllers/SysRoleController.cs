using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;

namespace API.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("api/role")]
public class SysRoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public SysRoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // GET: api/role/paged?page=1&pageSize=12&name=xxx&level=1&status=1
    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedRoles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? name = null,
        [FromQuery] string? code = null,
        [FromQuery] int? level = null,
        [FromQuery] int? status = null)
    {
        var query = new RoleQuery
        {
            Page = page,
            PageSize = pageSize,
            Name = name,
            Code = code,
            Level = level,
            Status = status
        };
        var (items, total) = await _roleService.GetPagedRolesAsync(query);
        return Ok(new { items, total });
    }

    // GET: api/role/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    // GET: api/role/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(long id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound(new { message = "角色不存在" });
        return Ok(role);
    }

    // POST: api/role
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] RoleForm form)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                message = "数据格式无效",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });

        try
        {
            var created = await _roleService.CreateRoleAsync(form);
            return Ok(new { id = created.Id, message = "创建成功", createdAt = created.CreatedAt });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // PUT: api/role/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(long id, [FromBody] RoleForm form)
    {
        // 兼容处理：前端表单经常只发字段不发 id，
        // 这里以 URL 上的 id 为准回填；只有当前端显式传了 id 且与 URL 不一致时才报错
        if (form.Id.HasValue && form.Id.Value != id)
            return BadRequest(new { message = "URL中的ID与请求体中的ID不匹配" });
        form.Id = id;

        try
        {
            var updated = await _roleService.UpdateRoleAsync(id, form);
            return Ok(new { message = "更新成功", updatedAt = updated.UpdatedAt });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // DELETE: api/role/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(long id)
    {
        try
        {
            var success = await _roleService.DeleteRoleAsync(id);
            if (!success)
                return NotFound(new { message = "角色不存在，删除失败" });
            return Ok(null);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // POST: api/role/batch-delete
    [HttpPost("batch-delete")]
    public async Task<IActionResult> BatchDelete([FromBody] RoleBatchDeleteRequest request)
    {
        if (request?.Ids == null || request.Ids.Count == 0)
            return BadRequest(new { message = "请选择要删除的角色" });

        var (deletedCount, message) = await _roleService.BatchDeleteRolesAsync(request.Ids);
        return Ok(new { deletedCount, message });
    }

    // POST: api/role/{id}/menu-permission
    [HttpPost("{id}/menu-permission")]
    public async Task<IActionResult> AssignMenuPermission(long id, [FromBody] AssignMenuPermissionRequest request)
    {
        try
        {
            await _roleService.AssignMenuPermissionAsync(id, request?.MenuIds ?? new List<long>());
            return Ok(new { message = "权限分配成功" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // GET: api/role/{id}/menu-permission
    [HttpGet("{id}/menu-permission")]
    public async Task<IActionResult> GetMenuPermission(long id)
    {
        var menuIds = await _roleService.GetRoleMenuIdsAsync(id);
        return Ok(menuIds);
    }

    // GET: api/role/{id}/data-permission
    [HttpGet("{id}/data-permission")]
    public async Task<IActionResult> GetDataPermission(long id)
    {
        try
        {
            var data = await _roleService.GetDataPermissionAsync(id);
            return Ok(data);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // POST: api/role/{id}/data-permission
    [HttpPost("{id}/data-permission")]
    public async Task<IActionResult> AssignDataPermission(long id, [FromBody] AssignDataPermissionRequest? request)
    {
        try
        {
            await _roleService.AssignDataPermissionAsync(id, request ?? new AssignDataPermissionRequest());
            return Ok(new { message = "数据权限分配成功" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/role/export?name=xxx&code=xxx&level=1&status=1
    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] string? name = null,
        [FromQuery] string? code = null,
        [FromQuery] int? level = null,
        [FromQuery] int? status = null)
    {
        var query = new RoleQuery
        {
            Name = name,
            Code = code,
            Level = level,
            Status = status
        };
        var items = await _roleService.ExportRolesAsync(query);

        // MiniExcel 用匿名对象的属性名作为表头；
        // 用中文名以贴近前端“角色列表”的展示
        var rows = items.Select(r => new
        {
            编号 = r.Id,
            角色名称 = r.Name,
            角色编号 = r.Code,
            权限字符 = r.Permission ?? string.Empty,
            等级 = r.Level,
            状态 = r.Status == 1 ? "启用" : "停用",
            关联用户数 = r.UserCount,
            备注 = r.Remark ?? string.Empty,
            创建时间 = r.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
            更新时间 = r.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
        });

        using var ms = new MemoryStream();
        await MiniExcel.SaveAsAsync(ms, rows);
        return File(
            ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"角色导出_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }
}

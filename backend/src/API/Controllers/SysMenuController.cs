using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// 菜单管理控制器
/// </summary>
[ApiController]
[Route("api/menu")]
public class SysMenuController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly ILogger<SysMenuController> _logger;

    public SysMenuController(IMenuService menuService, ILogger<SysMenuController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    [HttpGet("tree")]
    public async Task<IActionResult> GetMenuTree()
    {
        var tree = await _menuService.GetMenuTreeAsync();
        return Ok(new { success = true, data = tree });
    }

    /// <summary>
    /// 获取所有菜单（扁平列表）
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllMenus()
    {
        var menus = await _menuService.GetAllMenusAsync();
        return Ok(new { success = true, data = menus });
    }

    /// <summary>
    /// 根据ID获取菜单
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var menu = await _menuService.GetMenuByIdAsync(id);
        if (menu == null)
        {
            return NotFound(new { success = false, message = $"菜单 ID {id} 不存在" });
        }
        return Ok(new { success = true, data = menu });
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MenuForm form)
    {
        try
        {
            var menu = await _menuService.CreateMenuAsync(form);
            return Ok(new { success = true, data = menu, message = "创建成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建菜单失败");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] MenuForm form)
    {
        try
        {
            var menu = await _menuService.UpdateMenuAsync(id, form);
            return Ok(new { success = true, data = menu, message = "更新成功" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新菜单失败");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var result = await _menuService.DeleteMenuAsync(id);
            if (!result)
            {
                return NotFound(new { success = false, message = $"菜单 ID {id} 不存在" });
            }
            return Ok(new { success = true, message = "删除成功" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除菜单失败");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// 批量删除菜单
    /// </summary>
    [HttpPost("batch-delete")]
    public async Task<IActionResult> BatchDelete([FromBody] MenuBatchDeleteRequest request)
    {
        try
        {
            var (count, message) = await _menuService.BatchDeleteMenusAsync(request.Ids);
            return Ok(new { success = true, data = new { deletedCount = count }, message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除菜单失败");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

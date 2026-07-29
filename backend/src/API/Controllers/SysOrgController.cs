using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// 组织管理控制器
/// </summary>
[ApiController]
[Route("api/org")]
public class SysOrgController : ControllerBase
{
    private readonly IOrgService _orgService;

    public SysOrgController(IOrgService orgService)
    {
        _orgService = orgService;
    }

    // GET: api/org/paged?page=1&pageSize=10&name=xxx&status=1
    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedOrgs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? name = null,
        [FromQuery] int? status = null)
    {
        var query = new OrgQuery
        {
            Page = page,
            PageSize = pageSize,
            Name = name,
            Status = status
        };

        var result = await _orgService.GetPagedOrgsAsync(query);
        return Ok(new { items = result.Items, total = result.Total });
    }

    // GET: api/org/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAllOrgs()
    {
        var orgs = await _orgService.GetAllOrgsAsync();
        return Ok(orgs);
    }

    // GET: api/org/tree
    [HttpGet("tree")]
    public async Task<IActionResult> GetOrgTree()
    {
        var tree = await _orgService.GetOrgTreeAsync();
        return Ok(tree);
    }

    // GET: api/org/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrgById(long id)
    {
        var org = await _orgService.GetOrgByIdAsync(id);
        if (org == null)
        {
            return NotFound(new { message = "组织不存在" });
        }
        return Ok(org);
    }

    // POST: api/org
    [HttpPost]
    public async Task<IActionResult> CreateOrg([FromBody] OrgDto orgDto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(new { message = "数据格式无效", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var created = await _orgService.CreateOrgAsync(orgDto);
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

    // PUT: api/org/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrg(long id, [FromBody] OrgDto orgDto)
    {
        if (id != orgDto.Id)
        {
            return BadRequest(new { message = "URL中的ID与请求体中的ID不匹配" });
        }

        try
        {
            var updated = await _orgService.UpdateOrgAsync(orgDto);
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

    // DELETE: api/org/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrg(long id)
    {
        try
        {
            var success = await _orgService.DeleteOrgAsync(id);
            if (!success)
            {
                // 资源不存在：由全局过滤器包装为 { code: 404, data: null, message: "组织不存在，删除失败" }
                return NotFound(new { message = "组织不存在，删除失败" });
            }
            // 成功：由全局过滤器包装为 { code: 0, data: null, message: "ok" }
            return Ok(null);
        }
        catch (InvalidOperationException ex)
        {
            // 业务异常：由全局异常过滤器包装为 { code: 500, data: null, message: "..." }
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // POST: api/org/batch-delete
    [HttpPost("batch-delete")]
    public async Task<IActionResult> BatchDelete([FromBody] OrgBatchDeleteRequest request)
    {
        if (request?.Ids == null || request.Ids.Count == 0)
        {
            return BadRequest(new { message = "请选择要删除的组织" });
        }

        var (deletedCount, message) = await _orgService.BatchDeleteOrgsAsync(request.Ids);
        // 由全局过滤器包装为 { code: 0, data: { deletedCount, message }, message: "ok" }
        return Ok(new { deletedCount, message });
    }
}

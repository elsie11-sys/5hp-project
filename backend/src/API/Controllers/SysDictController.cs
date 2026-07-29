using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;

namespace API.Controllers;

[ApiController]
[Route("api/dict")]
public class SysDictController : ControllerBase
{
    private readonly IDictService _dictService;

    public SysDictController(IDictService dictService)
    {
        _dictService = dictService;
    }

    /// <summary>分页查询</summary>
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] DictQuery query)
    {
        var (items, total) = await _dictService.GetPagedAsync(query);
        return Ok(new { items, total });
    }

    /// <summary>全量（用于下拉/缓存）</summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _dictService.GetAllAsync();
        return Ok(items);
    }

    /// <summary>按类型查单个</summary>
    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetByType(string type)
    {
        var item = await _dictService.GetByTypeAsync(type);
        if (item == null) return NotFound(new { message = $"字典类型 '{type}' 不存在" });
        return Ok(item);
    }

    /// <summary>按 ID 查单个</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var item = await _dictService.GetByIdAsync(id);
        if (item == null) return NotFound(new { message = "字典不存在" });
        return Ok(item);
    }

    /// <summary>新建</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DictDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DictName)) return BadRequest(new { message = "字典名称不能为空" });
        if (string.IsNullOrWhiteSpace(dto.DictType)) return BadRequest(new { message = "字典类型不能为空" });

        if (await _dictService.ExistsTypeAsync(dto.DictType))
            return BadRequest(new { message = $"字典类型 '{dto.DictType}' 已存在" });

        var created = await _dictService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>更新</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] DictDto dto)
    {
        if (id != dto.Id) return BadRequest(new { message = "URL 中的 ID 与请求体不一致" });
        if (string.IsNullOrWhiteSpace(dto.DictName)) return BadRequest(new { message = "字典名称不能为空" });
        if (string.IsNullOrWhiteSpace(dto.DictType)) return BadRequest(new { message = "字典类型不能为空" });

        if (await _dictService.ExistsTypeAsync(dto.DictType, excludeId: id))
            return BadRequest(new { message = $"字典类型 '{dto.DictType}' 已被其他字典占用" });

        try
        {
            var updated = await _dictService.UpdateAsync(dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>删除单个</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var ok = await _dictService.DeleteAsync(id);
        if (!ok) return NotFound(new { message = "字典不存在" });
        return NoContent();
    }

    /// <summary>批量删除</summary>
    [HttpPost("batch-delete")]
    public async Task<IActionResult> BatchDelete([FromBody] BatchDeleteRequest request)
    {
        if (request?.Ids == null || request.Ids.Count == 0)
            return BadRequest(new { message = "请选择要删除的字典" });
        var count = await _dictService.BatchDeleteAsync(request.Ids);
        return Ok(new { message = $"已删除 {count} 个字典", deletedCount = count });
    }

    /// <summary>检查 DictType 是否被占用（创建/编辑时给前端实时反馈）</summary>
    [HttpGet("check-type")]
    public async Task<IActionResult> CheckType([FromQuery] string type, [FromQuery] long? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(type)) return Ok(new { exists = false });
        var exists = await _dictService.ExistsTypeAsync(type, excludeId);
        return Ok(new { exists });
    }

    /// <summary>导出（按当前查询条件）</summary>
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] DictQuery query)
    {
        var items = await _dictService.ExportAsync(query);

        // 把 DTO 映射成"中文表头"行
        var rows = items.Select(d => new
        {
            字典编号 = d.Id,
            字典名称 = d.DictName,
            字典类型 = d.DictType,
            状态     = d.StatusText,
            备注     = d.Remark ?? "",
            创建时间 = d.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
        });

        using var ms = new MemoryStream();
        await ms.SaveAsAsync(rows);
        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"字典导出_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }

    /// <summary>刷新字典缓存（生产里通常会把字典加载到内存缓存，这里只做占位）</summary>
    [HttpPost("refresh-cache")]
    public IActionResult RefreshCache()
    {
        // 真实场景：调用 _cacheService.ReloadAll();
        // 这里简化为先拉一次全量
        var all = _dictService.GetAllAsync().GetAwaiter().GetResult();
        return Ok(new { message = $"字典缓存已刷新，共 {all.Count} 条" });
    }
}

using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;

namespace API.Controllers;

/// <summary>
/// 系统操作日志控制器
/// <para>路由前缀 <c>api/system/log/operation</c>，与前端 operationLogApi 路径一致。</para>
/// </summary>
[ApiController]
[Route("api/system/log/operation")]
public class SysOperationLogController : ControllerBase
{
    private readonly IOperationLogService _logService;

    public SysOperationLogController(IOperationLogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// 写入一条操作日志（其它 Controller / 全局中间件可调用）
    /// <para>POST /api/system/log/operation  body: OperationLogCreateRequest</para>
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OperationLogCreateRequest? request)
    {
        if (request == null)
            return BadRequest(new { message = "请求体不能为空" });

        try
        {
            var created = await _logService.CreateAsync(request);
            return Ok(new { id = created.Id, message = "日志已记录", createdAt = created.CreatedAt });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 分页查询操作日志
    /// <para>GET /api/system/log/operation/paged?page=1&amp;pageSize=10</para>
    /// </summary>
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? operator_ = null,
        [FromQuery] string? type = null,
        [FromQuery] string? module = null,
        [FromQuery] string? ip = null,
        [FromQuery] int? status = null,
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null)
    {
        // 兜底：pageSize 太大直接压扁到 100，避免被恶意请求扫全表
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = new OperationLogQuery
        {
            Page = page,
            PageSize = pageSize,
            Operator = operator_,
            Type = type,
            Module = module,
            Ip = ip,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
        };

        var (items, total) = await _logService.GetPagedAsync(query);
        return Ok(new { items, total });
    }

    /// <summary>
    /// 批量删除操作日志
    /// <para>POST /api/system/log/operation/batch-delete  body: { ids: [1,2,3] }</para>
    /// </summary>
    [HttpPost("batch-delete")]
    public async Task<IActionResult> BatchDelete([FromBody] OperationLogBatchDeleteRequest? request)
    {
        var ids = request?.Ids ?? new List<long>();
        if (ids.Count == 0)
            return BadRequest(new { message = "请选择要删除的日志" });

        var deletedCount = await _logService.BatchDeleteAsync(ids);
        return Ok(new { deletedCount, message = $"已删除 {deletedCount} 条日志" });
    }

    /// <summary>
    /// 清空全部操作日志
    /// <para>POST /api/system/log/operation/clear</para>
    /// </summary>
    [HttpPost("clear")]
    public async Task<IActionResult> Clear()
    {
        var count = await _logService.ClearAsync();
        return Ok(new { deletedCount = count, message = $"已清空 {count} 条日志" });
    }

    /// <summary>
    /// 导出操作日志（按当前筛选条件全量）
    /// <para>GET /api/system/log/operation/export?...&amp;startDate=...</para>
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] string? operator_ = null,
        [FromQuery] string? type = null,
        [FromQuery] string? module = null,
        [FromQuery] string? ip = null,
        [FromQuery] int? status = null,
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null)
    {
        var query = new OperationLogQuery
        {
            Page = 1,
            PageSize = 10_000, // 导出时拉全量，给个合理上限
            Operator = operator_,
            Type = type,
            Module = module,
            Ip = ip,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
        };

        var items = await _logService.GetAllAsync(query);

        // 中文表头，贴近前端的展示
        var rows = items.Select(x => new
        {
            编号 = x.Id,
            操作人 = x.Operator,
            操作类型 = x.Type,
            操作模块 = x.Module ?? string.Empty,
            操作内容 = x.Content ?? string.Empty,
            IP地址 = x.Ip ?? string.Empty,
            请求方式 = x.RequestMethod ?? string.Empty,
            请求地址 = x.RequestUrl ?? string.Empty,
            操作方法 = x.Method ?? string.Empty,
            状态 = x.Status == 1 ? "成功" : "失败",
            耗时毫秒 = x.CostMs ?? 0,
            登录地点 = x.Location ?? string.Empty,
            错误信息 = x.ErrorMsg ?? string.Empty,
            操作时间 = x.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
        });

        using var ms = new MemoryStream();
        await MiniExcel.SaveAsAsync(ms, rows);
        return File(
            ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"操作日志_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }
}

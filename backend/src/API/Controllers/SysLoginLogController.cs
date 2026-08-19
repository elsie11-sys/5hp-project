using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;

namespace API.Controllers;

/// <summary>
/// 系统登录日志控制器
/// <para>
/// 路由前缀 <c>api/system/log/login</c>，与前端 loginLogApi 路径一致。
/// </para>
/// <para>
/// 这个 Controller 同时是登录日志的写入端点（前端登录成功/失败时调），
/// 也是登录日志的查询/管理端点。OperationLogFilter 已经把
/// <c>/api/system/log/login</c> 加入 Skip 名单，避免"写日志时再写日志"的循环。
/// </para>
/// </summary>
[ApiController]
[Route("api/system/log/login")]
public class SysLoginLogController : ControllerBase
{
    private readonly ILoginLogService _logService;
    private readonly IIpLocationService _ipLocation;

    public SysLoginLogController(ILoginLogService logService, IIpLocationService ipLocation)
    {
        _logService = logService;
        _ipLocation = ipLocation;
    }

    /// <summary>
    /// 写入一条登录日志（前端登录成功/失败时调用）
    /// <para>POST /api/system/log/login  body: LoginLogCreateRequest</para>
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LoginLogCreateRequest? request)
    {
        if (request == null)
            return BadRequest(new { message = "请求体不能为空" });

        try
        {
            // 1) IP 兜底：前端没传就从 HttpContext 拿（X-Forwarded-For 或 RemoteIpAddress）
            if (string.IsNullOrWhiteSpace(request.Ip))
            {
                request.Ip = GetClientIp(HttpContext);
            }

            // 2) 把 IPv6 loopback（::1）转成 127.0.0.1 方便展示
            request.Ip = NormalizeIp(request.Ip);

            // 3) 登录地点：前端没传就用 IP 查归属地
            if (string.IsNullOrWhiteSpace(request.Location))
            {
                request.Location = await _ipLocation.GetLocationAsync(request.Ip, HttpContext.RequestAborted);
            }

            var created = await _logService.CreateAsync(request);
            return Ok(new
            {
                id = created.Id,
                message = "登录日志已记录",
                loginTime = created.LoginTime,
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 分页查询登录日志
    /// <para>GET /api/system/log/login/paged?page=1&amp;pageSize=10</para>
    /// </summary>
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? ip = null,
        [FromQuery] string? userName = null,
        [FromQuery] int? status = null,
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = new LoginLogQuery
        {
            Page = page,
            PageSize = pageSize,
            Ip = ip,
            UserName = userName,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
        };

        var (items, total) = await _logService.GetPagedAsync(query);
        return Ok(new { items, total });
    }

    /// <summary>
    /// 批量删除登录日志
    /// <para>POST /api/system/log/login/batch-delete  body: { ids: [1,2,3] }</para>
    /// </summary>
    [HttpPost("batch-delete")]
    public async Task<IActionResult> BatchDelete([FromBody] LoginLogBatchDeleteRequest? request)
    {
        var ids = request?.Ids ?? new List<long>();
        if (ids.Count == 0)
            return BadRequest(new { message = "请选择要删除的日志" });

        var deletedCount = await _logService.BatchDeleteAsync(ids);
        return Ok(new { deletedCount, message = $"已删除 {deletedCount} 条日志" });
    }

    /// <summary>
    /// 清空全部登录日志
    /// <para>POST /api/system/log/login/clear</para>
    /// </summary>
    [HttpPost("clear")]
    public async Task<IActionResult> Clear()
    {
        var count = await _logService.ClearAsync();
        return Ok(new { deletedCount = count, message = $"已清空 {count} 条日志" });
    }

    /// <summary>
    /// 账户解锁（按用户名）
    /// <para>POST /api/system/log/login/unlock  body: { userName: "xxx" }</para>
    /// </summary>
    [HttpPost("unlock")]
    public async Task<IActionResult> Unlock([FromBody] UnlockRequest? request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.UserName))
            return BadRequest(new { message = "请输入要解锁的用户名" });

        try
        {
            var msg = await _logService.UnlockAsync(request.UserName);
            return Ok(new { message = msg });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 导出登录日志（按当前筛选条件全量）
    /// <para>GET /api/system/log/login/export?...&amp;startDate=...</para>
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] string? ip = null,
        [FromQuery] string? userName = null,
        [FromQuery] int? status = null,
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null)
    {
        var query = new LoginLogQuery
        {
            Page = 1,
            PageSize = 10_000, // 导出时拉全量，给个合理上限
            Ip = ip,
            UserName = userName,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
        };

        var items = await _logService.GetAllAsync(query);

        var rows = items.Select(x => new
        {
            编号 = x.Id,
            用户名称 = x.UserName,
            登录地址 = x.Ip ?? string.Empty,
            登录地点 = x.Location ?? string.Empty,
            操作系统 = x.Os ?? string.Empty,
            浏览器 = x.Browser ?? string.Empty,
            登录状态 = x.Status == 1 ? "成功" : "失败",
            描述 = x.Message ?? string.Empty,
            访问时间 = x.LoginTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
        });

        using var ms = new MemoryStream();
        await MiniExcel.SaveAsAsync(ms, rows);
        return File(
            ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"登录日志_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }

    // ============== 私有辅助 ==============

    /// <summary>
    /// 取客户端 IP（优先 X-Forwarded-For，否则 RemoteIpAddress）
    /// </summary>
    private static string? GetClientIp(HttpContext http)
    {
        var xff = http.Request.Headers["X-Forwarded-For"].ToString();
        if (!string.IsNullOrWhiteSpace(xff))
        {
            var first = xff.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                           .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(first)) return first;
        }
        return http.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    /// IPv6 loopback（::1）转 IPv4 127.0.0.1，方便展示
    /// </summary>
    private static string? NormalizeIp(string? ip)
    {
        if (string.IsNullOrWhiteSpace(ip)) return ip;
        var trimmed = ip.Trim();
        return trimmed == "::1" ? "127.0.0.1" : trimmed;
    }
}

/// <summary>解锁请求体</summary>
public class UnlockRequest
{
    public string? UserName { get; set; }
}

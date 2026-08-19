using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

/// <summary>
/// 操作日志服务实现
/// </summary>
public class OperationLogService : IOperationLogService
{
    private readonly IApplicationDbContext _context;

    public OperationLogService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<OperationLogDto> Items, int Total)> GetPagedAsync(OperationLogQuery query)
    {
        var q = BuildQuery(query);
        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(x => x.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToDto(x))
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<OperationLogDto>> GetAllAsync(OperationLogQuery query)
    {
        var q = BuildQuery(query);
        return await q
            .OrderByDescending(x => x.Id)
            .Select(x => MapToDto(x))
            .ToListAsync();
    }

    public async Task<int> BatchDeleteAsync(IReadOnlyList<long> ids)
    {
        if (ids == null || ids.Count == 0) return 0;
        var idList = ids.ToList();
        var rows = await _context.OperationLogs.Where(x => idList.Contains(x.Id)).ToListAsync();
        if (rows.Count == 0) return 0;
        _context.OperationLogs.RemoveRange(rows);
        await _context.SaveChangesAsync();
        return rows.Count;
    }

    public async Task<int> ClearAsync()
    {
        // 走 RemoveRange + SaveChanges，本项目 Application 层只引了 EF Core 8.0.8（没有 Relational），
        // 所以用不了 ExecuteDeleteAsync 扩展。这里是后台清空操作，性能不是瓶颈，可接受
        var rows = await _context.OperationLogs.ToListAsync();
        if (rows.Count == 0) return 0;
        _context.OperationLogs.RemoveRange(rows);
        await _context.SaveChangesAsync();
        return rows.Count;
    }

    public async Task<OperationLogDto> CreateAsync(OperationLogCreateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Operator))
            throw new ArgumentException("操作人不能为空");
        if (string.IsNullOrWhiteSpace(req.Type))
            throw new ArgumentException("操作类型不能为空");

        var entity = new SysOperationLog
        {
            Operator = req.Operator.Trim(),
            Type = req.Type.Trim().ToLowerInvariant(),
            Module = req.Module,
            Content = req.Content,
            Ip = req.Ip,
            Status = req.Status,
            CostMs = req.CostMs,
            RequestUrl = req.RequestUrl,
            RequestMethod = req.RequestMethod,
            Method = req.Method,
            RequestParams = req.RequestParams,
            ResponseParams = req.ResponseParams,
            ErrorMsg = req.ErrorMsg,
            UserAgent = req.UserAgent,
            Location = req.Location,
            BizData = req.BizData,
            CreatedAt = DateTime.UtcNow,
        };
        _context.OperationLogs.Add(entity);
        await _context.SaveChangesAsync();
        return MapToDto(entity);
    }

    // ============== 私有辅助 ==============

    private IQueryable<SysOperationLog> BuildQuery(OperationLogQuery query)
    {
        var q = _context.OperationLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Operator))
            q = q.Where(x => x.Operator.Contains(query.Operator));
        if (!string.IsNullOrWhiteSpace(query.Type))
            q = q.Where(x => x.Type == query.Type);
        if (!string.IsNullOrWhiteSpace(query.Module))
            q = q.Where(x => x.Module != null && x.Module.Contains(query.Module));
        if (!string.IsNullOrWhiteSpace(query.Ip))
            q = q.Where(x => x.Ip != null && x.Ip.Contains(query.Ip));
        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        // 日期范围：created_at 是 timestamptz，按"当地日历日"过滤；
        // startDate/endDate 是 YYYY-MM-DD 字符串，用 string 范围比较即可
        // （PG 端 timestamptz 序列化为 'yyyy-MM-dd HH:mm:ss+08'，字典序与时间序一致）
        if (!string.IsNullOrWhiteSpace(query.StartDate))
            q = q.Where(x => x.CreatedAt != null && string.Compare(x.CreatedAt.Value.ToString("yyyy-MM-dd"), query.StartDate) >= 0);
        if (!string.IsNullOrWhiteSpace(query.EndDate))
            q = q.Where(x => x.CreatedAt != null && string.Compare(x.CreatedAt.Value.ToString("yyyy-MM-dd"), query.EndDate) <= 0);

        return q;
    }

    private static OperationLogDto MapToDto(SysOperationLog x) => new()
    {
        Id = x.Id,
        Operator = x.Operator,
        Type = x.Type,
        Module = x.Module,
        Content = x.Content,
        Ip = x.Ip,
        Status = x.Status,
        CostMs = x.CostMs,
        RequestUrl = x.RequestUrl,
        RequestMethod = x.RequestMethod,
        Method = x.Method,
        RequestParams = x.RequestParams,
        ResponseParams = x.ResponseParams,
        ErrorMsg = x.ErrorMsg,
        UserAgent = x.UserAgent,
        Location = x.Location,
        BizData = x.BizData,
        CreatedAt = x.CreatedAt,
    };
}

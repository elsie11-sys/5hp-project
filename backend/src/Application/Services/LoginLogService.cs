using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

/// <summary>
/// 登录日志服务实现
/// </summary>
public class LoginLogService : ILoginLogService
{
    private readonly IApplicationDbContext _context;

    public LoginLogService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<LoginLogDto> Items, int Total)> GetPagedAsync(LoginLogQuery query)
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

    public async Task<List<LoginLogDto>> GetAllAsync(LoginLogQuery query)
    {
        var q = BuildQuery(query);
        return await q
            .OrderByDescending(x => x.Id)
            .Select(x => MapToDto(x))
            .ToListAsync();
    }

    public async Task<LoginLogDto> CreateAsync(LoginLogCreateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.UserName))
            throw new ArgumentException("用户名不能为空");

        var entity = new SysLoginLog
        {
            UserName = req.UserName.Trim(),
            Ip = req.Ip,
            Location = req.Location,
            Os = req.Os,
            Browser = req.Browser,
            Status = req.Status,
            Message = req.Message,
            // 登录时间：前端传了就用前端的（保证用户真实登录时间），否则用后端当前时间
            LoginTime = req.LoginTime ?? DateTime.UtcNow,
        };
        _context.LoginLogs.Add(entity);
        await _context.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<int> BatchDeleteAsync(IReadOnlyList<long> ids)
    {
        if (ids == null || ids.Count == 0) return 0;
        var idList = ids.ToList();
        var rows = await _context.LoginLogs.Where(x => idList.Contains(x.Id)).ToListAsync();
        if (rows.Count == 0) return 0;
        _context.LoginLogs.RemoveRange(rows);
        await _context.SaveChangesAsync();
        return rows.Count;
    }

    public async Task<int> ClearAsync()
    {
        var rows = await _context.LoginLogs.ToListAsync();
        if (rows.Count == 0) return 0;
        _context.LoginLogs.RemoveRange(rows);
        await _context.SaveChangesAsync();
        return rows.Count;
    }

    /// <summary>
    /// 解锁账户：当前仅做"清空该用户最近失败记录"+"返回提示文本"。
    /// 真实生产环境应该配合用户表的"是否锁定"字段来解锁，本项目暂不引入用户锁定状态。
    /// </summary>
    public async Task<string> UnlockAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("用户名不能为空");

        // 1) 把该用户最近 24h 内 status=0 的失败记录标记为"已解锁"——这里不引入新字段，
        //    改为：删除该用户最近 24h 内的失败记录，等价于"重置登录失败计数"
        var threshold = DateTime.UtcNow.AddHours(-24);
        var fails = await _context.LoginLogs
            .Where(x => x.UserName == userName && x.Status == 0 && x.LoginTime != null && x.LoginTime >= threshold)
            .ToListAsync();
        if (fails.Count > 0)
        {
            _context.LoginLogs.RemoveRange(fails);
            await _context.SaveChangesAsync();
        }
        return $"账户 [{userName.Trim()}] 已解锁（清理 {fails.Count} 条最近失败记录）";
    }

    // ============== 私有辅助 ==============

    private IQueryable<SysLoginLog> BuildQuery(LoginLogQuery query)
    {
        var q = _context.LoginLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.UserName))
            q = q.Where(x => x.UserName.Contains(query.UserName));
        if (!string.IsNullOrWhiteSpace(query.Ip))
            q = q.Where(x => x.Ip != null && x.Ip.Contains(query.Ip));
        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        // 日期范围：login_time 是 timestamptz，按"当地日历日"过滤；
        // startDate/endDate 是 YYYY-MM-DD 字符串，用 string 范围比较即可
        if (!string.IsNullOrWhiteSpace(query.StartDate))
            q = q.Where(x => x.LoginTime != null && string.Compare(x.LoginTime.Value.ToString("yyyy-MM-dd"), query.StartDate) >= 0);
        if (!string.IsNullOrWhiteSpace(query.EndDate))
            q = q.Where(x => x.LoginTime != null && string.Compare(x.LoginTime.Value.ToString("yyyy-MM-dd"), query.EndDate) <= 0);

        return q;
    }

    private static LoginLogDto MapToDto(SysLoginLog x) => new()
    {
        Id = x.Id,
        UserName = x.UserName,
        Ip = x.Ip,
        Location = x.Location,
        Os = x.Os,
        Browser = x.Browser,
        Status = x.Status,
        Message = x.Message,
        LoginTime = x.LoginTime,
    };
}

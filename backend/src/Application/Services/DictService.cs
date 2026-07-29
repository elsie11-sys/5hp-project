using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class DictService : IDictService
{
    private readonly IApplicationDbContext _context;

    public DictService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<DictDto> Items, int Total)> GetPagedAsync(DictQuery query)
    {
        var q = _context.Dicts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.DictName))
            q = q.Where(d => d.DictName.Contains(query.DictName));

        if (!string.IsNullOrWhiteSpace(query.DictType))
            q = q.Where(d => d.DictType.Contains(query.DictType));

        if (query.Status.HasValue)
            q = q.Where(d => d.Status == query.Status.Value);

        if (!string.IsNullOrWhiteSpace(query.OwnerType))
            q = q.Where(d => d.OwnerType == query.OwnerType);

        if (query.StartDate.HasValue)
            q = q.Where(d => d.CreatedAt >= query.StartDate.Value);

        if (query.EndDate.HasValue)
        {
            var end = query.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            q = q.Where(d => d.CreatedAt <= end);
        }

        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(d => d.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d => new DictDto
            {
                Id = d.Id,
                DictName = d.DictName,
                DictType = d.DictType,
                Status = d.Status,
                OwnerType = d.OwnerType,
                Remark = d.Remark,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync();

        return (items, total);
    }

    public async Task<DictDto?> GetByIdAsync(long id)
    {
        var dict = await _context.Dicts
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dict == null) return null;

        var dto = MapToDto(dict);
        dto.DictItems = await GetDictItemsAsync(dict.DictType);
        return dto;
    }

    public async Task<DictDto?> GetByTypeAsync(string dictType)
    {
        var dict = await _context.Dicts
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DictType == dictType);

        if (dict == null) return null;

        var dto = MapToDto(dict);
        dto.DictItems = await GetDictItemsAsync(dictType);
        return dto;
    }

    public async Task<List<DictDto>> GetAllAsync()
    {
        var dicts = await _context.Dicts
            .AsNoTracking()
            .OrderBy(d => d.DictType)
            .ToListAsync();

        var result = new List<DictDto>();
        foreach (var dict in dicts)
        {
            var dto = MapToDto(dict);
            dto.DictItems = await GetDictItemsAsync(dict.DictType);
            result.Add(dto);
        }
        return result;
    }

    public async Task<DictDto> CreateAsync(DictDto dto)
    {
        var entity = new SysDict
        {
            DictName = dto.DictName.Trim(),
            DictType = dto.DictType.Trim(),
            Status = dto.Status,
            OwnerType = dto.OwnerType ?? "USER",
            Remark = dto.Remark?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        _context.Dicts.Add(entity);
        await _context.SaveChangesAsync();

        dto.Id = entity.Id;
        dto.CreatedAt = entity.CreatedAt;

        if (dto.DictItems != null && dto.DictItems.Any())
        {
            // 强制所有字典项状态与字典一致（正常/停用双向）
            await SaveDictItemsAsync(entity.DictType, dto.DictItems, dto.Status);
        }

        dto.DictItems = await GetDictItemsAsync(entity.DictType);
        return dto;
    }

    public async Task<DictDto> UpdateAsync(DictDto dto)
    {
        var entity = await _context.Dicts.FindAsync(dto.Id);
        if (entity == null) throw new KeyNotFoundException($"找不到 ID 为 {dto.Id} 的字典");

        entity.DictName = dto.DictName.Trim();
        entity.DictType = dto.DictType.Trim();
        entity.Status = dto.Status;
        entity.OwnerType = dto.OwnerType ?? "USER";
        entity.Remark = dto.Remark?.Trim();
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        if (dto.DictItems != null)
        {
            // 允许字典项使用自身的状态，不强制覆盖
            await SaveDictItemsAsync(entity.DictType, dto.DictItems);
        }
        else
        {
            // 如果前端未提交字典项，则级联同步字典项状态
            await CascadeUpdateDictItemsStatusAsync(entity.DictType, dto.Status);
        }

        dto.UpdatedAt = entity.UpdatedAt;
        dto.OwnerType = entity.OwnerType;
        dto.DictItems = await GetDictItemsAsync(entity.DictType);
        return dto;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await _context.Dicts.FindAsync(id);
        if (entity == null) return false;
        _context.Dicts.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> BatchDeleteAsync(IReadOnlyList<long> ids)
    {
        if (ids == null || ids.Count == 0) return 0;
        var entities = await _context.Dicts.Where(d => ids.Contains(d.Id)).ToListAsync();
        _context.Dicts.RemoveRange(entities);
        await _context.SaveChangesAsync();
        return entities.Count;
    }

    public async Task<bool> ExistsTypeAsync(string dictType, long? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(dictType)) return false;
        var q = _context.Dicts.AsNoTracking().Where(d => d.DictType == dictType);
        if (excludeId.HasValue) q = q.Where(d => d.Id != excludeId.Value);
        return await q.AnyAsync();
    }

    public async Task<List<DictDto>> ExportAsync(DictQuery query)
    {
        query.Page = 1;
        query.PageSize = 100_000;
        var (items, _) = await GetPagedAsync(query);
        return items;
    }

    private static DictDto MapToDto(SysDict d)
    {
        return new DictDto
        {
            Id = d.Id,
            DictName = d.DictName,
            DictType = d.DictType,
            Status = d.Status,
            OwnerType = d.OwnerType,
            Remark = d.Remark,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        };
    }

    private async Task<List<DictItemDto>> GetDictItemsAsync(string dictType)
    {
        var items = await _context.DictItems
            .AsNoTracking()
            .Where(i => i.DictType == dictType)
            .OrderBy(i => i.SortOrder)
            .ThenBy(i => i.ItemValue)
            .Select(i => new DictItemDto
            {
                Id = $"{i.DictType}:{i.ItemValue}",
                DictType = i.DictType,
                //ItemCode = i.ItemCode,
                ItemLabel = i.ItemLabel,
                ItemValue = i.ItemValue,
                SortOrder = i.SortOrder,
                Status = i.Status,
                Remark = i.Remark,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync();

        return items;
    }

    /// <summary>
    /// 级联更新字典项下所有字典项的状态（正常/停用双向）
    /// </summary>
    private async Task CascadeUpdateDictItemsStatusAsync(string dictType, int status)
    {
        var items = await _context.DictItems
            .Where(i => i.DictType == dictType && i.Status != status)
            .ToListAsync();

        if (!items.Any()) return;

        foreach (var item in items)
        {
            item.Status = status;
            item.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 保存字典项
    /// </summary>
    /// <param name="dictType">字典类型</param>
    /// <param name="items">字典项列表</param>
    /// <param name="effectiveStatus">强制覆盖的状态。传入则所有项的状态统一为该值（0=停用,1=正常），null 时使用项自身的状态</param>
    private async Task SaveDictItemsAsync(string dictType, List<DictItemDto> items, int? effectiveStatus = null)
    {
        var existing = await _context.DictItems
            .Where(i => i.DictType == dictType)
            .ToListAsync();

        var existingDict = existing.ToDictionary(i => $"{i.DictType}:{i.ItemValue}");

        var incomingKeys = new HashSet<string>();
        var newItems = new List<SysDictItem>();

        foreach (var item in items)
        {
            var key = $"{dictType}:{item.ItemValue}";
            incomingKeys.Add(key);

            // 如果 effectiveStatus 有值（即字典停用），强制使用该状态
            var itemStatus = effectiveStatus ?? item.Status;

            if (existingDict.TryGetValue(key, out var existingItem))
            {
                existingItem.ItemLabel = item.ItemLabel;
                //existingItem.ItemCode = item.ItemCode;
                existingItem.SortOrder = item.SortOrder;
                existingItem.Status = itemStatus;
                existingItem.Remark = item.Remark;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                newItems.Add(new SysDictItem
                {
                    DictType = dictType,
                    ItemValue = item.ItemValue,
                    ItemLabel = item.ItemLabel,
                    //ItemCode = item.ItemCode,
                    SortOrder = item.SortOrder,
                    Status = itemStatus,
                    Remark = item.Remark,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        var toRemove = existing.Where(i => !incomingKeys.Contains($"{i.DictType}:{i.ItemValue}")).ToList();
        if (toRemove.Any())
        {
            _context.DictItems.RemoveRange(toRemove);
        }

        if (newItems.Any())
        {
            _context.DictItems.AddRange(newItems);
        }

        await _context.SaveChangesAsync();
    }
}

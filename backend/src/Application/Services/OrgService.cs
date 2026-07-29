using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

/// <summary>
/// 组织管理服务实现
/// </summary>
public class OrgService : IOrgService
{
    private readonly IApplicationDbContext _context;

    public OrgService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<OrgDto> Items, int Total)> GetPagedOrgsAsync(OrgQuery query)
    {
        var q = _context.Orgs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            q = q.Where(o => o.Name.Contains(query.Name));
        }

        if (query.Status.HasValue)
        {
            q = q.Where(o => o.Status == query.Status.Value);
        }

        var total = await q.CountAsync();

        var items = await q
            .OrderBy(o => o.Sort)
            .ThenBy(o => o.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(o => MapToDto(o))
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<OrgDto>> GetAllOrgsAsync()
    {
        return await _context.Orgs
            .AsNoTracking()
            .OrderBy(o => o.Sort)
            .ThenBy(o => o.Id)
            .Select(o => MapToDto(o))
            .ToListAsync();
    }

    public async Task<List<OrgDto>> GetOrgTreeAsync()
    {
        var allOrgs = await _context.Orgs
            .AsNoTracking()
            .OrderBy(o => o.Sort)
            .ThenBy(o => o.Id)
            .Select(o => MapToDto(o))
            .ToListAsync();

        // 构建树结构
        var tree = new List<OrgDto>();
        var orgDict = allOrgs.ToDictionary(o => o.Id);

        foreach (var org in allOrgs)
        {
            if (org.ParentId == null || !orgDict.ContainsKey(org.ParentId.Value))
            {
                tree.Add(org);
            }
            else
            {
                var parent = orgDict[org.ParentId.Value];
                parent.Children ??= new List<OrgDto>();
                parent.Children.Add(org);
            }
        }

        return tree;
    }

    public async Task<OrgDto?> GetOrgByIdAsync(long id)
    {
        var org = await _context.Orgs
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        return org == null ? null : MapToDto(org);
    }

    public async Task<OrgDto> CreateOrgAsync(OrgDto orgDto)
    {
        try
        {
            // 验证必需字段
            if (string.IsNullOrEmpty(orgDto.Name))
                throw new ArgumentException("机构名称不能为空");
            if (string.IsNullOrEmpty(orgDto.Level))
                throw new ArgumentException("机构级别不能为空");
            
            // 验证字段长度
            if (orgDto.Name.Length > 100)
                throw new ArgumentException("机构名称不能超过100个字符");
            if (!string.IsNullOrEmpty(orgDto.Code) && orgDto.Code.Length > 50)
                throw new ArgumentException("机构编码不能超过50个字符");
            if (orgDto.Level.Length > 20)
                throw new ArgumentException("机构级别不能超过20个字符");

            var org = new SysOrg
            {
                Name = orgDto.Name.Trim(),
                Code = !string.IsNullOrEmpty(orgDto.Code) ? orgDto.Code.Trim() : null,
                Level = orgDto.Level.Trim(),
                ParentId = orgDto.ParentId,
                Sort = orgDto.Sort,
                Status = orgDto.Status,
                CreatedAt = DateTime.UtcNow,  // 使用 UTC 时间以匹配数据库时区设置
                Remark = orgDto.Remark
            };

            _context.Orgs.Add(org);
            
            // 检查可能的外键问题
            if (org.ParentId.HasValue)
            {
                var parentExists = await _context.Orgs.AnyAsync(o => o.Id == org.ParentId.Value);
                if (!parentExists)
                    throw new InvalidOperationException($"上级部门不存在 (ID: {org.ParentId.Value})");
            }
            
            await _context.SaveChangesAsync();

            orgDto.Id = org.Id;
            orgDto.CreatedAt = org.CreatedAt;
            return orgDto;
        }
        catch (ArgumentException)
        {
            throw;  // 参数验证错误直接返回
        }
        catch (InvalidOperationException)
        {
            throw;  // 业务逻辑错误直接返回
        }
        catch (Exception ex)
        {
            // 记录详细错误
            var innerMsg = ex.InnerException?.Message ?? "无内部异常";
            throw new InvalidOperationException($"创建组织失败: {innerMsg}", ex);
        }
    }

    public async Task<OrgDto> UpdateOrgAsync(OrgDto orgDto)
    {
        try
        {
            var org = await _context.Orgs.FindAsync(orgDto.Id);
            if (org == null)
            {
                throw new KeyNotFoundException($"找不到 ID 为 {orgDto.Id} 的组织");
            }

            // 验证必需字段
            if (string.IsNullOrEmpty(orgDto.Name))
                throw new ArgumentException("机构名称不能为空");
            if (string.IsNullOrEmpty(orgDto.Level))
                throw new ArgumentException("机构级别不能为空");
            
            // 验证字段长度
            if (orgDto.Name.Length > 100)
                throw new ArgumentException("机构名称不能超过100个字符");
            if (!string.IsNullOrEmpty(orgDto.Code) && orgDto.Code.Length > 50)
                throw new ArgumentException("机构编码不能超过50个字符");
            if (orgDto.Level.Length > 20)
                throw new ArgumentException("机构级别不能超过20个字符");
            
            // 检查是否把自己设为父级
            if (orgDto.ParentId.HasValue && orgDto.ParentId.Value == orgDto.Id)
                throw new InvalidOperationException("不能将自己设为上级部门");

            // 检查父级是否存在
            if (orgDto.ParentId.HasValue)
            {
                var parentExists = await _context.Orgs.AnyAsync(o => o.Id == orgDto.ParentId.Value);
                if (!parentExists)
                    throw new InvalidOperationException($"上级部门不存在 (ID: {orgDto.ParentId.Value})");
            }

            org.Name = orgDto.Name.Trim();
            org.Code = !string.IsNullOrEmpty(orgDto.Code) ? orgDto.Code.Trim() : null;
            org.Level = orgDto.Level.Trim();
            org.ParentId = orgDto.ParentId;
            org.Sort = orgDto.Sort;
            org.Status = orgDto.Status;
            org.Remark = orgDto.Remark;
            org.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            orgDto.UpdatedAt = org.UpdatedAt;
            orgDto.CreatedAt = org.CreatedAt;
            return orgDto;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var innerMsg = ex.InnerException?.Message ?? "无内部异常";
            throw new InvalidOperationException($"更新组织失败: {innerMsg}", ex);
        }
    }

    public async Task<bool> DeleteOrgAsync(long id)
    {
        var org = await _context.Orgs.FindAsync(id);
        if (org == null) return false;

        // 检查是否有子组织
        var hasChildren = await _context.Orgs.AnyAsync(o => o.ParentId == id);
        if (hasChildren)
        {
            throw new InvalidOperationException("该组织下存在子组织，无法删除");
        }

        _context.Orgs.Remove(org);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(int DeletedCount, string Message)> BatchDeleteOrgsAsync(IReadOnlyList<long> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return (0, "请选择要删除的组织");
        }

        // 检查是否有子组织
        var idsList = ids.ToList();
        var hasChildren = await _context.Orgs.AnyAsync(o => idsList.Contains(o.ParentId!.Value));
        if (hasChildren)
        {
            return (0, "选中的组织中存在子组织，无法删除");
        }

        var orgs = await _context.Orgs.Where(o => idsList.Contains(o.Id)).ToListAsync();
        _context.Orgs.RemoveRange(orgs);
        await _context.SaveChangesAsync();

        return (orgs.Count, $"已删除 {orgs.Count} 个组织");
    }

    private static OrgDto MapToDto(SysOrg o)
    {
        return new OrgDto
        {
            Id = o.Id,
            Name = o.Name,
            Code = o.Code,
            Level = o.Level,
            ParentId = o.ParentId,
            Sort = o.Sort,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            Remark = o.Remark
        };
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<SysUser> Users { get; set; }
    DbSet<SysDict> Dicts { get; set; }
    DbSet<SysDictItem> DictItems { get; set; }
    DbSet<SysOrg> Orgs { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

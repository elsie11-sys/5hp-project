using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<SysUser> Users { get; set; }
    DbSet<SysDict> Dicts { get; set; }
    DbSet<SysDictItem> DictItems { get; set; }
    DbSet<SysOrg> Orgs { get; set; }
    DbSet<SysRole> Roles { get; set; }
    DbSet<SysMenu> Menus { get; set; }
    DbSet<SysRoleMenu> RoleMenus { get; set; }
    DbSet<Student> Students { get; set; }
    DbSet<VisionRecord> VisionRecords { get; set; }
    DbSet<OralRecord> OralRecords { get; set; }
    DbSet<MentalRecord> MentalRecords { get; set; }
    DbSet<WeightRecord> WeightRecords { get; set; }
    DbSet<BoneRecord> BoneRecords { get; set; }
    DbSet<SysOperationLog> OperationLogs { get; set; }
    DbSet<SysLoginLog> LoginLogs { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

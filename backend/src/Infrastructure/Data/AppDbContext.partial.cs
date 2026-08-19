using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public partial class AppDbContext
{
    public DbSet<SysRole> Roles { get; set; } = null!;
    public DbSet<SysMenu> Menus { get; set; } = null!;
    public DbSet<SysRoleMenu> RoleMenus { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<VisionRecord> VisionRecords { get; set; } = null!;
    public DbSet<OralRecord> OralRecords { get; set; } = null!;
    public DbSet<MentalRecord> MentalRecords { get; set; } = null!;
    public DbSet<WeightRecord> WeightRecords { get; set; } = null!;
    public DbSet<BoneRecord> BoneRecords { get; set; } = null!;
    public DbSet<SysOperationLog> OperationLogs { get; set; } = null!;
    public DbSet<SysLoginLog> LoginLogs { get; set; } = null!;
}

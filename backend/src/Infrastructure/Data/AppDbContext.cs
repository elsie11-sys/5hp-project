using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public partial class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<SysUser> Users { get; set; } = null!;
    public DbSet<SysDict> Dicts { get; set; } = null!;
    public DbSet<SysDictItem> DictItems { get; set; } = null!;
    public DbSet<SysOrg> Orgs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SysDict>(entity =>
        {
            entity.Property(d => d.OwnerType)
                .HasDefaultValue("USER")
                .HasMaxLength(20);

            entity.HasIndex(d => d.DictType).IsUnique();

            entity.HasMany(d => d.DictItems)
                .WithOne(i => i.Dict)
                .HasForeignKey(i => i.DictType)
                .HasPrincipalKey(d => d.DictType)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SysDictItem>(entity =>
        {
            entity.HasKey(i => new { i.DictType, i.ItemValue });

            entity.Property(i => i.DictType)
                .HasMaxLength(100);

            entity.Property(i => i.ItemValue)
                .HasMaxLength(100);

            entity.Property(i => i.ItemLabel)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(i => i.Remark)
                .HasMaxLength(500);

            entity.Property(i => i.Status)
                .HasDefaultValue(1);

            entity.Property(i => i.SortOrder)
                .HasDefaultValue(0);
        });

        modelBuilder.Entity<SysOrg>(entity =>
        {
            // 列名映射（PascalCase → snake_case）
            entity.Property(o => o.Id).HasColumnName("id");
            entity.Property(o => o.Name).HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(o => o.Code).HasColumnName("code")
                .HasMaxLength(50);

            entity.Property(o => o.Level).HasColumnName("level")
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(o => o.ParentId).HasColumnName("parent_id")
                .IsRequired(false);

            entity.Property(o => o.Sort).HasColumnName("sort")
                .HasDefaultValue(0);

            entity.Property(o => o.Status).HasColumnName("status")
                .HasDefaultValue(1);

            entity.Property(o => o.CreatedAt).HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            entity.Property(o => o.UpdatedAt).HasColumnName("updated_at");

            entity.Property(o => o.Remark).HasColumnName("remark")
                .HasMaxLength(500);

            // 表名映射
            entity.ToTable("sys_org");

            // 索引
            entity.HasIndex(o => o.ParentId);
            entity.HasIndex(o => o.Status);

            // 种子数据（使用相对合理的历史时间）
            entity.HasData(
                new { Id = 1L, Name = "国家教育部", Code = "GJ-001", Level = "国家级", ParentId = (long?)null, Sort = 0, Status = 1, CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 2L, Name = "江苏省教育厅", Code = "SJ-001", Level = "省级", ParentId = 1L, Sort = 1, Status = 1, CreatedAt = new DateTime(2025, 1, 10, 9, 5, 0, DateTimeKind.Local) },
                new { Id = 3L, Name = "浙江省教育厅", Code = "SJ-002", Level = "省级", ParentId = 1L, Sort = 2, Status = 1, CreatedAt = new DateTime(2025, 1, 10, 9, 10, 0, DateTimeKind.Local) },
                new { Id = 4L, Name = "南京市教育局", Code = "SHI-001", Level = "市级", ParentId = 2L, Sort = 1, Status = 1, CreatedAt = new DateTime(2025, 1, 12, 10, 0, 0, DateTimeKind.Local) },
                new { Id = 5L, Name = "苏州市教育局", Code = "SHI-002", Level = "市级", ParentId = 2L, Sort = 2, Status = 1, CreatedAt = new DateTime(2025, 1, 12, 10, 5, 0, DateTimeKind.Local) },
                new { Id = 6L, Name = "鼓楼区教育局", Code = "QX-001", Level = "区县级", ParentId = 4L, Sort = 1, Status = 1, CreatedAt = new DateTime(2025, 1, 15, 14, 0, 0, DateTimeKind.Local) },
                new { Id = 7L, Name = "南京市第一中学", Code = "XX-001", Level = "学校级", ParentId = 6L, Sort = 1, Status = 1, CreatedAt = new DateTime(2025, 2, 1, 8, 30, 0, DateTimeKind.Local) },
                new { Id = 8L, Name = "南京市金陵中学", Code = "XX-002", Level = "学校级", ParentId = 6L, Sort = 2, Status = 1, CreatedAt = new DateTime(2025, 2, 1, 9, 0, 0, DateTimeKind.Local) }
            );
        });

        // ===== sys_role =====
        modelBuilder.Entity<SysRole>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id");
            entity.Property(r => r.Name).HasColumnName("name")
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(r => r.Code).HasColumnName("code")
                .IsRequired()
                .HasMaxLength(50);
            entity.HasIndex(r => r.Code).IsUnique();
            entity.Property(r => r.Level).HasColumnName("level");
            entity.Property(r => r.Status).HasColumnName("status").HasDefaultValue(1);
            entity.Property(r => r.Remark).HasColumnName("remark").HasMaxLength(500);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");

            entity.ToTable("sys_role");

            entity.HasData(
                new { Id = 1L, Name = "国家管理员", Code = "ROLE_NATIONAL", Level = 1, Status = 1, Remark = "查看全国所有数据，统一下发标准、生成全国报表，管理省级账号", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 2L, Name = "省级管理员", Code = "ROLE_PROVINCE", Level = 2, Status = 1, Remark = "查看本省及各地市数据，生成省级报表、监控地市防控，管理市级账号", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 3L, Name = "市级管理员", Code = "ROLE_CITY", Level = 3, Status = 1, Remark = "查看本市及各县区数据，监控数据质量、发布预警，管理县级账号", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 4L, Name = "县级管理员", Code = "ROLE_DISTRICT", Level = 4, Status = 1, Remark = "查看本县所有学校数据，审核数据、督导防控工作，管理学校账号", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 5L, Name = "学校管理员", Code = "ROLE_SCHOOL", Level = 5, Status = 1, Remark = "仅管理本校数据，录入/审核视力档案，查看本校统计", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 6L, Name = "校医", Code = "ROLE_DOCTOR", Level = 5, Status = 1, Remark = "录入/审核视力数据，查看本校统计分析", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 7L, Name = "班主任", Code = "ROLE_TEACHER", Level = 5, Status = 1, Remark = "仅管理本班学生数据，录入视力档案，查看本班统计", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) }
            );
        });

        // ===== sys_menu =====
        modelBuilder.Entity<SysMenu>(entity =>
        {
            entity.Property(m => m.Id).HasColumnName("id");
            entity.Property(m => m.Name).HasColumnName("name")
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(m => m.Code).HasColumnName("code").HasMaxLength(50);
            entity.Property(m => m.Icon).HasColumnName("icon").HasMaxLength(20);
            entity.Property(m => m.Type).HasColumnName("type");
            entity.Property(m => m.ParentId).HasColumnName("parent_id");
            entity.Property(m => m.Sort).HasColumnName("sort");
            entity.Property(m => m.Status).HasColumnName("status").HasDefaultValue(1);
            entity.Property(m => m.Path).HasColumnName("path").HasMaxLength(200);
            entity.Property(m => m.Component).HasColumnName("component").HasMaxLength(200);
            entity.Property(m => m.Permission).HasColumnName("permission").HasMaxLength(100);
            entity.Property(m => m.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(m => m.UpdatedAt).HasColumnName("updated_at");

            entity.ToTable("sys_menu");

            entity.HasData(
                // 一级目录
                new { Id = 1L, Name = "数据查看权限", Code = "DATA_VIEW", Icon = "📊", Type = 1, ParentId = (long?)null, Sort = 1, Status = 1 },
                new { Id = 7L, Name = "数据操作权限", Code = "DATA_OPERATE", Icon = "📝", Type = 1, ParentId = (long?)null, Sort = 2, Status = 1 },
                new { Id = 13L, Name = "报表权限", Code = "REPORT", Icon = "📋", Type = 1, ParentId = (long?)null, Sort = 3, Status = 1 },
                new { Id = 16L, Name = "预警权限", Code = "ALERT", Icon = "🔔", Type = 1, ParentId = (long?)null, Sort = 4, Status = 1 },
                new { Id = 19L, Name = "系统管理权限", Code = "SYSTEM", Icon = "⚙️", Type = 1, ParentId = (long?)null, Sort = 5, Status = 1 },
                // 数据查看
                new { Id = 2L, Name = "查看全国数据", Code = "VIEW_NATIONAL", Icon = (string?)null, Type = 2, ParentId = 1L, Sort = 1, Status = 1 },
                new { Id = 3L, Name = "查看省级数据", Code = "VIEW_PROVINCE", Icon = (string?)null, Type = 2, ParentId = 1L, Sort = 2, Status = 1 },
                new { Id = 4L, Name = "查看市级数据", Code = "VIEW_CITY", Icon = (string?)null, Type = 2, ParentId = 1L, Sort = 3, Status = 1 },
                new { Id = 5L, Name = "查看县级数据", Code = "VIEW_DISTRICT", Icon = (string?)null, Type = 2, ParentId = 1L, Sort = 4, Status = 1 },
                new { Id = 6L, Name = "查看校级数据", Code = "VIEW_SCHOOL", Icon = (string?)null, Type = 2, ParentId = 1L, Sort = 5, Status = 1 },
                // 数据操作
                new { Id = 8L, Name = "新增数据", Code = "DATA_ADD", Icon = (string?)null, Type = 2, ParentId = 7L, Sort = 1, Status = 1 },
                new { Id = 9L, Name = "编辑数据", Code = "DATA_EDIT", Icon = (string?)null, Type = 2, ParentId = 7L, Sort = 2, Status = 1 },
                new { Id = 10L, Name = "删除数据", Code = "DATA_DELETE", Icon = (string?)null, Type = 2, ParentId = 7L, Sort = 3, Status = 1 },
                new { Id = 11L, Name = "审核数据", Code = "DATA_AUDIT", Icon = (string?)null, Type = 2, ParentId = 7L, Sort = 4, Status = 1 },
                new { Id = 12L, Name = "导入导出", Code = "DATA_IMPORT_EXPORT", Icon = (string?)null, Type = 2, ParentId = 7L, Sort = 5, Status = 1 },
                // 报表
                new { Id = 14L, Name = "生成报表", Code = "REPORT_GENERATE", Icon = (string?)null, Type = 2, ParentId = 13L, Sort = 1, Status = 1 },
                new { Id = 15L, Name = "导出报表", Code = "REPORT_EXPORT", Icon = (string?)null, Type = 2, ParentId = 13L, Sort = 2, Status = 1 },
                // 预警
                new { Id = 17L, Name = "查看预警", Code = "ALERT_VIEW", Icon = (string?)null, Type = 2, ParentId = 16L, Sort = 1, Status = 1 },
                new { Id = 18L, Name = "发布预警", Code = "ALERT_PUBLISH", Icon = (string?)null, Type = 2, ParentId = 16L, Sort = 2, Status = 1 },
                // 系统管理
                new { Id = 20L, Name = "用户管理", Code = "SYS_USER", Icon = (string?)null, Type = 2, ParentId = 19L, Sort = 1, Status = 1 },
                new { Id = 21L, Name = "角色管理", Code = "SYS_ROLE", Icon = (string?)null, Type = 2, ParentId = 19L, Sort = 2, Status = 1 },
                new { Id = 22L, Name = "组织管理", Code = "SYS_ORG", Icon = (string?)null, Type = 2, ParentId = 19L, Sort = 3, Status = 1 },
                new { Id = 23L, Name = "字典管理", Code = "SYS_DICT", Icon = (string?)null, Type = 2, ParentId = 19L, Sort = 4, Status = 1 },
                new { Id = 24L, Name = "日志查看", Code = "SYS_LOG", Icon = (string?)null, Type = 2, ParentId = 19L, Sort = 5, Status = 1 }
            );
        });

        // ===== sys_role_menu =====
        modelBuilder.Entity<SysRoleMenu>(entity =>
        {
            entity.Property(rm => rm.Id).HasColumnName("id");
            entity.Property(rm => rm.RoleId).HasColumnName("role_id");
            entity.Property(rm => rm.MenuId).HasColumnName("menu_id");
            entity.Property(rm => rm.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            entity.HasIndex(rm => new { rm.RoleId, rm.MenuId }).IsUnique();
            entity.HasIndex(rm => rm.RoleId);
            entity.HasIndex(rm => rm.MenuId);

            entity.ToTable("sys_role_menu");
        });
    }
}

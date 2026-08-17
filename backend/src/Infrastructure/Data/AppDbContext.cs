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

        // ===== sys_dict / sys_dict_item 种子数据 =====
        // 角色数据权限范围字典：用于角色管理「分配数据权限」抽屉的权限范围下拉
        modelBuilder.Entity<SysDict>().HasData(
            new
            {
                Id = 100L,
                DictName = "角色数据权限",
                DictType = "role_based_data_permissions",
                Status = 1,
                OwnerType = "SYSTEM",
                Remark = "角色管理中可分配的数据权限范围（全部/自定义/本部门/本部门及以下/仅本人）",
                CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local)
            }
        );
        modelBuilder.Entity<SysDictItem>().HasData(
            new
            {
                DictType = "role_based_data_permissions",
                ItemValue = "1",
                ItemLabel = "全部数据权限",
                SortOrder = 1,
                Status = 1,
                Remark = "可访问所有数据",
                CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local)
            },
            new
            {
                DictType = "role_based_data_permissions",
                ItemValue = "2",
                ItemLabel = "自定义数据权限",
                SortOrder = 2,
                Status = 1,
                Remark = "按指定部门范围访问",
                CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local)
            },
            new
            {
                DictType = "role_based_data_permissions",
                ItemValue = "3",
                ItemLabel = "本部门数据权限",
                SortOrder = 3,
                Status = 1,
                Remark = "仅访问本部门数据",
                CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local)
            },
            new
            {
                DictType = "role_based_data_permissions",
                ItemValue = "4",
                ItemLabel = "本部门及以下数据权限",
                SortOrder = 4,
                Status = 1,
                Remark = "可访问本部门及下级部门数据",
                CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local)
            },
            new
            {
                DictType = "role_based_data_permissions",
                ItemValue = "5",
                ItemLabel = "仅本人数据权限",
                SortOrder = 5,
                Status = 1,
                Remark = "仅能访问自己的数据",
                CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local)
            }
        );

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
            entity.Property(r => r.Permission).HasColumnName("permission").HasMaxLength(100);
            entity.Property(r => r.Level).HasColumnName("level");
            entity.Property(r => r.Status).HasColumnName("status").HasDefaultValue(1);
            entity.Property(r => r.DataScope).HasColumnName("data_scope").HasMaxLength(32).HasDefaultValue("ALL");
            entity.Property(r => r.Remark).HasColumnName("remark").HasMaxLength(500);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");

            entity.ToTable("sys_role");

            entity.HasData(
                new { Id = 1L, Name = "国家管理员", Code = "ROLE_NATIONAL", Permission = "system:national", Level = 1, Status = 1, DataScope = "1", Remark = "查看全国所有数据，统一下发标准、生成全国报表，管理省级账号", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 2L, Name = "省级管理员", Code = "ROLE_PROVINCE", Permission = "system:province", Level = 2, Status = 1, DataScope = "4", Remark = "查看本省及各地市数据，生成省级报表、监控地市防控，管理市级账号", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 3L, Name = "市级管理员", Code = "ROLE_CITY", Permission = "system:city", Level = 3, Status = 1, DataScope = "4", Remark = "查看本市及各县区数据，监控数据质量、发布预警，管理县级账号", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 4L, Name = "县级管理员", Code = "ROLE_DISTRICT", Permission = "system:district", Level = 4, Status = 1, DataScope = "4", Remark = "查看本县所有学校数据，审核数据、督导防控工作，管理学校账号", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 5L, Name = "学校管理员", Code = "ROLE_SCHOOL", Permission = "system:school", Level = 5, Status = 1, DataScope = "3", Remark = "仅管理本校数据，录入/审核视力档案，查看本校统计", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 6L, Name = "校医", Code = "ROLE_DOCTOR", Permission = "system:doctor", Level = 5, Status = 1, DataScope = "3", Remark = "录入/审核视力数据，查看本校统计分析", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) },
                new { Id = 7L, Name = "班主任", Code = "ROLE_TEACHER", Permission = "system:teacher", Level = 5, Status = 1, DataScope = "5", Remark = "仅管理本班学生数据，录入视力档案，查看本班统计", CreatedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Local) }
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
            entity.Property(m => m.Icon).HasColumnName("icon").HasMaxLength(50);
            entity.Property(m => m.Type).HasColumnName("type");
            entity.Property(m => m.ParentId).HasColumnName("parent_id");
            entity.Property(m => m.Sort).HasColumnName("sort");
            entity.Property(m => m.Status).HasColumnName("status").HasDefaultValue(1);
            entity.Property(m => m.Path).HasColumnName("path").HasMaxLength(200);
            entity.Property(m => m.Component).HasColumnName("component").HasMaxLength(200);
            entity.Property(m => m.Permission).HasColumnName("permission").HasMaxLength(100);
            entity.Property(m => m.IsExternal).HasColumnName("is_external").HasDefaultValue(0);
            entity.Property(m => m.RouteParams).HasColumnName("route_params").HasMaxLength(500);
            entity.Property(m => m.IsKeepAlive).HasColumnName("is_keep_alive").HasDefaultValue(1);
            entity.Property(m => m.IsVisible).HasColumnName("is_visible").HasDefaultValue(1);
            entity.Property(m => m.Remark).HasColumnName("remark").HasMaxLength(500);
            entity.Property(m => m.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(m => m.UpdatedAt).HasColumnName("updated_at");

            entity.ToTable("sys_menu");

            entity.HasData(
                // 系统管理 - 一级目录
                new { Id = 1L, Name = "系统管理", Code = "SYSTEM", Icon = "ant-design:settings", Type = 1, ParentId = (long?)null, Sort = 1, Status = 1, IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                // 系统管理子菜单
                new { Id = 2L, Name = "用户管理", Code = "system:user", Icon = "ant-design:user", Type = 2, ParentId = 1L, Sort = 1, Status = 1, Path = "/vision-system/user", Component = "vision-archive/system/user/index", Permission = "system:user:list", IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                new { Id = 3L, Name = "角色管理", Code = "system:role", Icon = "ant-design:team", Type = 2, ParentId = 1L, Sort = 2, Status = 1, Path = "/vision-system/role", Component = "vision-archive/system/role/index", Permission = "system:role:list", IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                new { Id = 4L, Name = "菜单管理", Code = "system:menu", Icon = "ant-design:menu", Type = 2, ParentId = 1L, Sort = 3, Status = 1, Path = "/vision-system/menu", Component = "vision-archive/system/menu/index", Permission = "system:menu:list", IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                new { Id = 5L, Name = "组织管理", Code = "system:org", Icon = "ant-design:apartment", Type = 2, ParentId = 1L, Sort = 4, Status = 1, Path = "/vision-system/org", Component = "vision-archive/system/org/index", Permission = "system:org:list", IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                new { Id = 7L, Name = "字典管理", Code = "system:dict", Icon = "ant-design:book", Type = 2, ParentId = 1L, Sort = 5, Status = 1, Path = "/vision-system/dict", Component = "vision-archive/system/dict/index", Permission = "system:dict:list", IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                new { Id = 10L, Name = "操作日志", Code = "system:log", Icon = "ant-design:file-text", Type = 2, ParentId = 1L, Sort = 6, Status = 1, Path = "/vision-system/log", Component = "vision-archive/system/log/index", Permission = "system:log:list", IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                // 系统监控 - 一级目录
                new { Id = 11L, Name = "系统监控", Code = "MONITOR", Icon = "ant-design:dashboard", Type = 1, ParentId = (long?)null, Sort = 2, Status = 1, IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                new { Id = 12L, Name = "在线用户", Code = "monitor:online", Icon = "ant-design:user", Type = 2, ParentId = 11L, Sort = 1, Status = 1, Path = "/monitor/online", Component = "monitor/online/index", Permission = "monitor:online:list", IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                // 系统工具 - 一级目录
                new { Id = 13L, Name = "系统工具", Code = "TOOLS", Icon = "ant-design:tool", Type = 1, ParentId = (long?)null, Sort = 3, Status = 1, IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                new { Id = 14L, Name = "表单构建", Code = "tools:form", Icon = "ant-design:form", Type = 2, ParentId = 13L, Sort = 1, Status = 1, Path = "/tools/form", Component = "tools/form/index", Permission = "tools:form:list", IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                // 数据查看 - 一级目录
                new { Id = 15L, Name = "数据管理", Code = "DATA", Icon = "ant-design:database", Type = 1, ParentId = (long?)null, Sort = 4, Status = 1, IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                new { Id = 16L, Name = "大屏管理", Code = "screen", Icon = "ant-design:global", Type = 2, ParentId = 15L, Sort = 1, Status = 1, Path = "/screen", Component = "screen/index", Permission = "screen:list", IsExternal = 0, IsKeepAlive = 1, IsVisible = 1 },
                // 系统管理按钮权限
                new { Id = 100L, Name = "用户新增", Code = "", Type = 3, ParentId = 2L, Sort = 1, Status = 1, Permission = "system:user:add", IsExternal = 0, IsKeepAlive = 0, IsVisible = 1 },
                new { Id = 101L, Name = "用户修改", Code = "", Type = 3, ParentId = 2L, Sort = 2, Status = 1, Permission = "system:user:edit", IsExternal = 0, IsKeepAlive = 0, IsVisible = 1 },
                new { Id = 102L, Name = "用户删除", Code = "", Type = 3, ParentId = 2L, Sort = 3, Status = 1, Permission = "system:user:delete", IsExternal = 0, IsKeepAlive = 0, IsVisible = 1 },
                new { Id = 103L, Name = "角色新增", Code = "", Type = 3, ParentId = 3L, Sort = 1, Status = 1, Permission = "system:role:add", IsExternal = 0, IsKeepAlive = 0, IsVisible = 1 },
                new { Id = 104L, Name = "角色修改", Code = "", Type = 3, ParentId = 3L, Sort = 2, Status = 1, Permission = "system:role:edit", IsExternal = 0, IsKeepAlive = 0, IsVisible = 1 },
                new { Id = 105L, Name = "角色删除", Code = "", Type = 3, ParentId = 3L, Sort = 3, Status = 1, Permission = "system:role:delete", IsExternal = 0, IsKeepAlive = 0, IsVisible = 1 }
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

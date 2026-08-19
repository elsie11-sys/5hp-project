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

        // ===== sys_operation_log =====
        modelBuilder.Entity<SysOperationLog>(entity =>
        {
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Operator).HasColumnName("operator")
                .IsRequired().HasMaxLength(50);
            entity.Property(x => x.Type).HasColumnName("type")
                .IsRequired().HasMaxLength(20);
            entity.Property(x => x.Module).HasColumnName("module").HasMaxLength(50);
            entity.Property(x => x.Content).HasColumnName("content").HasMaxLength(500);
            entity.Property(x => x.Ip).HasColumnName("ip").HasMaxLength(50);
            entity.Property(x => x.Status).HasColumnName("status").HasDefaultValue(1);
            entity.Property(x => x.CostMs).HasColumnName("cost_ms");
            entity.Property(x => x.RequestUrl).HasColumnName("request_url").HasMaxLength(200);
            entity.Property(x => x.RequestMethod).HasColumnName("request_method").HasMaxLength(10);
            entity.Property(x => x.Method).HasColumnName("method").HasMaxLength(200);
            entity.Property(x => x.RequestParams).HasColumnName("request_params");
            entity.Property(x => x.ResponseParams).HasColumnName("response_params");
            entity.Property(x => x.ErrorMsg).HasColumnName("error_msg");
            entity.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
            entity.Property(x => x.Location).HasColumnName("location").HasMaxLength(100);
            entity.Property(x => x.BizData).HasColumnName("biz_data");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            entity.ToTable("sys_operation_log");

            // 常用查询索引：操作人 / 类型 / 状态 / 时间
            entity.HasIndex(x => x.Operator);
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.CreatedAt);
        });

        // ===== sys_login_log =====
        modelBuilder.Entity<SysLoginLog>(entity =>
        {
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.UserName).HasColumnName("user_name")
                .IsRequired().HasMaxLength(50);
            entity.Property(x => x.Ip).HasColumnName("ip").HasMaxLength(50);
            entity.Property(x => x.Location).HasColumnName("location").HasMaxLength(100);
            entity.Property(x => x.Os).HasColumnName("os").HasMaxLength(50);
            entity.Property(x => x.Browser).HasColumnName("browser").HasMaxLength(50);
            entity.Property(x => x.Status).HasColumnName("status").HasDefaultValue(1);
            entity.Property(x => x.Message).HasColumnName("message").HasMaxLength(500);
            entity.Property(x => x.LoginTime).HasColumnName("login_time").HasDefaultValueSql("NOW()");

            entity.ToTable("sys_login_log");

            // 常用查询索引：用户名 / IP / 状态 / 时间
            entity.HasIndex(x => x.UserName);
            entity.HasIndex(x => x.Ip);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.LoginTime);
        });

        // ===== student_archive =====
        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(s => s.Id).HasColumnName("id");
            entity.Property(s => s.StudentNo).HasColumnName("student_no")
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(s => s.Avatar).HasColumnName("avatar").HasMaxLength(500);
            entity.Property(s => s.GradeYear).HasColumnName("grade_year").HasMaxLength(50);
            entity.Property(s => s.School).HasColumnName("school").HasMaxLength(100);
            entity.Property(s => s.ClassName).HasColumnName("class_name").HasMaxLength(100);
            entity.Property(s => s.Name).HasColumnName("name")
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(s => s.Gender).HasColumnName("gender").HasMaxLength(10);
            entity.Property(s => s.Nation).HasColumnName("nation").HasMaxLength(50);
            entity.Property(s => s.Birthday).HasColumnName("birthday");
            entity.Property(s => s.IdCard).HasColumnName("id_card").HasMaxLength(50);
            entity.Property(s => s.NativePlace).HasColumnName("native_place").HasMaxLength(100);
            entity.Property(s => s.Address).HasColumnName("address").HasMaxLength(255);
            entity.Property(s => s.ParentPhone).HasColumnName("parent_phone").HasMaxLength(20);
            entity.Property(s => s.ParentContact).HasColumnName("parent_contact").HasMaxLength(100);
            entity.Property(s => s.AllergyHistory).HasColumnName("allergy_history").HasMaxLength(255);
            entity.Property(s => s.FatherMyopia).HasColumnName("father_myopia").HasMaxLength(50);
            entity.Property(s => s.MotherMyopia).HasColumnName("mother_myopia").HasMaxLength(50);
            entity.Property(s => s.DataSource).HasColumnName("data_source").HasMaxLength(50);
            entity.Property(s => s.Intervention).HasColumnName("intervention").HasDefaultValue(false);
            entity.Property(s => s.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(s => s.UpdatedAt).HasColumnName("updated_at");

            entity.ToTable("student_archive");

            // 学号唯一索引
            entity.HasIndex(s => s.StudentNo).IsUnique();
            // 常用查询索引
            entity.HasIndex(s => s.School);
            entity.HasIndex(s => s.ClassName);
            entity.HasIndex(s => s.Name);
            entity.HasIndex(s => s.Intervention);
            entity.HasIndex(s => s.CreatedAt);

            // 种子数据：把前端 mock 的 6 条数据灌进来
            var now = new DateTime(2026, 8, 17, 21, 0, 0, DateTimeKind.Local);
            entity.HasData(
                new { Id = 1L, StudentNo = "2022001", Avatar = "👦", GradeYear = "2022级", School = "淮安外国语学校", ClassName = "七年级五班", Name = "陈熙航", Gender = "男", Nation = "汉族", Birthday = (DateTime?)new DateTime(2011, 2, 18), IdCard = "320829201108090012", NativePlace = "江苏淮安", Address = "江苏淮安淮安区淮安区新城广场22栋", ParentPhone = "15952305677", ParentContact = "chenxh_2022", AllergyHistory = "未知", FatherMyopia = "未知", MotherMyopia = "未知", DataSource = "sjwjuser1", Intervention = true, CreatedAt = (DateTime?)now, UpdatedAt = (DateTime?)now },
                new { Id = 2L, StudentNo = "2022002", Avatar = "👦", GradeYear = "2022级", School = "淮安外国语学校", ClassName = "七年级五班", Name = "席振轩", Gender = "男", Nation = "汉族", Birthday = (DateTime?)new DateTime(2011, 8, 9), IdCard = "320829201108090012", NativePlace = "江苏淮安", Address = "江苏淮安淮安区小区8栋301", ParentPhone = "15949198257", ParentContact = "xizhenxuan", AllergyHistory = "未知", FatherMyopia = "未知", MotherMyopia = "未知", DataSource = "sjwjuser1", Intervention = false, CreatedAt = (DateTime?)now, UpdatedAt = (DateTime?)now },
                new { Id = 3L, StudentNo = "2022003", Avatar = "👧", GradeYear = "2022级", School = "淮安外国语学校", ClassName = "七年级五班", Name = "丁书婉", Gender = "女", Nation = "汉族", Birthday = (DateTime?)new DateTime(2011, 2, 12), IdCard = "320831201102121422", NativePlace = "江苏淮安", Address = "江苏淮安淮安区花苑3栋502", ParentPhone = "13952371506", ParentContact = "dingshuwan", AllergyHistory = "未知", FatherMyopia = "未知", MotherMyopia = "未知", DataSource = "sjwjuser1", Intervention = false, CreatedAt = (DateTime?)now, UpdatedAt = (DateTime?)now },
                new { Id = 4L, StudentNo = "2022004", Avatar = "👧", GradeYear = "2022级", School = "淮安外国语学校", ClassName = "七年级五班", Name = "周梓萌", Gender = "女", Nation = "汉族", Birthday = (DateTime?)new DateTime(2011, 3, 14), IdCard = "320803201103140048", NativePlace = "江苏淮安", Address = "江苏淮安淮安区小区1幢2单元403", ParentPhone = "13952390315", ParentContact = "zhouzimeng", AllergyHistory = "未知", FatherMyopia = "未知", MotherMyopia = "未知", DataSource = "sjwjuser1", Intervention = true, CreatedAt = (DateTime?)now, UpdatedAt = (DateTime?)now },
                new { Id = 5L, StudentNo = "2022005", Avatar = "👦", GradeYear = "2022级", School = "淮安外国语学校", ClassName = "七年级五班", Name = "朱宇土", Gender = "男", Nation = "汉族", Birthday = (DateTime?)new DateTime(2011, 5, 2), IdCard = "320831201105021422", NativePlace = "江苏淮安", Address = "江苏淮安淮安区小区9栋304", ParentPhone = "15152566388", ParentContact = "zhuyutu", AllergyHistory = "未知", FatherMyopia = "未知", MotherMyopia = "未知", DataSource = "sjwjuser1", Intervention = false, CreatedAt = (DateTime?)now, UpdatedAt = (DateTime?)now },
                new { Id = 6L, StudentNo = "2022006", Avatar = "👦", GradeYear = "2022级", School = "淮安外国语学校", ClassName = "七年级五班", Name = "招崧熙", Gender = "男", Nation = "汉族", Birthday = (DateTime?)new DateTime(2010, 11, 23), IdCard = "320831201011232637", NativePlace = "江苏淮安", Address = "江苏淮安淮安区小区2栋1001", ParentPhone = "13805285721", ParentContact = "zhaosongxi", AllergyHistory = "未知", FatherMyopia = "未知", MotherMyopia = "未知", DataSource = "sjwjuser1", Intervention = false, CreatedAt = (DateTime?)now, UpdatedAt = (DateTime?)now }
            );
        });

        // ===== vision_record =====
        modelBuilder.Entity<VisionRecord>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id");
            entity.Property(r => r.StudentId).HasColumnName("student_id");
            entity.Property(r => r.CheckDate).HasColumnName("check_date");
            entity.Property(r => r.LeftEye).HasColumnName("left_eye").HasMaxLength(20);
            entity.Property(r => r.RightEye).HasColumnName("right_eye").HasMaxLength(20);
            entity.Property(r => r.VisionLevel).HasColumnName("vision_level").HasMaxLength(50);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("vision_record");
            entity.HasIndex(r => new { r.StudentId, r.CheckDate });
        });

        // ===== oral_record =====
        modelBuilder.Entity<OralRecord>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id");
            entity.Property(r => r.StudentId).HasColumnName("student_id");
            entity.Property(r => r.CheckDate).HasColumnName("check_date");
            entity.Property(r => r.ToothStatus).HasColumnName("tooth_status").HasMaxLength(50);
            entity.Property(r => r.CavityCount).HasColumnName("cavity_count");
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("oral_record");
            entity.HasIndex(r => new { r.StudentId, r.CheckDate });
        });

        // ===== mental_record =====
        modelBuilder.Entity<MentalRecord>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id");
            entity.Property(r => r.StudentId).HasColumnName("student_id");
            entity.Property(r => r.CheckDate).HasColumnName("check_date");
            entity.Property(r => r.StressLevel).HasColumnName("stress_level").HasMaxLength(50);
            entity.Property(r => r.SleepQuality).HasColumnName("sleep_quality").HasMaxLength(50);
            entity.Property(r => r.MoodStatus).HasColumnName("mood_status").HasMaxLength(50);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("mental_record");
            entity.HasIndex(r => new { r.StudentId, r.CheckDate });
        });

        // ===== weight_record =====
        modelBuilder.Entity<WeightRecord>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id");
            entity.Property(r => r.StudentId).HasColumnName("student_id");
            entity.Property(r => r.CheckDate).HasColumnName("check_date");
            entity.Property(r => r.Height).HasColumnName("height").HasMaxLength(20);
            entity.Property(r => r.Weight).HasColumnName("weight").HasMaxLength(20);
            entity.Property(r => r.Bmi).HasColumnName("bmi").HasMaxLength(20);
            entity.Property(r => r.BmiLevel).HasColumnName("bmi_level").HasMaxLength(50);
            entity.Property(r => r.WaistCircumference).HasColumnName("waist_circumference").HasMaxLength(20);
            entity.Property(r => r.HipCircumference).HasColumnName("hip_circumference").HasMaxLength(20);
            entity.Property(r => r.Whr).HasColumnName("whr").HasMaxLength(20);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("weight_record");
            entity.HasIndex(r => new { r.StudentId, r.CheckDate });
        });

        // ===== bone_record =====
        modelBuilder.Entity<BoneRecord>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id");
            entity.Property(r => r.StudentId).HasColumnName("student_id");
            entity.Property(r => r.CheckDate).HasColumnName("check_date");
            entity.Property(r => r.BoneDensity).HasColumnName("bone_density").HasMaxLength(50);
            entity.Property(r => r.BoneAge).HasColumnName("bone_age").HasMaxLength(20);
            entity.Property(r => r.VitaminD).HasColumnName("vitamin_d").HasMaxLength(50);
            entity.Property(r => r.CalciumLevel).HasColumnName("calcium_level").HasMaxLength(50);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("bone_record");
            entity.HasIndex(r => new { r.StudentId, r.CheckDate });
        });

        // ============================================================
        // 健康档案种子数据：6 个学生 × 5 个维度 × 3 条历史 = 90 条
        // 招崧熙 (id=6) 用与前端图片一致的数据；其他学生用合理近似值
        // ============================================================
        var seedNow = new DateTime(2026, 8, 17, 21, 0, 0, DateTimeKind.Local);

        // ---- vision_record (18 条) ----
        modelBuilder.Entity<VisionRecord>().HasData(
            // 招崧熙 - 与前端图片完全一致
            new { Id = 1L, StudentId = 6L, CheckDate = new DateTime(2026, 6, 28), LeftEye = "4.8", RightEye = "4.9", VisionLevel = "轻度近视", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 2L, StudentId = 6L, CheckDate = new DateTime(2026, 3, 15), LeftEye = "4.7", RightEye = "4.8", VisionLevel = "轻度近视", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 3L, StudentId = 6L, CheckDate = new DateTime(2025, 12, 1), LeftEye = "4.9", RightEye = "4.9", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            // 陈熙航
            new { Id = 4L, StudentId = 1L, CheckDate = new DateTime(2026, 6, 28), LeftEye = "4.5", RightEye = "4.6", VisionLevel = "中度近视", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 5L, StudentId = 1L, CheckDate = new DateTime(2026, 3, 15), LeftEye = "4.7", RightEye = "4.7", VisionLevel = "轻度近视", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 6L, StudentId = 1L, CheckDate = new DateTime(2025, 12, 1), LeftEye = "4.8", RightEye = "4.9", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            // 席振轩
            new { Id = 7L, StudentId = 2L, CheckDate = new DateTime(2026, 6, 28), LeftEye = "5.0", RightEye = "5.0", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 8L, StudentId = 2L, CheckDate = new DateTime(2026, 3, 15), LeftEye = "4.9", RightEye = "5.0", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 9L, StudentId = 2L, CheckDate = new DateTime(2025, 12, 1), LeftEye = "4.9", RightEye = "4.9", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            // 丁书婉
            new { Id = 10L, StudentId = 3L, CheckDate = new DateTime(2026, 6, 28), LeftEye = "4.9", RightEye = "4.8", VisionLevel = "轻度近视", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 11L, StudentId = 3L, CheckDate = new DateTime(2026, 3, 15), LeftEye = "4.9", RightEye = "4.9", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 12L, StudentId = 3L, CheckDate = new DateTime(2025, 12, 1), LeftEye = "5.0", RightEye = "5.0", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            // 周梓萌
            new { Id = 13L, StudentId = 4L, CheckDate = new DateTime(2026, 6, 28), LeftEye = "4.6", RightEye = "4.7", VisionLevel = "轻度近视", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 14L, StudentId = 4L, CheckDate = new DateTime(2026, 3, 15), LeftEye = "4.8", RightEye = "4.8", VisionLevel = "轻度近视", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 15L, StudentId = 4L, CheckDate = new DateTime(2025, 12, 1), LeftEye = "4.9", RightEye = "4.9", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            // 朱宇土
            new { Id = 16L, StudentId = 5L, CheckDate = new DateTime(2026, 6, 28), LeftEye = "5.0", RightEye = "4.9", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 17L, StudentId = 5L, CheckDate = new DateTime(2026, 3, 15), LeftEye = "4.9", RightEye = "4.8", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 18L, StudentId = 5L, CheckDate = new DateTime(2025, 12, 1), LeftEye = "4.9", RightEye = "4.8", VisionLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow }
        );

        // ---- oral_record (18 条) ----
        modelBuilder.Entity<OralRecord>().HasData(
            // 招崧熙 - 与前端图片一致
            new { Id = 1L, StudentId = 6L, CheckDate = new DateTime(2026, 6, 20), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 2L, StudentId = 6L, CheckDate = new DateTime(2026, 3, 10), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 3L, StudentId = 6L, CheckDate = new DateTime(2025, 11, 15), ToothStatus = "轻微龋齿", CavityCount = 1, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            // 其他学生
            new { Id = 4L, StudentId = 1L, CheckDate = new DateTime(2026, 6, 20), ToothStatus = "轻微龋齿", CavityCount = 2, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 5L, StudentId = 1L, CheckDate = new DateTime(2026, 3, 10), ToothStatus = "轻微龋齿", CavityCount = 1, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 6L, StudentId = 1L, CheckDate = new DateTime(2025, 11, 15), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 7L, StudentId = 2L, CheckDate = new DateTime(2026, 6, 20), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 8L, StudentId = 2L, CheckDate = new DateTime(2026, 3, 10), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 9L, StudentId = 2L, CheckDate = new DateTime(2025, 11, 15), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 10L, StudentId = 3L, CheckDate = new DateTime(2026, 6, 20), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 11L, StudentId = 3L, CheckDate = new DateTime(2026, 3, 10), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 12L, StudentId = 3L, CheckDate = new DateTime(2025, 11, 15), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 13L, StudentId = 4L, CheckDate = new DateTime(2026, 6, 20), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 14L, StudentId = 4L, CheckDate = new DateTime(2026, 3, 10), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 15L, StudentId = 4L, CheckDate = new DateTime(2025, 11, 15), ToothStatus = "轻微龋齿", CavityCount = 1, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 16L, StudentId = 5L, CheckDate = new DateTime(2026, 6, 20), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 17L, StudentId = 5L, CheckDate = new DateTime(2026, 3, 10), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 18L, StudentId = 5L, CheckDate = new DateTime(2025, 11, 15), ToothStatus = "良好", CavityCount = 0, CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow }
        );

        // ---- mental_record (18 条) ----
        modelBuilder.Entity<MentalRecord>().HasData(
            // 招崧熙 - 与前端图片一致
            new { Id = 1L, StudentId = 6L, CheckDate = new DateTime(2026, 6, 25), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 2L, StudentId = 6L, CheckDate = new DateTime(2026, 4, 20), StressLevel = "中度", SleepQuality = "一般", MoodStatus = "波动", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 3L, StudentId = 6L, CheckDate = new DateTime(2026, 2, 10), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            // 其他学生
            new { Id = 4L, StudentId = 1L, CheckDate = new DateTime(2026, 6, 25), StressLevel = "中度", SleepQuality = "一般", MoodStatus = "波动", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 5L, StudentId = 1L, CheckDate = new DateTime(2026, 4, 20), StressLevel = "中度", SleepQuality = "一般", MoodStatus = "波动", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 6L, StudentId = 1L, CheckDate = new DateTime(2026, 2, 10), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 7L, StudentId = 2L, CheckDate = new DateTime(2026, 6, 25), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 8L, StudentId = 2L, CheckDate = new DateTime(2026, 4, 20), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 9L, StudentId = 2L, CheckDate = new DateTime(2026, 2, 10), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 10L, StudentId = 3L, CheckDate = new DateTime(2026, 6, 25), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 11L, StudentId = 3L, CheckDate = new DateTime(2026, 4, 20), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 12L, StudentId = 3L, CheckDate = new DateTime(2026, 2, 10), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 13L, StudentId = 4L, CheckDate = new DateTime(2026, 6, 25), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 14L, StudentId = 4L, CheckDate = new DateTime(2026, 4, 20), StressLevel = "中度", SleepQuality = "一般", MoodStatus = "波动", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 15L, StudentId = 4L, CheckDate = new DateTime(2026, 2, 10), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 16L, StudentId = 5L, CheckDate = new DateTime(2026, 6, 25), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 17L, StudentId = 5L, CheckDate = new DateTime(2026, 4, 20), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 18L, StudentId = 5L, CheckDate = new DateTime(2026, 2, 10), StressLevel = "轻度", SleepQuality = "良好", MoodStatus = "稳定", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow }
        );

        // ---- weight_record (18 条) ----
        modelBuilder.Entity<WeightRecord>().HasData(
            // 招崧熙 - 与前端图片一致
            new { Id = 1L, StudentId = 6L, CheckDate = new DateTime(2026, 6, 28), Height = "165cm", Weight = "52kg", Bmi = "19.1", BmiLevel = "正常", WaistCircumference = "68cm", HipCircumference = "88cm", Whr = "0.77", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 2L, StudentId = 6L, CheckDate = new DateTime(2026, 3, 15), Height = "163cm", Weight = "50kg", Bmi = "18.8", BmiLevel = "正常", WaistCircumference = "66cm", HipCircumference = "86cm", Whr = "0.77", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 3L, StudentId = 6L, CheckDate = new DateTime(2025, 12, 1), Height = "160cm", Weight = "48kg", Bmi = "18.8", BmiLevel = "正常", WaistCircumference = "64cm", HipCircumference = "84cm", Whr = "0.76", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            // 其他学生
            new { Id = 4L, StudentId = 1L, CheckDate = new DateTime(2026, 6, 28), Height = "162cm", Weight = "55kg", Bmi = "21.0", BmiLevel = "正常", WaistCircumference = "72cm", HipCircumference = "90cm", Whr = "0.80", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 5L, StudentId = 1L, CheckDate = new DateTime(2026, 3, 15), Height = "160cm", Weight = "53kg", Bmi = "20.7", BmiLevel = "正常", WaistCircumference = "70cm", HipCircumference = "88cm", Whr = "0.80", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 6L, StudentId = 1L, CheckDate = new DateTime(2025, 12, 1), Height = "158cm", Weight = "50kg", Bmi = "20.0", BmiLevel = "正常", WaistCircumference = "68cm", HipCircumference = "86cm", Whr = "0.79", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 7L, StudentId = 2L, CheckDate = new DateTime(2026, 6, 28), Height = "160cm", Weight = "48kg", Bmi = "18.8", BmiLevel = "正常", WaistCircumference = "65cm", HipCircumference = "85cm", Whr = "0.76", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 8L, StudentId = 2L, CheckDate = new DateTime(2026, 3, 15), Height = "158cm", Weight = "47kg", Bmi = "18.8", BmiLevel = "正常", WaistCircumference = "64cm", HipCircumference = "84cm", Whr = "0.76", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 9L, StudentId = 2L, CheckDate = new DateTime(2025, 12, 1), Height = "156cm", Weight = "45kg", Bmi = "18.5", BmiLevel = "正常", WaistCircumference = "63cm", HipCircumference = "83cm", Whr = "0.76", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 10L, StudentId = 3L, CheckDate = new DateTime(2026, 6, 28), Height = "158cm", Weight = "45kg", Bmi = "18.0", BmiLevel = "正常", WaistCircumference = "62cm", HipCircumference = "84cm", Whr = "0.74", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 11L, StudentId = 3L, CheckDate = new DateTime(2026, 3, 15), Height = "156cm", Weight = "44kg", Bmi = "18.1", BmiLevel = "正常", WaistCircumference = "61cm", HipCircumference = "83cm", Whr = "0.73", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 12L, StudentId = 3L, CheckDate = new DateTime(2025, 12, 1), Height = "154cm", Weight = "42kg", Bmi = "17.7", BmiLevel = "偏瘦", WaistCircumference = "60cm", HipCircumference = "82cm", Whr = "0.73", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 13L, StudentId = 4L, CheckDate = new DateTime(2026, 6, 28), Height = "155cm", Weight = "46kg", Bmi = "19.2", BmiLevel = "正常", WaistCircumference = "63cm", HipCircumference = "85cm", Whr = "0.74", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 14L, StudentId = 4L, CheckDate = new DateTime(2026, 3, 15), Height = "153cm", Weight = "45kg", Bmi = "19.2", BmiLevel = "正常", WaistCircumference = "62cm", HipCircumference = "84cm", Whr = "0.74", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 15L, StudentId = 4L, CheckDate = new DateTime(2025, 12, 1), Height = "151cm", Weight = "43kg", Bmi = "18.9", BmiLevel = "正常", WaistCircumference = "61cm", HipCircumference = "83cm", Whr = "0.73", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 16L, StudentId = 5L, CheckDate = new DateTime(2026, 6, 28), Height = "163cm", Weight = "54kg", Bmi = "20.3", BmiLevel = "正常", WaistCircumference = "70cm", HipCircumference = "89cm", Whr = "0.79", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 17L, StudentId = 5L, CheckDate = new DateTime(2026, 3, 15), Height = "161cm", Weight = "52kg", Bmi = "20.1", BmiLevel = "正常", WaistCircumference = "68cm", HipCircumference = "88cm", Whr = "0.77", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 18L, StudentId = 5L, CheckDate = new DateTime(2025, 12, 1), Height = "159cm", Weight = "50kg", Bmi = "19.8", BmiLevel = "正常", WaistCircumference = "67cm", HipCircumference = "87cm", Whr = "0.77", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow }
        );

        // ---- bone_record (18 条) ----
        modelBuilder.Entity<BoneRecord>().HasData(
            // 招崧熙 - 与前端图片一致
            new { Id = 1L, StudentId = 6L, CheckDate = new DateTime(2026, 6, 20), BoneDensity = "正常", BoneAge = "12岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 2L, StudentId = 6L, CheckDate = new DateTime(2026, 3, 10), BoneDensity = "正常", BoneAge = "11.5岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 3L, StudentId = 6L, CheckDate = new DateTime(2025, 11, 15), BoneDensity = "正常", BoneAge = "11岁", VitaminD = "良好", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            // 其他学生
            new { Id = 4L, StudentId = 1L, CheckDate = new DateTime(2026, 6, 20), BoneDensity = "正常", BoneAge = "12.5岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 5L, StudentId = 1L, CheckDate = new DateTime(2026, 3, 10), BoneDensity = "正常", BoneAge = "12岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 6L, StudentId = 1L, CheckDate = new DateTime(2025, 11, 15), BoneDensity = "正常", BoneAge = "11.5岁", VitaminD = "良好", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 7L, StudentId = 2L, CheckDate = new DateTime(2026, 6, 20), BoneDensity = "正常", BoneAge = "12岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 8L, StudentId = 2L, CheckDate = new DateTime(2026, 3, 10), BoneDensity = "正常", BoneAge = "11.5岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 9L, StudentId = 2L, CheckDate = new DateTime(2025, 11, 15), BoneDensity = "正常", BoneAge = "11岁", VitaminD = "良好", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 10L, StudentId = 3L, CheckDate = new DateTime(2026, 6, 20), BoneDensity = "正常", BoneAge = "11.5岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 11L, StudentId = 3L, CheckDate = new DateTime(2026, 3, 10), BoneDensity = "正常", BoneAge = "11岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 12L, StudentId = 3L, CheckDate = new DateTime(2025, 11, 15), BoneDensity = "正常", BoneAge = "10.5岁", VitaminD = "良好", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 13L, StudentId = 4L, CheckDate = new DateTime(2026, 6, 20), BoneDensity = "正常", BoneAge = "11岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 14L, StudentId = 4L, CheckDate = new DateTime(2026, 3, 10), BoneDensity = "正常", BoneAge = "10.5岁", VitaminD = "良好", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 15L, StudentId = 4L, CheckDate = new DateTime(2025, 11, 15), BoneDensity = "正常", BoneAge = "10岁", VitaminD = "良好", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 16L, StudentId = 5L, CheckDate = new DateTime(2026, 6, 20), BoneDensity = "正常", BoneAge = "12.5岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 17L, StudentId = 5L, CheckDate = new DateTime(2026, 3, 10), BoneDensity = "正常", BoneAge = "12岁", VitaminD = "充足", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow },
            new { Id = 18L, StudentId = 5L, CheckDate = new DateTime(2025, 11, 15), BoneDensity = "正常", BoneAge = "11.5岁", VitaminD = "良好", CalciumLevel = "正常", CreatedAt = (DateTime?)seedNow, UpdatedAt = (DateTime?)seedNow }
        );
    }
}

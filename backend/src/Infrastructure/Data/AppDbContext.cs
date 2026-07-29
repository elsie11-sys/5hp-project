using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext, IApplicationDbContext
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
    }
}

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
}

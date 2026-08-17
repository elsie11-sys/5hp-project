using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoleDataScopeDict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "sys_dict",
                columns: new[] { "Id", "CreatedAt", "DictName", "DictType", "OwnerType", "Remark", "Status", "UpdatedAt" },
                values: new object[] { 100L, new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "角色数据权限", "role_based_data_permissions", "SYSTEM", "角色管理中可分配的数据权限范围（全部/自定义/本部门/本部门及以下/仅本人）", 1, null });

            migrationBuilder.InsertData(
                table: "sys_dict_item",
                columns: new[] { "DictType", "ItemValue", "CreatedAt", "ItemLabel", "Remark", "SortOrder", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { "role_based_data_permissions", "ALL", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "全部数据权限", "可访问所有数据", 1, 1, null },
                    { "role_based_data_permissions", "CUSTOM", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "自定义数据权限", "按指定部门范围访问", 2, 1, null },
                    { "role_based_data_permissions", "DEPT", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "本部门数据权限", "仅访问本部门数据", 3, 1, null },
                    { "role_based_data_permissions", "DEPT_AND_BELOW", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "本部门及以下数据权限", "可访问本部门及下级部门数据", 4, 1, null },
                    { "role_based_data_permissions", "SELF", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "仅本人数据权限", "仅能访问自己的数据", 5, 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "role_based_data_permissions", "ALL" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "role_based_data_permissions", "CUSTOM" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "role_based_data_permissions", "DEPT" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "role_based_data_permissions", "DEPT_AND_BELOW" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "role_based_data_permissions", "SELF" });

            migrationBuilder.DeleteData(
                table: "sys_dict",
                keyColumn: "Id",
                keyValue: 100L);
        }
    }
}

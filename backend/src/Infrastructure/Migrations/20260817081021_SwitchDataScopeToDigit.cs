using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SwitchDataScopeToDigit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "sys_dict_item",
                columns: new[] { "DictType", "ItemValue", "CreatedAt", "ItemLabel", "Remark", "SortOrder", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { "role_based_data_permissions", "1", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "全部数据权限", "可访问所有数据", 1, 1, null },
                    { "role_based_data_permissions", "2", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "自定义数据权限", "按指定部门范围访问", 2, 1, null },
                    { "role_based_data_permissions", "3", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "本部门数据权限", "仅访问本部门数据", 3, 1, null },
                    { "role_based_data_permissions", "4", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "本部门及以下数据权限", "可访问本部门及下级部门数据", 4, 1, null },
                    { "role_based_data_permissions", "5", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Local), "仅本人数据权限", "仅能访问自己的数据", 5, 1, null }
                });

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 1L,
                column: "data_scope",
                value: "1");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 2L,
                column: "data_scope",
                value: "4");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 3L,
                column: "data_scope",
                value: "4");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 4L,
                column: "data_scope",
                value: "4");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 5L,
                column: "data_scope",
                value: "3");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 6L,
                column: "data_scope",
                value: "3");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 7L,
                column: "data_scope",
                value: "5");

            // 转换 sys_role 表里所有非 seed 行的旧英文 data_scope（用户手动加的角色可能存的是 ALL/CUSTOM/DEPT/DEPT_AND_BELOW/SELF）
            // 任何不在 '1'~'5' 白名单的值（含 NULL）都转为 '1'（全部数据权限）
            migrationBuilder.Sql(@"
                UPDATE sys_role
                SET data_scope = CASE UPPER(TRIM(COALESCE(data_scope, '')))
                    WHEN 'ALL'             THEN '1'
                    WHEN 'CUSTOM'          THEN '2'
                    WHEN 'DEPT'            THEN '3'
                    WHEN 'DEPT_AND_BELOW'  THEN '4'
                    WHEN 'SELF'            THEN '5'
                    ELSE '1'
                END
                WHERE data_scope IS NULL
                   OR UPPER(TRIM(data_scope)) NOT IN ('1','2','3','4','5');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "role_based_data_permissions", "1" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "role_based_data_permissions", "2" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "role_based_data_permissions", "3" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "role_based_data_permissions", "4" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "role_based_data_permissions", "5" });

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

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 1L,
                column: "data_scope",
                value: "ALL");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 2L,
                column: "data_scope",
                value: "DEPT_AND_BELOW");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 3L,
                column: "data_scope",
                value: "DEPT_AND_BELOW");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 4L,
                column: "data_scope",
                value: "DEPT_AND_BELOW");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 5L,
                column: "data_scope",
                value: "DEPT");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 6L,
                column: "data_scope",
                value: "DEPT");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 7L,
                column: "data_scope",
                value: "SELF");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDataScopeToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "data_scope",
                table: "sys_role",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "ALL");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_scope",
                table: "sys_role");
        }
    }
}

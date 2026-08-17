using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sys_role_role_code",
                table: "sys_role");

            migrationBuilder.DropColumn(
                name: "role_code",
                table: "sys_role");

            migrationBuilder.AddColumn<string>(
                name: "permission",
                table: "sys_role",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 1L,
                column: "permission",
                value: "system:national");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 2L,
                column: "permission",
                value: "system:province");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 3L,
                column: "permission",
                value: "system:city");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 4L,
                column: "permission",
                value: "system:district");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 5L,
                column: "permission",
                value: "system:school");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 6L,
                column: "permission",
                value: "system:doctor");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 7L,
                column: "permission",
                value: "system:teacher");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "permission",
                table: "sys_role");

            migrationBuilder.AddColumn<string>(
                name: "role_code",
                table: "sys_role",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 1L,
                column: "role_code",
                value: "R001");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 2L,
                column: "role_code",
                value: "R002");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 3L,
                column: "role_code",
                value: "R003");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 4L,
                column: "role_code",
                value: "R004");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 5L,
                column: "role_code",
                value: "R005");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 6L,
                column: "role_code",
                value: "R006");

            migrationBuilder.UpdateData(
                table: "sys_role",
                keyColumn: "id",
                keyValue: 7L,
                column: "role_code",
                value: "R007");

            migrationBuilder.CreateIndex(
                name: "IX_sys_role_role_code",
                table: "sys_role",
                column: "role_code",
                unique: true,
                filter: "role_code IS NOT NULL");
        }
    }
}

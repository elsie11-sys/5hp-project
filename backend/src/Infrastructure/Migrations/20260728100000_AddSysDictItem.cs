using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [Migration("20260728100000_AddSysDictItem")]
    public partial class AddSysDictItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerType",
                table: "sys_dict",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "USER");

            migrationBuilder.CreateTable(
                name: "sys_dict_item",
                columns: table => new
                {
                    DictType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ItemValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ItemLabel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                   // ItemCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Remark = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_dict_item", x => new { x.DictType, x.ItemValue });
                    table.ForeignKey(
                        name: "FK_sys_dict_item_sys_dict_DictType",
                        column: x => x.DictType,
                        principalTable: "sys_dict",
                        principalColumn: "DictType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sys_dict_item_DictType",
                table: "sys_dict_item",
                column: "DictType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_dict_item");

            migrationBuilder.DropColumn(
                name: "OwnerType",
                table: "sys_dict");
        }
    }
}

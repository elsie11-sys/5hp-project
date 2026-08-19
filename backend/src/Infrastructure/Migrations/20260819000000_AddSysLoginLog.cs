using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <summary>
    /// 新增 sys_login_log 表（系统登录日志）
    /// <para>
    /// 每条记录代表一次"用户尝试登录系统"的行为，包含用户名、IP、归属地、操作系统、浏览器、登录状态、描述、登录时间。
    /// 状态约定：1=成功 0=失败。
    /// </para>
    /// </summary>
    public partial class AddSysLoginLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_login_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    os = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    browser = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    login_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_login_log", x => x.id);
                });

            // 常用查询索引
            migrationBuilder.CreateIndex(
                name: "IX_sys_login_log_user_name",
                table: "sys_login_log",
                column: "user_name");

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_log_ip",
                table: "sys_login_log",
                column: "ip");

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_log_status",
                table: "sys_login_log",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_sys_login_log_login_time",
                table: "sys_login_log",
                column: "login_time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_login_log");
        }
    }
}

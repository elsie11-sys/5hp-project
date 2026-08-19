using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <summary>
    /// 新增 sys_operation_log 表（系统操作日志）
    /// <para>
    /// 每条记录代表一次"前端 → 后端"的 HTTP 调用，包含操作人、模块、类型、状态、耗时、请求/响应内容、客户端 IP 等。
    /// 状态约定：1=成功 0=失败。
    /// </para>
    /// </summary>
    public partial class AddSysOperationLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_operation_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    @operator = table.Column<string>(name: "operator", type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    cost_ms = table.Column<int>(type: "integer", nullable: true),
                    request_url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    request_method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    method = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    request_params = table.Column<string>(type: "text", nullable: true),
                    response_params = table.Column<string>(type: "text", nullable: true),
                    error_msg = table.Column<string>(type: "text", nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_operation_log", x => x.id);
                });

            // 常用查询索引
            migrationBuilder.CreateIndex(
                name: "IX_sys_operation_log_operator",
                table: "sys_operation_log",
                column: "operator");

            migrationBuilder.CreateIndex(
                name: "IX_sys_operation_log_type",
                table: "sys_operation_log",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_sys_operation_log_status",
                table: "sys_operation_log",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_sys_operation_log_created_at",
                table: "sys_operation_log",
                column: "created_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_operation_log");
        }
    }
}

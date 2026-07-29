using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSysDict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_dict",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DictName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DictType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Remark = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_dict", x => x.Id);
                });

            // 唯一索引：DictType 业务唯一
            migrationBuilder.CreateIndex(
                name: "IX_sys_dict_DictType",
                table: "sys_dict",
                column: "DictType",
                unique: true);

            // 种子数据：10 条常用字典（来自 RuoYi 经典字典）
            migrationBuilder.Sql(@"
                INSERT INTO sys_dict (""DictName"", ""DictType"", ""Status"", ""Remark"", ""CreatedAt"") VALUES
                ('用户性别', 'sys_user_sex',         1, '用户性别列表',     now()),
                ('菜单状态', 'sys_show_hide',        1, '菜单状态列表',     now()),
                ('系统开关', 'sys_normal_disable',  1, '系统开关列表',     now()),
                ('任务状态', 'sys_job_status',      1, '任务状态列表',     now()),
                ('任务分组', 'sys_job_group',       1, '任务分组列表',     now()),
                ('系统是否', 'sys_yes_no',          1, '系统是否列表',     now()),
                ('通知类型', 'sys_notice_type',     1, '通知类型列表',     now()),
                ('通知状态', 'sys_notice_status',   1, '通知状态列表',     now()),
                ('操作类型', 'sys_oper_type',       1, '操作类型列表',     now()),
                ('系统状态', 'sys_common_status',   1, '登录状态列表',     now());
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "sys_dict");
        }
    }
}

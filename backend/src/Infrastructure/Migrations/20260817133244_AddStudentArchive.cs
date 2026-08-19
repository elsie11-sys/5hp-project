using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "student_archive",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    avatar = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    grade_year = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    school = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    class_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    birthday = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    id_card = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    native_place = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    parent_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    parent_contact = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    allergy_history = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    father_myopia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    mother_myopia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    data_source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    intervention = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_archive", x => x.id);
                });

            // 种子数据：把前端 mock 的 6 条学生档案灌进来
            migrationBuilder.InsertData(
                table: "student_archive",
                columns: new[] { "id", "address", "allergy_history", "avatar", "birthday", "class_name", "created_at", "data_source", "father_myopia", "gender", "grade_year", "id_card", "intervention", "mother_myopia", "name", "nation", "native_place", "parent_contact", "parent_phone", "school", "student_no", "updated_at" },
                values: new object[] { 1L, "江苏淮安淮安区淮安区新城广场22栋", "未知", "👦", new DateTime(2011, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "七年级五班", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "sjwjuser1", "未知", "男", "2022级", "320829201108090012", true, "未知", "陈熙航", "汉族", "江苏淮安", "chenxh_2022", "15952305677", "淮安外国语学校", "2022001", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.InsertData(
                table: "student_archive",
                columns: new[] { "id", "address", "allergy_history", "avatar", "birthday", "class_name", "created_at", "data_source", "father_myopia", "gender", "grade_year", "id_card", "mother_myopia", "name", "nation", "native_place", "parent_contact", "parent_phone", "school", "student_no", "updated_at" },
                values: new object[,]
                {
                    { 2L, "江苏淮安淮安区小区8栋301", "未知", "👦", new DateTime(2011, 8, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "七年级五班", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "sjwjuser1", "未知", "男", "2022级", "320829201108090012", "未知", "席振轩", "汉族", "江苏淮安", "xizhenxuan", "15949198257", "淮安外国语学校", "2022002", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 3L, "江苏淮安淮安区花苑3栋502", "未知", "👧", new DateTime(2011, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "七年级五班", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "sjwjuser1", "未知", "女", "2022级", "320831201102121422", "未知", "丁书婉", "汉族", "江苏淮安", "dingshuwan", "13952371506", "淮安外国语学校", "2022003", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) }
                });

            migrationBuilder.InsertData(
                table: "student_archive",
                columns: new[] { "id", "address", "allergy_history", "avatar", "birthday", "class_name", "created_at", "data_source", "father_myopia", "gender", "grade_year", "id_card", "intervention", "mother_myopia", "name", "nation", "native_place", "parent_contact", "parent_phone", "school", "student_no", "updated_at" },
                values: new object[] { 4L, "江苏淮安淮安区小区1幢2单元403", "未知", "👧", new DateTime(2011, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "七年级五班", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "sjwjuser1", "未知", "女", "2022级", "320803201103140048", true, "未知", "周梓萌", "汉族", "江苏淮安", "zhouzimeng", "13952390315", "淮安外国语学校", "2022004", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.InsertData(
                table: "student_archive",
                columns: new[] { "id", "address", "allergy_history", "avatar", "birthday", "class_name", "created_at", "data_source", "father_myopia", "gender", "grade_year", "id_card", "mother_myopia", "name", "nation", "native_place", "parent_contact", "parent_phone", "school", "student_no", "updated_at" },
                values: new object[,]
                {
                    { 5L, "江苏淮安淮安区小区9栋304", "未知", "👦", new DateTime(2011, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "七年级五班", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "sjwjuser1", "未知", "男", "2022级", "320831201105021422", "未知", "朱宇土", "汉族", "江苏淮安", "zhuyutu", "15152566388", "淮安外国语学校", "2022005", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 6L, "江苏淮安淮安区小区2栋1001", "未知", "👦", new DateTime(2010, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "七年级五班", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "sjwjuser1", "未知", "男", "2022级", "320831201011232637", "未知", "招崧熙", "汉族", "江苏淮安", "zhaosongxi", "13805285721", "淮安外国语学校", "2022006", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_archive_class_name",
                table: "student_archive",
                column: "class_name");

            migrationBuilder.CreateIndex(
                name: "IX_student_archive_created_at",
                table: "student_archive",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_student_archive_intervention",
                table: "student_archive",
                column: "intervention");

            migrationBuilder.CreateIndex(
                name: "IX_student_archive_name",
                table: "student_archive",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_student_archive_school",
                table: "student_archive",
                column: "school");

            migrationBuilder.CreateIndex(
                name: "IX_student_archive_student_no",
                table: "student_archive",
                column: "student_no",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_archive");
        }
    }
}

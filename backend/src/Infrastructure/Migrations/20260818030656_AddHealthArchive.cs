using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bone_record",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    check_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    bone_density = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    bone_age = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    vitamin_d = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    calcium_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bone_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mental_record",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    check_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    stress_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    sleep_quality = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    mood_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mental_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "oral_record",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    check_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    tooth_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cavity_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oral_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vision_record",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    check_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    left_eye = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    right_eye = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    vision_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vision_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "weight_record",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    check_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    height = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    weight = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    bmi = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    bmi_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    waist_circumference = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    hip_circumference = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    whr = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weight_record", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "bone_record",
                columns: new[] { "id", "bone_age", "bone_density", "calcium_level", "check_date", "created_at", "student_id", "updated_at", "vitamin_d" },
                values: new object[,]
                {
                    { 1L, "12岁", "正常", "正常", new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 2L, "11.5岁", "正常", "正常", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 3L, "11岁", "正常", "正常", new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "良好" },
                    { 4L, "12.5岁", "正常", "正常", new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 5L, "12岁", "正常", "正常", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 6L, "11.5岁", "正常", "正常", new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "良好" },
                    { 7L, "12岁", "正常", "正常", new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 8L, "11.5岁", "正常", "正常", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 9L, "11岁", "正常", "正常", new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "良好" },
                    { 10L, "11.5岁", "正常", "正常", new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 11L, "11岁", "正常", "正常", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 12L, "10.5岁", "正常", "正常", new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "良好" },
                    { 13L, "11岁", "正常", "正常", new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 14L, "10.5岁", "正常", "正常", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "良好" },
                    { 15L, "10岁", "正常", "正常", new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "良好" },
                    { 16L, "12.5岁", "正常", "正常", new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 17L, "12岁", "正常", "正常", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "充足" },
                    { 18L, "11.5岁", "正常", "正常", new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "良好" }
                });

            migrationBuilder.InsertData(
                table: "mental_record",
                columns: new[] { "id", "check_date", "created_at", "mood_status", "sleep_quality", "stress_level", "student_id", "updated_at" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 2L, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "波动", "一般", "中度", 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 3L, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 4L, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "波动", "一般", "中度", 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 5L, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "波动", "一般", "中度", 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 6L, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 7L, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 8L, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 9L, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 10L, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 11L, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 12L, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 13L, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 14L, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "波动", "一般", "中度", 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 15L, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 16L, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 17L, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 18L, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "稳定", "良好", "轻度", 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) }
                });

            migrationBuilder.InsertData(
                table: "oral_record",
                columns: new[] { "id", "cavity_count", "check_date", "created_at", "student_id", "tooth_status", "updated_at" },
                values: new object[,]
                {
                    { 1L, 0, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 6L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 2L, 0, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 6L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 3L, 1, new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 6L, "轻微龋齿", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 4L, 2, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 1L, "轻微龋齿", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 5L, 1, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 1L, "轻微龋齿", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 6L, 0, new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 1L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 7L, 0, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 2L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 8L, 0, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 2L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 9L, 0, new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 2L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 10L, 0, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 3L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 11L, 0, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 3L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 12L, 0, new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 3L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 13L, 0, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 4L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 14L, 0, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 4L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 15L, 1, new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 4L, "轻微龋齿", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 16L, 0, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 5L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 17L, 0, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 5L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) },
                    { 18L, 0, new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), 5L, "良好", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local) }
                });

            migrationBuilder.InsertData(
                table: "vision_record",
                columns: new[] { "id", "check_date", "created_at", "left_eye", "right_eye", "student_id", "updated_at", "vision_level" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.8", "4.9", 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "轻度近视" },
                    { 2L, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.7", "4.8", 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "轻度近视" },
                    { 3L, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.9", "4.9", 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" },
                    { 4L, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.5", "4.6", 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "中度近视" },
                    { 5L, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.7", "4.7", 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "轻度近视" },
                    { 6L, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.8", "4.9", 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" },
                    { 7L, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "5.0", "5.0", 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" },
                    { 8L, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.9", "5.0", 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" },
                    { 9L, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.9", "4.9", 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" },
                    { 10L, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.9", "4.8", 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "轻度近视" },
                    { 11L, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.9", "4.9", 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" },
                    { 12L, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "5.0", "5.0", 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" },
                    { 13L, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.6", "4.7", 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "轻度近视" },
                    { 14L, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.8", "4.8", 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "轻度近视" },
                    { 15L, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.9", "4.9", 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" },
                    { 16L, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "5.0", "4.9", 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" },
                    { 17L, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.9", "4.8", 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" },
                    { 18L, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "4.9", "4.8", 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "正常" }
                });

            migrationBuilder.InsertData(
                table: "weight_record",
                columns: new[] { "id", "bmi", "bmi_level", "check_date", "created_at", "height", "hip_circumference", "student_id", "updated_at", "waist_circumference", "weight", "whr" },
                values: new object[,]
                {
                    { 1L, "19.1", "正常", new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "165cm", "88cm", 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "68cm", "52kg", "0.77" },
                    { 2L, "18.8", "正常", new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "163cm", "86cm", 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "66cm", "50kg", "0.77" },
                    { 3L, "18.8", "正常", new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "160cm", "84cm", 6L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "64cm", "48kg", "0.76" },
                    { 4L, "21.0", "正常", new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "162cm", "90cm", 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "72cm", "55kg", "0.80" },
                    { 5L, "20.7", "正常", new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "160cm", "88cm", 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "70cm", "53kg", "0.80" },
                    { 6L, "20.0", "正常", new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "158cm", "86cm", 1L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "68cm", "50kg", "0.79" },
                    { 7L, "18.8", "正常", new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "160cm", "85cm", 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "65cm", "48kg", "0.76" },
                    { 8L, "18.8", "正常", new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "158cm", "84cm", 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "64cm", "47kg", "0.76" },
                    { 9L, "18.5", "正常", new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "156cm", "83cm", 2L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "63cm", "45kg", "0.76" },
                    { 10L, "18.0", "正常", new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "158cm", "84cm", 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "62cm", "45kg", "0.74" },
                    { 11L, "18.1", "正常", new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "156cm", "83cm", 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "61cm", "44kg", "0.73" },
                    { 12L, "17.7", "偏瘦", new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "154cm", "82cm", 3L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "60cm", "42kg", "0.73" },
                    { 13L, "19.2", "正常", new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "155cm", "85cm", 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "63cm", "46kg", "0.74" },
                    { 14L, "19.2", "正常", new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "153cm", "84cm", 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "62cm", "45kg", "0.74" },
                    { 15L, "18.9", "正常", new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "151cm", "83cm", 4L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "61cm", "43kg", "0.73" },
                    { 16L, "20.3", "正常", new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "163cm", "89cm", 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "70cm", "54kg", "0.79" },
                    { 17L, "20.1", "正常", new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "161cm", "88cm", 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "68cm", "52kg", "0.77" },
                    { 18L, "19.8", "正常", new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "159cm", "87cm", 5L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "67cm", "50kg", "0.77" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_bone_record_student_id_check_date",
                table: "bone_record",
                columns: new[] { "student_id", "check_date" });

            migrationBuilder.CreateIndex(
                name: "IX_mental_record_student_id_check_date",
                table: "mental_record",
                columns: new[] { "student_id", "check_date" });

            migrationBuilder.CreateIndex(
                name: "IX_oral_record_student_id_check_date",
                table: "oral_record",
                columns: new[] { "student_id", "check_date" });

            migrationBuilder.CreateIndex(
                name: "IX_vision_record_student_id_check_date",
                table: "vision_record",
                columns: new[] { "student_id", "check_date" });

            migrationBuilder.CreateIndex(
                name: "IX_weight_record_student_id_check_date",
                table: "weight_record",
                columns: new[] { "student_id", "check_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bone_record");

            migrationBuilder.DropTable(
                name: "mental_record");

            migrationBuilder.DropTable(
                name: "oral_record");

            migrationBuilder.DropTable(
                name: "vision_record");

            migrationBuilder.DropTable(
                name: "weight_record");
        }
    }
}

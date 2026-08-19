using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnrichStudentFieldsAndDicts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "current_city",
                table: "student_archive",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "current_district",
                table: "student_archive",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "current_province",
                table: "student_archive",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "current_street",
                table: "student_archive",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "native_city",
                table: "student_archive",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "native_province",
                table: "student_archive",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "remark",
                table: "student_archive",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "student_archive",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "current_city", "current_district", "current_province", "current_street", "native_city", "native_province", "remark" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "student_archive",
                keyColumn: "id",
                keyValue: 2L,
                columns: new[] { "current_city", "current_district", "current_province", "current_street", "native_city", "native_province", "remark" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "student_archive",
                keyColumn: "id",
                keyValue: 3L,
                columns: new[] { "current_city", "current_district", "current_province", "current_street", "native_city", "native_province", "remark" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "student_archive",
                keyColumn: "id",
                keyValue: 4L,
                columns: new[] { "current_city", "current_district", "current_province", "current_street", "native_city", "native_province", "remark" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "student_archive",
                keyColumn: "id",
                keyValue: 5L,
                columns: new[] { "current_city", "current_district", "current_province", "current_street", "native_city", "native_province", "remark" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "student_archive",
                keyColumn: "id",
                keyValue: 6L,
                columns: new[] { "current_city", "current_district", "current_province", "current_street", "native_city", "native_province", "remark" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.InsertData(
                table: "sys_dict",
                columns: new[] { "Id", "CreatedAt", "DictName", "DictType", "OwnerType", "Remark", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 200L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "学生性别", "sys_gender", "SYSTEM", "学生档案-性别下拉", 1, null },
                    { 201L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "民族", "sys_nation", "SYSTEM", "学生档案-民族下拉", 1, null },
                    { 202L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "入学年级", "sys_grade_year", "SYSTEM", "学生档案-级/届下拉", 1, null },
                    { 203L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "学校", "sys_school", "SYSTEM", "学生档案-学校下拉", 1, null },
                    { 204L, new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "父母近视", "sys_myopia", "SYSTEM", "学生档案-父母近视下拉", 1, null }
                });

            migrationBuilder.InsertData(
                table: "sys_dict_item",
                columns: new[] { "DictType", "ItemValue", "CreatedAt", "ItemLabel", "Remark", "SortOrder", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { "sys_gender", "男", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "男", null, 1, 1, null },
                    { "sys_gender", "女", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "女", null, 2, 1, null },
                    { "sys_grade_year", "2018级", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "2018级", null, 1, 1, null },
                    { "sys_grade_year", "2019级", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "2019级", null, 2, 1, null },
                    { "sys_grade_year", "2020级", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "2020级", null, 3, 1, null },
                    { "sys_grade_year", "2021级", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "2021级", null, 4, 1, null },
                    { "sys_grade_year", "2022级", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "2022级", null, 5, 1, null },
                    { "sys_grade_year", "2023级", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "2023级", null, 6, 1, null },
                    { "sys_grade_year", "2024级", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "2024级", null, 7, 1, null },
                    { "sys_grade_year", "2025级", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "2025级", null, 8, 1, null },
                    { "sys_grade_year", "2026级", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "2026级", null, 9, 1, null },
                    { "sys_myopia", "否", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "否", null, 1, 1, null },
                    { "sys_myopia", "是", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "是", null, 2, 1, null },
                    { "sys_myopia", "未知", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "未知", null, 3, 1, null },
                    { "sys_nation", "阿昌族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "阿昌族", null, 39, 1, null },
                    { "sys_nation", "白族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "白族", null, 14, 1, null },
                    { "sys_nation", "保安族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "保安族", null, 47, 1, null },
                    { "sys_nation", "布朗族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "布朗族", null, 34, 1, null },
                    { "sys_nation", "布依族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "布依族", null, 9, 1, null },
                    { "sys_nation", "藏族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "藏族", null, 4, 1, null },
                    { "sys_nation", "朝鲜族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "朝鲜族", null, 10, 1, null },
                    { "sys_nation", "达斡尔族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "达斡尔族", null, 31, 1, null },
                    { "sys_nation", "傣族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "傣族", null, 18, 1, null },
                    { "sys_nation", "德昂族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "德昂族", null, 46, 1, null },
                    { "sys_nation", "东乡族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "东乡族", null, 26, 1, null },
                    { "sys_nation", "侗族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "侗族", null, 12, 1, null },
                    { "sys_nation", "独龙族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "独龙族", null, 51, 1, null },
                    { "sys_nation", "俄罗斯族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "俄罗斯族", null, 44, 1, null },
                    { "sys_nation", "鄂伦春族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "鄂伦春族", null, 52, 1, null },
                    { "sys_nation", "鄂温克族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "鄂温克族", null, 45, 1, null },
                    { "sys_nation", "高山族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "高山族", null, 23, 1, null },
                    { "sys_nation", "仡佬族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "仡佬族", null, 37, 1, null },
                    { "sys_nation", "哈尼族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "哈尼族", null, 16, 1, null },
                    { "sys_nation", "哈萨克族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "哈萨克族", null, 17, 1, null },
                    { "sys_nation", "汉族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "汉族", null, 1, 1, null },
                    { "sys_nation", "赫哲族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "赫哲族", null, 53, 1, null },
                    { "sys_nation", "回族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "回族", null, 3, 1, null },
                    { "sys_nation", "基诺族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "基诺族", null, 56, 1, null },
                    { "sys_nation", "京族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "京族", null, 49, 1, null },
                    { "sys_nation", "景颇族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "景颇族", null, 28, 1, null },
                    { "sys_nation", "柯尔克孜族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "柯尔克孜族", null, 29, 1, null },
                    { "sys_nation", "拉祜族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "拉祜族", null, 24, 1, null },
                    { "sys_nation", "黎族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "黎族", null, 19, 1, null },
                    { "sys_nation", "傈僳族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "傈僳族", null, 20, 1, null },
                    { "sys_nation", "珞巴族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "珞巴族", null, 55, 1, null },
                    { "sys_nation", "满族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "满族", null, 11, 1, null },
                    { "sys_nation", "毛南族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "毛南族", null, 36, 1, null },
                    { "sys_nation", "门巴族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "门巴族", null, 54, 1, null },
                    { "sys_nation", "蒙古族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "蒙古族", null, 2, 1, null },
                    { "sys_nation", "苗族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "苗族", null, 6, 1, null },
                    { "sys_nation", "仫佬族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "仫佬族", null, 32, 1, null },
                    { "sys_nation", "纳西族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "纳西族", null, 27, 1, null },
                    { "sys_nation", "怒族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "怒族", null, 42, 1, null },
                    { "sys_nation", "普米族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "普米族", null, 40, 1, null },
                    { "sys_nation", "其他", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "其他", null, 57, 1, null },
                    { "sys_nation", "羌族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "羌族", null, 33, 1, null },
                    { "sys_nation", "撒拉族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "撒拉族", null, 35, 1, null },
                    { "sys_nation", "畲族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "畲族", null, 22, 1, null },
                    { "sys_nation", "水族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "水族", null, 25, 1, null },
                    { "sys_nation", "塔吉克族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "塔吉克族", null, 41, 1, null },
                    { "sys_nation", "塔塔尔族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "塔塔尔族", null, 50, 1, null },
                    { "sys_nation", "土家族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "土家族", null, 15, 1, null },
                    { "sys_nation", "土族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "土族", null, 30, 1, null },
                    { "sys_nation", "佤族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "佤族", null, 21, 1, null },
                    { "sys_nation", "维吾尔族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "维吾尔族", null, 5, 1, null },
                    { "sys_nation", "乌孜别克族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "乌孜别克族", null, 43, 1, null },
                    { "sys_nation", "锡伯族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "锡伯族", null, 38, 1, null },
                    { "sys_nation", "瑶族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "瑶族", null, 13, 1, null },
                    { "sys_nation", "彝族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "彝族", null, 7, 1, null },
                    { "sys_nation", "裕固族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "裕固族", null, 48, 1, null },
                    { "sys_nation", "壮族", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "壮族", null, 8, 1, null },
                    { "sys_school", "大冶市基二实验学校", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "大冶市基二实验学校", null, 2, 1, null },
                    { "sys_school", "坊前中学", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "坊前中学", null, 3, 1, null },
                    { "sys_school", "寒亭实验中学", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "寒亭实验中学", null, 5, 1, null },
                    { "sys_school", "淮安外国语学校", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "淮安外国语学校", null, 1, 1, null },
                    { "sys_school", "学铺侨实验小学", new DateTime(2026, 8, 17, 21, 0, 0, 0, DateTimeKind.Local), "学铺侨实验小学", null, 4, 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_gender", "男" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_gender", "女" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_grade_year", "2018级" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_grade_year", "2019级" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_grade_year", "2020级" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_grade_year", "2021级" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_grade_year", "2022级" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_grade_year", "2023级" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_grade_year", "2024级" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_grade_year", "2025级" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_grade_year", "2026级" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_myopia", "否" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_myopia", "是" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_myopia", "未知" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "阿昌族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "白族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "保安族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "布朗族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "布依族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "藏族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "朝鲜族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "达斡尔族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "傣族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "德昂族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "东乡族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "侗族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "独龙族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "俄罗斯族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "鄂伦春族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "鄂温克族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "高山族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "仡佬族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "哈尼族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "哈萨克族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "汉族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "赫哲族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "回族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "基诺族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "京族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "景颇族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "柯尔克孜族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "拉祜族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "黎族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "傈僳族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "珞巴族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "满族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "毛南族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "门巴族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "蒙古族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "苗族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "仫佬族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "纳西族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "怒族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "普米族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "其他" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "羌族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "撒拉族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "畲族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "水族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "塔吉克族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "塔塔尔族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "土家族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "土族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "佤族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "维吾尔族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "乌孜别克族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "锡伯族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "瑶族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "彝族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "裕固族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_nation", "壮族" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_school", "大冶市基二实验学校" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_school", "坊前中学" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_school", "寒亭实验中学" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_school", "淮安外国语学校" });

            migrationBuilder.DeleteData(
                table: "sys_dict_item",
                keyColumns: new[] { "DictType", "ItemValue" },
                keyValues: new object[] { "sys_school", "学铺侨实验小学" });

            migrationBuilder.DeleteData(
                table: "sys_dict",
                keyColumn: "Id",
                keyValue: 200L);

            migrationBuilder.DeleteData(
                table: "sys_dict",
                keyColumn: "Id",
                keyValue: 201L);

            migrationBuilder.DeleteData(
                table: "sys_dict",
                keyColumn: "Id",
                keyValue: 202L);

            migrationBuilder.DeleteData(
                table: "sys_dict",
                keyColumn: "Id",
                keyValue: 203L);

            migrationBuilder.DeleteData(
                table: "sys_dict",
                keyColumn: "Id",
                keyValue: 204L);

            migrationBuilder.DropColumn(
                name: "current_city",
                table: "student_archive");

            migrationBuilder.DropColumn(
                name: "current_district",
                table: "student_archive");

            migrationBuilder.DropColumn(
                name: "current_province",
                table: "student_archive");

            migrationBuilder.DropColumn(
                name: "current_street",
                table: "student_archive");

            migrationBuilder.DropColumn(
                name: "native_city",
                table: "student_archive");

            migrationBuilder.DropColumn(
                name: "native_province",
                table: "student_archive");

            migrationBuilder.DropColumn(
                name: "remark",
                table: "student_archive");
        }
    }
}

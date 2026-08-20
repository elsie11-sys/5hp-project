using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <summary>
    /// 数据采集与审核模块 5 张健康档案表（增量 migration）
    /// 使用 IF NOT EXISTS 兜底，重复执行不会报错
    /// 包含 5 张表 + 25 个索引 + 34 条 seed 数据
    /// </summary>
    public partial class _20260820070000_InitCollect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =================== student_vision_record ===================
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS student_vision_record (
    id              BIGSERIAL PRIMARY KEY,
    student_id      BIGINT NOT NULL DEFAULT 0,
    student_no      VARCHAR(20) NOT NULL DEFAULT '',
    student_name    VARCHAR(50) NOT NULL DEFAULT '',
    grade           VARCHAR(20) NOT NULL DEFAULT '',
    class_name      VARCHAR(50) NOT NULL DEFAULT '',
    check_date      TIMESTAMP NOT NULL,
    left_eye        VARCHAR(20) NOT NULL DEFAULT '',
    right_eye       VARCHAR(20) NOT NULL DEFAULT '',
    vision_level    VARCHAR(50) NOT NULL DEFAULT '',
    recorder_id     BIGINT NOT NULL DEFAULT 0,
    recorder_name   VARCHAR(50) NOT NULL DEFAULT '',
    record_time     TIMESTAMP NOT NULL,
    source          VARCHAR(20) NOT NULL DEFAULT 'manual',
    device_sn       VARCHAR(100),
    status          VARCHAR(20) NOT NULL DEFAULT 'pending',
    reviewer_id     BIGINT,
    reviewer_name   VARCHAR(50),
    review_time     TIMESTAMP,
    review_remark   VARCHAR(500),
    remark          VARCHAR(500),
    created_at      TIMESTAMP DEFAULT NOW(),
    updated_at      TIMESTAMP
);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_vision_record_student_id_check_date ON student_vision_record (student_id, check_date);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_vision_record_student_no ON student_vision_record (student_no);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_vision_record_status ON student_vision_record (status);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_vision_record_source ON student_vision_record (source);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_vision_record_record_time ON student_vision_record (record_time);");

            // =================== student_oral_record ===================
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS student_oral_record (
    id                        BIGSERIAL PRIMARY KEY,
    student_id                BIGINT NOT NULL DEFAULT 0,
    student_no                VARCHAR(20) NOT NULL DEFAULT '',
    student_name              VARCHAR(50) NOT NULL DEFAULT '',
    grade                     VARCHAR(20) NOT NULL DEFAULT '',
    class_name                VARCHAR(50) NOT NULL DEFAULT '',
    check_date                TIMESTAMP NOT NULL,
    decayed_baby_teeth        INT NOT NULL DEFAULT 0,
    decayed_permanent_teeth   INT NOT NULL DEFAULT 0,
    tooth_stage               VARCHAR(50) NOT NULL DEFAULT '',
    jaw_development           VARCHAR(50) NOT NULL DEFAULT '',
    recorder_id               BIGINT NOT NULL DEFAULT 0,
    recorder_name             VARCHAR(50) NOT NULL DEFAULT '',
    record_time               TIMESTAMP NOT NULL,
    source                    VARCHAR(20) NOT NULL DEFAULT 'manual',
    device_sn                 VARCHAR(100),
    status                    VARCHAR(20) NOT NULL DEFAULT 'pending',
    reviewer_id               BIGINT,
    reviewer_name             VARCHAR(50),
    review_time               TIMESTAMP,
    review_remark             VARCHAR(500),
    remark                    VARCHAR(500),
    created_at                TIMESTAMP DEFAULT NOW(),
    updated_at                TIMESTAMP
);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_oral_record_student_id_check_date ON student_oral_record (student_id, check_date);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_oral_record_student_no ON student_oral_record (student_no);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_oral_record_status ON student_oral_record (status);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_oral_record_source ON student_oral_record (source);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_oral_record_record_time ON student_oral_record (record_time);");

            // =================== student_mental_record ===================
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS student_mental_record (
    id                          BIGSERIAL PRIMARY KEY,
    student_id                  BIGINT NOT NULL DEFAULT 0,
    student_no                  VARCHAR(20) NOT NULL DEFAULT '',
    student_name                VARCHAR(50) NOT NULL DEFAULT '',
    grade                       VARCHAR(20) NOT NULL DEFAULT '',
    class_name                  VARCHAR(50) NOT NULL DEFAULT '',
    check_date                  TIMESTAMP NOT NULL,
    anxiety_score               INT NOT NULL DEFAULT 0,
    depression_score            INT NOT NULL DEFAULT 0,
    learning_anxiety            VARCHAR(50) NOT NULL DEFAULT '',
    interpersonal_sensitivity   VARCHAR(50) NOT NULL DEFAULT '',
    recorder_id                 BIGINT NOT NULL DEFAULT 0,
    recorder_name               VARCHAR(50) NOT NULL DEFAULT '',
    record_time                 TIMESTAMP NOT NULL,
    source                      VARCHAR(20) NOT NULL DEFAULT 'manual',
    device_sn                   VARCHAR(100),
    status                      VARCHAR(20) NOT NULL DEFAULT 'pending',
    reviewer_id                 BIGINT,
    reviewer_name               VARCHAR(50),
    review_time                 TIMESTAMP,
    review_remark               VARCHAR(500),
    remark                      VARCHAR(500),
    created_at                  TIMESTAMP DEFAULT NOW(),
    updated_at                  TIMESTAMP
);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_mental_record_student_id_check_date ON student_mental_record (student_id, check_date);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_mental_record_student_no ON student_mental_record (student_no);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_mental_record_status ON student_mental_record (status);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_mental_record_source ON student_mental_record (source);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_mental_record_record_time ON student_mental_record (record_time);");

            // =================== weight_record ===================
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS weight_record (
    id                    BIGSERIAL PRIMARY KEY,
    student_id            BIGINT NOT NULL DEFAULT 0,
    student_no            VARCHAR(20) NOT NULL DEFAULT '',
    student_name          VARCHAR(50) NOT NULL DEFAULT '',
    grade                 VARCHAR(20) NOT NULL DEFAULT '',
    class_name            VARCHAR(50) NOT NULL DEFAULT '',
    check_date            TIMESTAMP NOT NULL,
    height                VARCHAR(20) NOT NULL DEFAULT '',
    weight                VARCHAR(20) NOT NULL DEFAULT '',
    bmi                   VARCHAR(20) NOT NULL DEFAULT '',
    bmi_level             VARCHAR(50) NOT NULL DEFAULT '',
    waist_circumference   VARCHAR(20) NOT NULL DEFAULT '',
    hip_circumference     VARCHAR(20) NOT NULL DEFAULT '',
    whr                   VARCHAR(20) NOT NULL DEFAULT '',
    recorder_id           BIGINT NOT NULL DEFAULT 0,
    recorder_name         VARCHAR(50) NOT NULL DEFAULT '',
    record_time           TIMESTAMP NOT NULL,
    source                VARCHAR(20) NOT NULL DEFAULT 'manual',
    device_sn             VARCHAR(100),
    status                VARCHAR(20) NOT NULL DEFAULT 'pending',
    reviewer_id           BIGINT,
    reviewer_name         VARCHAR(50),
    review_time           TIMESTAMP,
    review_remark         VARCHAR(500),
    remark                VARCHAR(500),
    created_at            TIMESTAMP DEFAULT NOW(),
    updated_at            TIMESTAMP
);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_weight_record_student_id_check_date ON weight_record (student_id, check_date);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_weight_record_student_no ON weight_record (student_no);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_weight_record_status ON weight_record (status);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_weight_record_source ON weight_record (source);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_weight_record_record_time ON weight_record (record_time);");

            // =================== student_bone_record ===================
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS student_bone_record (
    id                BIGSERIAL PRIMARY KEY,
    student_id        BIGINT NOT NULL DEFAULT 0,
    student_no        VARCHAR(20) NOT NULL DEFAULT '',
    student_name      VARCHAR(50) NOT NULL DEFAULT '',
    grade             VARCHAR(20) NOT NULL DEFAULT '',
    class_name        VARCHAR(50) NOT NULL DEFAULT '',
    check_date        TIMESTAMP NOT NULL,
    bone_density      VARCHAR(20) NOT NULL DEFAULT '',
    bone_level        VARCHAR(50) NOT NULL DEFAULT '',
    bone_age          VARCHAR(20) NOT NULL DEFAULT '',
    vitamin_d         VARCHAR(50) NOT NULL DEFAULT '',
    calcium_level     VARCHAR(50) NOT NULL DEFAULT '',
    recorder_id       BIGINT NOT NULL DEFAULT 0,
    recorder_name     VARCHAR(50) NOT NULL DEFAULT '',
    record_time       TIMESTAMP NOT NULL,
    source            VARCHAR(20) NOT NULL DEFAULT 'manual',
    device_sn         VARCHAR(100),
    status            VARCHAR(20) NOT NULL DEFAULT 'pending',
    reviewer_id       BIGINT,
    reviewer_name     VARCHAR(50),
    review_time       TIMESTAMP,
    review_remark     VARCHAR(500),
    remark            VARCHAR(500),
    created_at        TIMESTAMP DEFAULT NOW(),
    updated_at        TIMESTAMP
);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_bone_record_student_id_check_date ON student_bone_record (student_id, check_date);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_bone_record_student_no ON student_bone_record (student_no);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_bone_record_status ON student_bone_record (status);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_bone_record_source ON student_bone_record (source);");
            migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS IX_student_bone_record_record_time ON student_bone_record (record_time);");

            // =================== seed 数据（用 ON CONFLICT 兜底） ===================
            var seedTime = "2026-08-20 11:00:00";
            migrationBuilder.Sql($@"
INSERT INTO student_vision_record (id, student_id, student_no, student_name, grade, class_name, check_date, left_eye, right_eye, vision_level, recorder_id, recorder_name, record_time, source, device_sn, status, reviewer_id, reviewer_name, review_time, created_at, updated_at) VALUES
(1, 6, '2022006', '招崧熙', '七年级', '七年级五班', '2026-06-28', '4.8', '4.9', '轻度近视', 7, '王老师', '2026-06-28 14:30:00', 'manual', NULL, 'pending', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(2, 6, '2022006', '招崧熙', '七年级', '七年级五班', '2026-03-15', '4.7', '4.8', '轻度近视', 7, '王老师', '2026-03-15 10:00:00', 'manual', NULL, 'approved', 6, '校医李', '2026-03-16 09:00:00', '{seedTime}', '{seedTime}'),
(3, 6, '2022006', '招崧熙', '七年级', '七年级五班', '2025-12-01', '4.9', '4.9', '正常', 7, '王老师', '2025-12-01 11:00:00', 'device', 'VISION-A001', 'approved', 6, '校医李', '2025-12-02 09:00:00', '{seedTime}', '{seedTime}'),
(4, 1, '2022001', '陈熙航', '七年级', '七年级五班', '2026-06-28', '4.5', '4.6', '中度近视', 7, '王老师', '2026-06-28 14:35:00', 'manual', NULL, 'abnormal', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(5, 1, '2022001', '陈熙航', '七年级', '七年级五班', '2026-03-15', '4.7', '4.7', '轻度近视', 7, '王老师', '2026-03-15 10:00:00', 'manual', NULL, 'approved', 6, '校医李', '2026-03-16 09:00:00', '{seedTime}', '{seedTime}'),
(6, 2, '2022002', '席振轩', '七年级', '七年级五班', '2026-06-28', '5.0', '5.0', '正常', 7, '王老师', '2026-06-28 14:40:00', 'manual', NULL, 'approved', 6, '校医李', '2026-06-29 09:00:00', '{seedTime}', '{seedTime}'),
(7, 3, '2022003', '丁书婉', '七年级', '七年级五班', '2026-06-28', '4.9', '4.8', '轻度近视', 7, '王老师', '2026-06-28 14:45:00', 'manual', NULL, 'pending', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(8, 4, '2022004', '周梓萌', '七年级', '七年级五班', '2026-06-28', '4.6', '4.7', '轻度近视', 7, '王老师', '2026-06-28 14:50:00', 'manual', NULL, 'spot', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(9, 5, '2022005', '朱宇土', '七年级', '七年级五班', '2026-06-28', '5.0', '4.9', '正常', 7, '王老师', '2026-06-28 14:55:00', 'device', 'VISION-A001', 'approved', 6, '校医李', '2026-06-29 09:00:00', '{seedTime}', '{seedTime}'),
(10, 1, '2022001', '陈熙航', '七年级', '七年级五班', '2025-12-01', '4.8', '4.9', '正常', 7, '王老师', '2025-12-01 11:00:00', 'device', 'VISION-A001', 'approved', 6, '校医李', '2025-12-02 09:00:00', '{seedTime}', '{seedTime}')
ON CONFLICT (id) DO NOTHING;");

            migrationBuilder.Sql($@"
INSERT INTO student_oral_record (id, student_id, student_no, student_name, grade, class_name, check_date, decayed_baby_teeth, decayed_permanent_teeth, tooth_stage, jaw_development, recorder_id, recorder_name, record_time, source, device_sn, status, reviewer_id, reviewer_name, review_time, created_at, updated_at) VALUES
(1, 6, '2022006', '招崧熙', '七年级', '七年级五班', '2026-06-20', 0, 0, '替牙期', '正常', 7, '王老师', '2026-06-20 10:00:00', 'manual', NULL, 'approved', 6, '校医李', '2026-06-21 09:00:00', '{seedTime}', '{seedTime}'),
(2, 1, '2022001', '陈熙航', '七年级', '七年级五班', '2026-06-20', 2, 0, '替牙期', '正常', 7, '王老师', '2026-06-20 10:05:00', 'manual', NULL, 'pending', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(3, 2, '2022002', '席振轩', '七年级', '七年级五班', '2026-06-20', 0, 0, '替牙期', '正常', 7, '王老师', '2026-06-20 10:10:00', 'manual', NULL, 'approved', 6, '校医李', '2026-06-21 09:00:00', '{seedTime}', '{seedTime}'),
(4, 3, '2022003', '丁书婉', '七年级', '七年级五班', '2026-06-20', 0, 0, '替牙期', '正常', 7, '王老师', '2026-06-20 10:15:00', 'manual', NULL, 'approved', 6, '校医李', '2026-06-21 09:00:00', '{seedTime}', '{seedTime}'),
(5, 4, '2022004', '周梓萌', '七年级', '七年级五班', '2026-06-20', 1, 0, '替牙期', '正常', 7, '王老师', '2026-06-20 10:20:00', 'manual', NULL, 'abnormal', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(6, 5, '2022005', '朱宇土', '七年级', '七年级五班', '2026-06-20', 0, 0, '替牙期', '正常', 7, '王老师', '2026-06-20 10:25:00', 'device', 'ORAL-B002', 'approved', 6, '校医李', '2026-06-21 09:00:00', '{seedTime}', '{seedTime}')
ON CONFLICT (id) DO NOTHING;");

            migrationBuilder.Sql($@"
INSERT INTO student_mental_record (id, student_id, student_no, student_name, grade, class_name, check_date, anxiety_score, depression_score, learning_anxiety, interpersonal_sensitivity, recorder_id, recorder_name, record_time, source, device_sn, status, reviewer_id, reviewer_name, review_time, created_at, updated_at) VALUES
(1, 6, '2022006', '招崧熙', '七年级', '七年级五班', '2026-06-25', 8, 6, '轻度', '正常', 7, '王老师', '2026-06-25 14:00:00', 'manual', NULL, 'approved', 8, '心理张', '2026-06-26 09:00:00', '{seedTime}', '{seedTime}'),
(2, 1, '2022001', '陈熙航', '七年级', '七年级五班', '2026-06-25', 14, 10, '中度', '中度', 7, '王老师', '2026-06-25 14:05:00', 'manual', NULL, 'pending', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(3, 2, '2022002', '席振轩', '七年级', '七年级五班', '2026-06-25', 6, 5, '轻度', '正常', 7, '王老师', '2026-06-25 14:10:00', 'manual', NULL, 'approved', 8, '心理张', '2026-06-26 09:00:00', '{seedTime}', '{seedTime}'),
(4, 3, '2022003', '丁书婉', '七年级', '七年级五班', '2026-06-25', 10, 8, '轻度', '轻度', 7, '王老师', '2026-06-25 14:15:00', 'manual', NULL, 'pending', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(5, 4, '2022004', '周梓萌', '七年级', '七年级五班', '2026-06-25', 12, 9, '中度', '轻度', 7, '王老师', '2026-06-25 14:20:00', 'manual', NULL, 'abnormal', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(6, 5, '2022005', '朱宇土', '七年级', '七年级五班', '2026-06-25', 7, 5, '轻度', '正常', 7, '王老师', '2026-06-25 14:25:00', 'manual', NULL, 'approved', 8, '心理张', '2026-06-26 09:00:00', '{seedTime}', '{seedTime}')
ON CONFLICT (id) DO NOTHING;");

            migrationBuilder.Sql($@"
INSERT INTO weight_record (id, student_id, student_no, student_name, grade, class_name, check_date, height, weight, bmi, bmi_level, waist_circumference, hip_circumference, whr, recorder_id, recorder_name, record_time, source, device_sn, status, reviewer_id, reviewer_name, review_time, created_at, updated_at) VALUES
(1, 6, '2022006', '招崧熙', '七年级', '七年级五班', '2026-06-28', '165', '52', '19.1', '正常', '68', '88', '0.77', 9, '体育赵', '2026-06-28 09:00:00', 'device', 'WEIGHT-C003', 'approved', 6, '校医李', '2026-06-29 09:00:00', '{seedTime}', '{seedTime}'),
(2, 1, '2022001', '陈熙航', '七年级', '七年级五班', '2026-06-28', '162', '55', '21.0', '正常', '72', '90', '0.80', 9, '体育赵', '2026-06-28 09:05:00', 'manual', NULL, 'pending', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(3, 2, '2022002', '席振轩', '七年级', '七年级五班', '2026-06-28', '160', '48', '18.8', '正常', '65', '85', '0.76', 9, '体育赵', '2026-06-28 09:10:00', 'manual', NULL, 'approved', 6, '校医李', '2026-06-29 09:00:00', '{seedTime}', '{seedTime}'),
(4, 3, '2022003', '丁书婉', '七年级', '七年级五班', '2026-06-28', '158', '45', '18.0', '正常', '62', '84', '0.74', 9, '体育赵', '2026-06-28 09:15:00', 'manual', NULL, 'pending', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(5, 4, '2022004', '周梓萌', '七年级', '七年级五班', '2026-06-28', '155', '60', '25.0', '超重', '78', '86', '0.91', 9, '体育赵', '2026-06-28 09:20:00', 'manual', NULL, 'abnormal', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(6, 5, '2022005', '朱宇土', '七年级', '七年级五班', '2026-06-28', '163', '54', '20.3', '正常', '70', '89', '0.79', 9, '体育赵', '2026-06-28 09:25:00', 'device', 'WEIGHT-C003', 'approved', 6, '校医李', '2026-06-29 09:00:00', '{seedTime}', '{seedTime}')
ON CONFLICT (id) DO NOTHING;");

            migrationBuilder.Sql($@"
INSERT INTO student_bone_record (id, student_id, student_no, student_name, grade, class_name, check_date, bone_density, bone_level, bone_age, vitamin_d, calcium_level, recorder_id, recorder_name, record_time, source, device_sn, status, reviewer_id, reviewer_name, review_time, created_at, updated_at) VALUES
(1, 6, '2022006', '招崧熙', '七年级', '七年级五班', '2026-06-20', '0.85', '正常', '12岁', '充足', '正常', 6, '校医李', '2026-06-20 14:00:00', 'device', 'BONE-D004', 'pending', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(2, 1, '2022001', '陈熙航', '七年级', '七年级五班', '2026-06-20', '0.82', '正常', '12.5岁', '充足', '正常', 6, '校医李', '2026-06-20 14:05:00', 'device', 'BONE-D004', 'pending', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(3, 2, '2022002', '席振轩', '七年级', '七年级五班', '2026-06-20', '0.88', '正常', '12岁', '充足', '正常', 6, '校医李', '2026-06-20 14:10:00', 'device', 'BONE-D004', 'approved', 10, '专家王', '2026-06-21 09:00:00', '{seedTime}', '{seedTime}'),
(4, 3, '2022003', '丁书婉', '七年级', '七年级五班', '2026-06-20', '0.65', '偏低', '11.5岁', '不足', '偏低', 6, '校医李', '2026-06-20 14:15:00', 'device', 'BONE-D004', 'abnormal', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(5, 4, '2022004', '周梓萌', '七年级', '七年级五班', '2026-06-20', '0.78', '正常', '11岁', '充足', '正常', 6, '校医李', '2026-06-20 14:20:00', 'manual', NULL, 'pending', NULL, NULL, NULL, '{seedTime}', '{seedTime}'),
(6, 5, '2022005', '朱宇土', '七年级', '七年级五班', '2026-06-20', '0.86', '正常', '12.5岁', '充足', '正常', 6, '校医李', '2026-06-20 14:25:00', 'device', 'BONE-D004', 'approved', 10, '专家王', '2026-06-21 09:00:00', '{seedTime}', '{seedTime}')
ON CONFLICT (id) DO NOTHING;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS student_vision_record CASCADE;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS student_oral_record CASCADE;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS student_mental_record CASCADE;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS weight_record CASCADE;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS student_bone_record CASCADE;");
        }
    }
}

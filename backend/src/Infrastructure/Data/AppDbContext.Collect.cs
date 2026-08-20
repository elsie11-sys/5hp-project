using Application.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

/// <summary>
/// 数据采集与审核模块 5 张表的 Fluent 配置
/// - snake_case 列名
/// - 通用业务索引（学号 / 状态 / 来源 / 时间）
/// </summary>
public partial class AppDbContext
{
    private void ConfigureCollectTables(ModelBuilder modelBuilder)
    {
        // ===================== student_vision_record =====================
        modelBuilder.Entity<VisionRecord>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd().UseIdentityAlwaysColumn();
            entity.Property(r => r.StudentId).HasColumnName("student_id");
            entity.Property(r => r.StudentNo).HasColumnName("student_no").HasMaxLength(20);
            entity.Property(r => r.StudentName).HasColumnName("student_name").HasMaxLength(50);
            entity.Property(r => r.Grade).HasColumnName("grade").HasMaxLength(20);
            entity.Property(r => r.ClassName).HasColumnName("class_name").HasMaxLength(50);
            entity.Property(r => r.CheckDate).HasColumnName("check_date");
            entity.Property(r => r.LeftEye).HasColumnName("left_eye").HasMaxLength(20);
            entity.Property(r => r.RightEye).HasColumnName("right_eye").HasMaxLength(20);
            entity.Property(r => r.VisionLevel).HasColumnName("vision_level").HasMaxLength(50);
            entity.Property(r => r.RecorderId).HasColumnName("recorder_id");
            entity.Property(r => r.RecorderName).HasColumnName("recorder_name").HasMaxLength(50);
            entity.Property(r => r.RecordTime).HasColumnName("record_time");
            entity.Property(r => r.Source).HasColumnName("source").HasMaxLength(20).HasDefaultValue("manual");
            entity.Property(r => r.DeviceSn).HasColumnName("device_sn").HasMaxLength(100);
            entity.Property(r => r.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("pending");
            entity.Property(r => r.ReviewerId).HasColumnName("reviewer_id");
            entity.Property(r => r.ReviewerName).HasColumnName("reviewer_name").HasMaxLength(50);
            entity.Property(r => r.ReviewTime).HasColumnName("review_time");
            entity.Property(r => r.ReviewRemark).HasColumnName("review_remark").HasMaxLength(500);
            entity.Property(r => r.Remark).HasColumnName("remark").HasMaxLength(500);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("student_vision_record");

            entity.HasIndex(r => new { r.StudentId, r.CheckDate });
            entity.HasIndex(r => r.StudentNo);
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => r.Source);
            entity.HasIndex(r => r.RecordTime);
        });

        // ===================== student_oral_record =====================
        modelBuilder.Entity<OralRecord>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd().UseIdentityAlwaysColumn();
            entity.Property(r => r.StudentId).HasColumnName("student_id");
            entity.Property(r => r.StudentNo).HasColumnName("student_no").HasMaxLength(20);
            entity.Property(r => r.StudentName).HasColumnName("student_name").HasMaxLength(50);
            entity.Property(r => r.Grade).HasColumnName("grade").HasMaxLength(20);
            entity.Property(r => r.ClassName).HasColumnName("class_name").HasMaxLength(50);
            entity.Property(r => r.CheckDate).HasColumnName("check_date");
            entity.Property(r => r.DecayedBabyTeeth).HasColumnName("decayed_baby_teeth");
            entity.Property(r => r.DecayedPermanentTeeth).HasColumnName("decayed_permanent_teeth");
            entity.Property(r => r.ToothStage).HasColumnName("tooth_stage").HasMaxLength(50);
            entity.Property(r => r.JawDevelopment).HasColumnName("jaw_development").HasMaxLength(50);
            entity.Property(r => r.RecorderId).HasColumnName("recorder_id");
            entity.Property(r => r.RecorderName).HasColumnName("recorder_name").HasMaxLength(50);
            entity.Property(r => r.RecordTime).HasColumnName("record_time");
            entity.Property(r => r.Source).HasColumnName("source").HasMaxLength(20).HasDefaultValue("manual");
            entity.Property(r => r.DeviceSn).HasColumnName("device_sn").HasMaxLength(100);
            entity.Property(r => r.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("pending");
            entity.Property(r => r.ReviewerId).HasColumnName("reviewer_id");
            entity.Property(r => r.ReviewerName).HasColumnName("reviewer_name").HasMaxLength(50);
            entity.Property(r => r.ReviewTime).HasColumnName("review_time");
            entity.Property(r => r.ReviewRemark).HasColumnName("review_remark").HasMaxLength(500);
            entity.Property(r => r.Remark).HasColumnName("remark").HasMaxLength(500);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("student_oral_record");

            entity.HasIndex(r => new { r.StudentId, r.CheckDate });
            entity.HasIndex(r => r.StudentNo);
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => r.Source);
            entity.HasIndex(r => r.RecordTime);
        });

        // ===================== student_mental_record =====================
        modelBuilder.Entity<MentalRecord>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd().UseIdentityAlwaysColumn();
            entity.Property(r => r.StudentId).HasColumnName("student_id");
            entity.Property(r => r.StudentNo).HasColumnName("student_no").HasMaxLength(20);
            entity.Property(r => r.StudentName).HasColumnName("student_name").HasMaxLength(50);
            entity.Property(r => r.Grade).HasColumnName("grade").HasMaxLength(20);
            entity.Property(r => r.ClassName).HasColumnName("class_name").HasMaxLength(50);
            entity.Property(r => r.CheckDate).HasColumnName("check_date");
            entity.Property(r => r.AnxietyScore).HasColumnName("anxiety_score");
            entity.Property(r => r.DepressionScore).HasColumnName("depression_score");
            entity.Property(r => r.LearningAnxiety).HasColumnName("learning_anxiety").HasMaxLength(50);
            entity.Property(r => r.InterpersonalSensitivity).HasColumnName("interpersonal_sensitivity").HasMaxLength(50);
            entity.Property(r => r.RecorderId).HasColumnName("recorder_id");
            entity.Property(r => r.RecorderName).HasColumnName("recorder_name").HasMaxLength(50);
            entity.Property(r => r.RecordTime).HasColumnName("record_time");
            entity.Property(r => r.Source).HasColumnName("source").HasMaxLength(20).HasDefaultValue("manual");
            entity.Property(r => r.DeviceSn).HasColumnName("device_sn").HasMaxLength(100);
            entity.Property(r => r.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("pending");
            entity.Property(r => r.ReviewerId).HasColumnName("reviewer_id");
            entity.Property(r => r.ReviewerName).HasColumnName("reviewer_name").HasMaxLength(50);
            entity.Property(r => r.ReviewTime).HasColumnName("review_time");
            entity.Property(r => r.ReviewRemark).HasColumnName("review_remark").HasMaxLength(500);
            entity.Property(r => r.Remark).HasColumnName("remark").HasMaxLength(500);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("student_mental_record");

            entity.HasIndex(r => new { r.StudentId, r.CheckDate });
            entity.HasIndex(r => r.StudentNo);
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => r.Source);
            entity.HasIndex(r => r.RecordTime);
        });

        // ===================== weight_record =====================
        modelBuilder.Entity<WeightRecord>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd().UseIdentityAlwaysColumn();
            entity.Property(r => r.StudentId).HasColumnName("student_id");
            entity.Property(r => r.StudentNo).HasColumnName("student_no").HasMaxLength(20);
            entity.Property(r => r.StudentName).HasColumnName("student_name").HasMaxLength(50);
            entity.Property(r => r.Grade).HasColumnName("grade").HasMaxLength(20);
            entity.Property(r => r.ClassName).HasColumnName("class_name").HasMaxLength(50);
            entity.Property(r => r.CheckDate).HasColumnName("check_date");
            entity.Property(r => r.Height).HasColumnName("height").HasMaxLength(20);
            entity.Property(r => r.Weight).HasColumnName("weight").HasMaxLength(20);
            entity.Property(r => r.Bmi).HasColumnName("bmi").HasMaxLength(20);
            entity.Property(r => r.BmiLevel).HasColumnName("bmi_level").HasMaxLength(50);
            entity.Property(r => r.WaistCircumference).HasColumnName("waist_circumference").HasMaxLength(20);
            entity.Property(r => r.HipCircumference).HasColumnName("hip_circumference").HasMaxLength(20);
            entity.Property(r => r.Whr).HasColumnName("whr").HasMaxLength(20);
            entity.Property(r => r.RecorderId).HasColumnName("recorder_id");
            entity.Property(r => r.RecorderName).HasColumnName("recorder_name").HasMaxLength(50);
            entity.Property(r => r.RecordTime).HasColumnName("record_time");
            entity.Property(r => r.Source).HasColumnName("source").HasMaxLength(20).HasDefaultValue("manual");
            entity.Property(r => r.DeviceSn).HasColumnName("device_sn").HasMaxLength(100);
            entity.Property(r => r.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("pending");
            entity.Property(r => r.ReviewerId).HasColumnName("reviewer_id");
            entity.Property(r => r.ReviewerName).HasColumnName("reviewer_name").HasMaxLength(50);
            entity.Property(r => r.ReviewTime).HasColumnName("review_time");
            entity.Property(r => r.ReviewRemark).HasColumnName("review_remark").HasMaxLength(500);
            entity.Property(r => r.Remark).HasColumnName("remark").HasMaxLength(500);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("weight_record");

            entity.HasIndex(r => new { r.StudentId, r.CheckDate });
            entity.HasIndex(r => r.StudentNo);
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => r.Source);
            entity.HasIndex(r => r.RecordTime);
        });

        // ===================== student_bone_record =====================
        modelBuilder.Entity<BoneRecord>(entity =>
        {
            entity.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd().UseIdentityAlwaysColumn();
            entity.Property(r => r.StudentId).HasColumnName("student_id");
            entity.Property(r => r.StudentNo).HasColumnName("student_no").HasMaxLength(20);
            entity.Property(r => r.StudentName).HasColumnName("student_name").HasMaxLength(50);
            entity.Property(r => r.Grade).HasColumnName("grade").HasMaxLength(20);
            entity.Property(r => r.ClassName).HasColumnName("class_name").HasMaxLength(50);
            entity.Property(r => r.CheckDate).HasColumnName("check_date");
            entity.Property(r => r.BoneDensity).HasColumnName("bone_density").HasMaxLength(20);
            entity.Property(r => r.BoneLevel).HasColumnName("bone_level").HasMaxLength(50);
            entity.Property(r => r.BoneAge).HasColumnName("bone_age").HasMaxLength(20);
            entity.Property(r => r.VitaminD).HasColumnName("vitamin_d").HasMaxLength(50);
            entity.Property(r => r.CalciumLevel).HasColumnName("calcium_level").HasMaxLength(50);
            entity.Property(r => r.RecorderId).HasColumnName("recorder_id");
            entity.Property(r => r.RecorderName).HasColumnName("recorder_name").HasMaxLength(50);
            entity.Property(r => r.RecordTime).HasColumnName("record_time");
            entity.Property(r => r.Source).HasColumnName("source").HasMaxLength(20).HasDefaultValue("manual");
            entity.Property(r => r.DeviceSn).HasColumnName("device_sn").HasMaxLength(100);
            entity.Property(r => r.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("pending");
            entity.Property(r => r.ReviewerId).HasColumnName("reviewer_id");
            entity.Property(r => r.ReviewerName).HasColumnName("reviewer_name").HasMaxLength(50);
            entity.Property(r => r.ReviewTime).HasColumnName("review_time");
            entity.Property(r => r.ReviewRemark).HasColumnName("review_remark").HasMaxLength(500);
            entity.Property(r => r.Remark).HasColumnName("remark").HasMaxLength(500);
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            entity.ToTable("student_bone_record");

            entity.HasIndex(r => new { r.StudentId, r.CheckDate });
            entity.HasIndex(r => r.StudentNo);
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => r.Source);
            entity.HasIndex(r => r.RecordTime);
        });
    }
}

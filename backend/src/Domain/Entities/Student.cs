using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// 学生档案实体（视力档案管理系统 - 学生基本信息）
/// 对应数据库表：student_archive
/// </summary>
[Table("student_archive")]
public class Student
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 学号（业务唯一标识）
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string StudentNo { get; set; } = string.Empty;

    /// <summary>
    /// 头像（emoji 或 URL）
    /// </summary>
    [MaxLength(500)]
    public string Avatar { get; set; } = "👦";

    /// <summary>
    /// 年级/届（如：2022级）
    /// </summary>
    [MaxLength(50)]
    public string GradeYear { get; set; } = string.Empty;

    /// <summary>
    /// 学校
    /// </summary>
    [MaxLength(100)]
    public string School { get; set; } = string.Empty;

    /// <summary>
    /// 班级
    /// </summary>
    [MaxLength(100)]
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// 姓名
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 性别（男 / 女）
    /// </summary>
    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 民族
    /// </summary>
    [MaxLength(50)]
    public string Nation { get; set; } = "汉族";

    /// <summary>
    /// 出生日期
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 身份证号
    /// </summary>
    [MaxLength(50)]
    public string IdCard { get; set; } = string.Empty;

    /// <summary>
    /// 籍贯
    /// </summary>
    [MaxLength(100)]
    public string NativePlace { get; set; } = string.Empty;

    /// <summary>
    /// 现居住地
    /// </summary>
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 家长电话
    /// </summary>
    [MaxLength(20)]
    public string ParentPhone { get; set; } = string.Empty;

    /// <summary>
    /// 家长微信/QQ
    /// </summary>
    [MaxLength(100)]
    public string ParentContact { get; set; } = string.Empty;

    /// <summary>
    /// 过敏史
    /// </summary>
    [MaxLength(255)]
    public string AllergyHistory { get; set; } = "未知";

    /// <summary>
    /// 父亲近视情况
    /// </summary>
    [MaxLength(50)]
    public string FatherMyopia { get; set; } = "未知";

    /// <summary>
    /// 母亲近视情况
    /// </summary>
    [MaxLength(50)]
    public string MotherMyopia { get; set; } = "未知";

    /// <summary>
    /// 数据来源（手动录入 / 批量导入 / 系统对接 等）
    /// </summary>
    [MaxLength(50)]
    public string DataSource { get; set; } = "手动录入";

    /// <summary>
    /// 是否开启干预
    /// </summary>
    public bool Intervention { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

namespace Application.DTOs;

/// <summary>
/// 学生档案 DTO（前后端传输用）
/// 字段顺序/命名与前端 student/index.vue 中表单保持一致
/// </summary>
public class StudentDto
{
    public long Id { get; set; }

    /// <summary>学号（必填）</summary>
    public string StudentNo { get; set; } = string.Empty;

    /// <summary>头像（emoji 或 URL）</summary>
    public string Avatar { get; set; } = "👦";

    /// <summary>年级/届（如 2022级）</summary>
    public string GradeYear { get; set; } = string.Empty;

    /// <summary>学校</summary>
    public string School { get; set; } = string.Empty;

    /// <summary>班级</summary>
    public string ClassName { get; set; } = string.Empty;

    /// <summary>姓名（必填）</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>性别（男/女）</summary>
    public string Gender { get; set; } = "男";

    /// <summary>民族</summary>
    public string Nation { get; set; } = "汉族";

    /// <summary>出生日期（ISO 字符串）</summary>
    public string? Birthday { get; set; }

    /// <summary>身份证号</summary>
    public string IdCard { get; set; } = string.Empty;

    /// <summary>籍贯</summary>
    public string NativePlace { get; set; } = string.Empty;

    /// <summary>现居住地</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>家长电话</summary>
    public string ParentPhone { get; set; } = string.Empty;

    /// <summary>家长微信/QQ</summary>
    public string ParentContact { get; set; } = string.Empty;

    /// <summary>过敏史</summary>
    public string AllergyHistory { get; set; } = "未知";

    /// <summary>父亲近视</summary>
    public string FatherMyopia { get; set; } = "未知";

    /// <summary>母亲近视</summary>
    public string MotherMyopia { get; set; } = "未知";

    /// <summary>数据来源</summary>
    public string DataSource { get; set; } = "手动录入";

    /// <summary>是否干预</summary>
    public bool Intervention { get; set; }

    /// <summary>创建时间</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>更新时间</summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 学生档案查询参数
/// </summary>
public class StudentQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    /// <summary>学校精确匹配</summary>
    public string? School { get; set; }

    /// <summary>班级精确匹配</summary>
    public string? ClassName { get; set; }

    /// <summary>姓名模糊匹配</summary>
    public string? Name { get; set; }

    /// <summary>学号精确匹配（路由定位时使用）</summary>
    public string? StudentNo { get; set; }

    /// <summary>是否干预（null=全部，true=干预中，false=未干预）</summary>
    public bool? Intervention { get; set; }

    /// <summary>关键词（学号/姓名 同时模糊）</summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// 切换干预状态请求
/// </summary>
public class ToggleInterventionRequest
{
    public bool Intervention { get; set; }
}

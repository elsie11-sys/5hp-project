using Application.DTOs;

namespace Application.Interfaces;

/// <summary>
/// 学生档案服务接口
/// </summary>
public interface IStudentService
{
    /// <summary>
    /// 分页查询学生档案
    /// </summary>
    Task<(List<StudentDto> Items, int Total)> GetPagedStudentsAsync(StudentQuery query);

    /// <summary>
    /// 获取所有学生（用于下拉/选择器）
    /// </summary>
    Task<List<StudentDto>> GetAllStudentsAsync();

    /// <summary>
    /// 根据 ID 获取学生档案
    /// </summary>
    Task<StudentDto?> GetStudentByIdAsync(long id);

    /// <summary>
    /// 根据学号获取学生档案
    /// </summary>
    Task<StudentDto?> GetStudentByNoAsync(string studentNo);

    /// <summary>
    /// 创建学生档案
    /// </summary>
    Task<StudentDto> CreateStudentAsync(StudentDto dto);

    /// <summary>
    /// 更新学生档案
    /// </summary>
    Task<StudentDto> UpdateStudentAsync(StudentDto dto);

    /// <summary>
    /// 删除单个学生档案
    /// </summary>
    Task<bool> DeleteStudentAsync(long id);

    /// <summary>
    /// 批量删除学生档案
    /// </summary>
    Task<int> BatchDeleteStudentsAsync(IReadOnlyList<long> ids);

    /// <summary>
    /// 切换是否干预状态
    /// </summary>
    Task<bool> ToggleInterventionAsync(long id, bool intervention);

    /// <summary>
    /// 检查学号是否已存在（编辑时排除自身）
    /// </summary>
    Task<bool> IsStudentNoExistsAsync(string studentNo, long? excludeId = null);
}

using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// 学生档案管理 - RESTful API
/// 路径前缀：/api/student
/// </summary>
[ApiController]
[Route("api/student")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// 分页查询学生档案
    /// GET /api/student/paged?page=1&pageSize=10&school=xxx&className=xxx&name=xxx&intervention=true&keyword=xxx
    /// </summary>
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? school = null,
        [FromQuery] string? className = null,
        [FromQuery] string? name = null,
        [FromQuery] string? studentNo = null,
        [FromQuery] bool? intervention = null,
        [FromQuery] string? keyword = null)
    {
        // 限制最大 pageSize
        if (pageSize <= 0) pageSize = 20;
        if (pageSize > 200) pageSize = 200;
        if (page <= 0) page = 1;

        var query = new StudentQuery
        {
            Page = page,
            PageSize = pageSize,
            School = school,
            ClassName = className,
            Name = name,
            StudentNo = studentNo,
            Intervention = intervention,
            Keyword = keyword,
        };

        var (items, total) = await _studentService.GetPagedStudentsAsync(query);
        return Ok(new { items, total });
    }

    /// <summary>
    /// 获取所有学生档案（用于下拉/选择器）
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _studentService.GetAllStudentsAsync();
        return Ok(list);
    }

    /// <summary>
    /// 根据 ID 获取学生档案
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var s = await _studentService.GetStudentByIdAsync(id);
        if (s == null)
        {
            return NotFound(new { message = "学生档案不存在" });
        }
        return Ok(s);
    }

    /// <summary>
    /// 根据学号获取学生档案
    /// </summary>
    [HttpGet("by-no/{studentNo}")]
    public async Task<IActionResult> GetByNo(string studentNo)
    {
        var s = await _studentService.GetStudentByNoAsync(studentNo);
        if (s == null)
        {
            return NotFound(new { message = "学生档案不存在" });
        }
        return Ok(s);
    }

    /// <summary>
    /// 创建学生档案
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "请求体不能为空" });
        }
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new { message = "姓名不能为空" });
        }
        if (string.IsNullOrWhiteSpace(dto.StudentNo))
        {
            return BadRequest(new { message = "学号不能为空" });
        }
        if (await _studentService.IsStudentNoExistsAsync(dto.StudentNo))
        {
            return BadRequest(new { message = $"学号 {dto.StudentNo} 已存在" });
        }

        var created = await _studentService.CreateStudentAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// 更新学生档案
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] StudentDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "请求体不能为空" });
        }
        if (id != dto.Id)
        {
            return BadRequest(new { message = "URL 中的 ID 与请求体中的 ID 不匹配" });
        }
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new { message = "姓名不能为空" });
        }
        if (string.IsNullOrWhiteSpace(dto.StudentNo))
        {
            return BadRequest(new { message = "学号不能为空" });
        }
        if (await _studentService.IsStudentNoExistsAsync(dto.StudentNo, dto.Id))
        {
            return BadRequest(new { message = $"学号 {dto.StudentNo} 已被其他学生占用" });
        }

        try
        {
            var updated = await _studentService.UpdateStudentAsync(dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 删除单个学生档案
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var ok = await _studentService.DeleteStudentAsync(id);
        if (!ok)
        {
            return NotFound(new { message = "学生档案不存在，删除失败" });
        }
        return NoContent();
    }

    /// <summary>
    /// 批量删除学生档案
    /// POST /api/student/batch-delete  body: { "ids": [1,2,3] }
    /// </summary>
    [HttpPost("batch-delete")]
    public async Task<IActionResult> BatchDelete([FromBody] BatchDeleteRequest request)
    {
        if (request?.Ids == null || request.Ids.Count == 0)
        {
            return BadRequest(new { message = "请选择要删除的学生" });
        }
        var count = await _studentService.BatchDeleteStudentsAsync(request.Ids);
        return Ok(new { message = $"已删除 {count} 名学生", deletedCount = count });
    }

    /// <summary>
    /// 切换是否干预状态
    /// POST /api/student/{id}/toggle-intervention  body: { "intervention": true }
    /// </summary>
    [HttpPost("{id:long}/toggle-intervention")]
    public async Task<IActionResult> ToggleIntervention(long id, [FromBody] ToggleInterventionRequest? body)
    {
        var intervention = body?.Intervention ?? false;
        var ok = await _studentService.ToggleInterventionAsync(id, intervention);
        if (!ok)
        {
            return NotFound(new { message = "学生档案不存在" });
        }
        return Ok(new { message = "已更新干预状态", intervention });
    }

    /// <summary>
    /// 检查学号是否已被占用（前端编辑时实时反馈）
    /// </summary>
    [HttpGet("check-no")]
    public async Task<IActionResult> CheckNo([FromQuery] string studentNo, [FromQuery] long? excludeId = null)
    {
        var exists = await _studentService.IsStudentNoExistsAsync(studentNo, excludeId);
        return Ok(new { exists });
    }
}

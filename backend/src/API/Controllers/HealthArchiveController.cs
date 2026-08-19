using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// 学生健康档案 - 5 维度 RESTful API
/// 路径前缀：/api/health-archive
/// </summary>
[ApiController]
[Route("api/health-archive")]
public class HealthArchiveController : ControllerBase
{
    private readonly IHealthArchiveService _service;

    public HealthArchiveController(IHealthArchiveService service)
    {
        _service = service;
    }

    // ============================================================
    // 视力健康
    // ============================================================
    [HttpGet("vision/by-student/{studentId:long}")]
    public async Task<IActionResult> GetVisionHistory(long studentId)
    {
        var list = await _service.GetVisionHistoryAsync(studentId);
        return Ok(list);
    }

    [HttpGet("vision/latest/{studentId:long}")]
    public async Task<IActionResult> GetLatestVision(long studentId)
    {
        var item = await _service.GetLatestVisionAsync(studentId);
        if (item == null) return NotFound(new { message = "暂无视力档案" });
        return Ok(item);
    }

    [HttpPost("vision")]
    public async Task<IActionResult> CreateVision([FromBody] VisionRecordDto dto)
    {
        if (dto == null) return BadRequest(new { message = "请求体不能为空" });
        if (dto.StudentId <= 0) return BadRequest(new { message = "学生 ID 不能为空" });
        var created = await _service.CreateVisionRecordAsync(dto);
        return Ok(created);
    }

    [HttpDelete("vision/{id:long}")]
    public async Task<IActionResult> DeleteVision(long id)
    {
        var ok = await _service.DeleteVisionRecordAsync(id);
        if (!ok) return NotFound(new { message = "视力记录不存在" });
        return NoContent();
    }

    // ============================================================
    // 口腔健康
    // ============================================================
    [HttpGet("oral/by-student/{studentId:long}")]
    public async Task<IActionResult> GetOralHistory(long studentId)
    {
        var list = await _service.GetOralHistoryAsync(studentId);
        return Ok(list);
    }

    [HttpGet("oral/latest/{studentId:long}")]
    public async Task<IActionResult> GetLatestOral(long studentId)
    {
        var item = await _service.GetLatestOralAsync(studentId);
        if (item == null) return NotFound(new { message = "暂无口腔档案" });
        return Ok(item);
    }

    [HttpPost("oral")]
    public async Task<IActionResult> CreateOral([FromBody] OralRecordDto dto)
    {
        if (dto == null) return BadRequest(new { message = "请求体不能为空" });
        if (dto.StudentId <= 0) return BadRequest(new { message = "学生 ID 不能为空" });
        var created = await _service.CreateOralRecordAsync(dto);
        return Ok(created);
    }

    [HttpDelete("oral/{id:long}")]
    public async Task<IActionResult> DeleteOral(long id)
    {
        var ok = await _service.DeleteOralRecordAsync(id);
        if (!ok) return NotFound(new { message = "口腔记录不存在" });
        return NoContent();
    }

    // ============================================================
    // 心理健康
    // ============================================================
    [HttpGet("mental/by-student/{studentId:long}")]
    public async Task<IActionResult> GetMentalHistory(long studentId)
    {
        var list = await _service.GetMentalHistoryAsync(studentId);
        return Ok(list);
    }

    [HttpGet("mental/latest/{studentId:long}")]
    public async Task<IActionResult> GetLatestMental(long studentId)
    {
        var item = await _service.GetLatestMentalAsync(studentId);
        if (item == null) return NotFound(new { message = "暂无心理档案" });
        return Ok(item);
    }

    [HttpPost("mental")]
    public async Task<IActionResult> CreateMental([FromBody] MentalRecordDto dto)
    {
        if (dto == null) return BadRequest(new { message = "请求体不能为空" });
        if (dto.StudentId <= 0) return BadRequest(new { message = "学生 ID 不能为空" });
        var created = await _service.CreateMentalRecordAsync(dto);
        return Ok(created);
    }

    [HttpDelete("mental/{id:long}")]
    public async Task<IActionResult> DeleteMental(long id)
    {
        var ok = await _service.DeleteMentalRecordAsync(id);
        if (!ok) return NotFound(new { message = "心理记录不存在" });
        return NoContent();
    }

    // ============================================================
    // 健康体重
    // ============================================================
    [HttpGet("weight/by-student/{studentId:long}")]
    public async Task<IActionResult> GetWeightHistory(long studentId)
    {
        var list = await _service.GetWeightHistoryAsync(studentId);
        return Ok(list);
    }

    [HttpGet("weight/latest/{studentId:long}")]
    public async Task<IActionResult> GetLatestWeight(long studentId)
    {
        var item = await _service.GetLatestWeightAsync(studentId);
        if (item == null) return NotFound(new { message = "暂无体重档案" });
        return Ok(item);
    }

    [HttpPost("weight")]
    public async Task<IActionResult> CreateWeight([FromBody] WeightRecordDto dto)
    {
        if (dto == null) return BadRequest(new { message = "请求体不能为空" });
        if (dto.StudentId <= 0) return BadRequest(new { message = "学生 ID 不能为空" });
        var created = await _service.CreateWeightRecordAsync(dto);
        return Ok(created);
    }

    [HttpDelete("weight/{id:long}")]
    public async Task<IActionResult> DeleteWeight(long id)
    {
        var ok = await _service.DeleteWeightRecordAsync(id);
        if (!ok) return NotFound(new { message = "体重记录不存在" });
        return NoContent();
    }

    // ============================================================
    // 骨骼健康
    // ============================================================
    [HttpGet("bone/by-student/{studentId:long}")]
    public async Task<IActionResult> GetBoneHistory(long studentId)
    {
        var list = await _service.GetBoneHistoryAsync(studentId);
        return Ok(list);
    }

    [HttpGet("bone/latest/{studentId:long}")]
    public async Task<IActionResult> GetLatestBone(long studentId)
    {
        var item = await _service.GetLatestBoneAsync(studentId);
        if (item == null) return NotFound(new { message = "暂无骨骼档案" });
        return Ok(item);
    }

    [HttpPost("bone")]
    public async Task<IActionResult> CreateBone([FromBody] BoneRecordDto dto)
    {
        if (dto == null) return BadRequest(new { message = "请求体不能为空" });
        if (dto.StudentId <= 0) return BadRequest(new { message = "学生 ID 不能为空" });
        var created = await _service.CreateBoneRecordAsync(dto);
        return Ok(created);
    }

    [HttpDelete("bone/{id:long}")]
    public async Task<IActionResult> DeleteBone(long id)
    {
        var ok = await _service.DeleteBoneRecordAsync(id);
        if (!ok) return NotFound(new { message = "骨骼记录不存在" });
        return NoContent();
    }
}

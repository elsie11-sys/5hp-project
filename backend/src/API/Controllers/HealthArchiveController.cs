using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// 学生健康档案 - 5 维度 RESTful API
/// 路径前缀：/api/health-archive
///
/// 包含：分页查询、统计、状态变更（审核/驳回/抽检）、单个录入、批量导入（Excel 解析后 JSON 提交）、按学生查历史
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
    // 总体统计（5 维度汇总）
    // ============================================================
    [HttpGet("stats/overall")]
    public async Task<IActionResult> GetOverallStats()
    {
        var stats = await _service.GetOverallStatsAsync();
        return Ok(stats);
    }

    // ============================================================
    // 视力健康
    // ============================================================
    [HttpGet("vision/page")]
    public async Task<IActionResult> GetVisionPage([FromQuery] CollectQueryDto query)
        => Ok(await _service.GetVisionPagedAsync(Normalize(query)));

    [HttpGet("vision/stats")]
    public async Task<IActionResult> GetVisionStats() => Ok(await _service.GetVisionStatsAsync());

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
        if (string.IsNullOrEmpty(dto.StudentNo)) return BadRequest(new { message = "学号不能为空" });
        // StudentId 缺省由 Service 根据 studentNo 自动从 student_archive 表解析
        var created = await _service.CreateVisionRecordAsync(dto);
        return Ok(created);
    }

    [HttpPut("vision/{id:long}")]
    public async Task<IActionResult> UpdateVision(long id, [FromBody] VisionRecordDto dto)
    {
        if (dto == null) return BadRequest(new { message = "请求体不能为空" });
        var updated = await _service.UpdateVisionRecordAsync(id, dto);
        if (updated == null) return NotFound(new { message = "视力记录不存在" });
        return Ok(updated);
    }

    [HttpPost("vision/{id:long}/status")]
    public async Task<IActionResult> ChangeVisionStatus(long id, [FromBody] StatusChangeRequest req)
    {
        if (req == null) return BadRequest(new { message = "请求体不能为空" });
        var ok = await _service.ChangeVisionStatusAsync(id, req);
        if (!ok) return NotFound(new { message = "视力记录不存在" });
        return Ok(new { message = "状态已更新" });
    }

    [HttpDelete("vision/{id:long}")]
    public async Task<IActionResult> DeleteVision(long id)
    {
        var ok = await _service.DeleteVisionRecordAsync(id);
        if (!ok) return NotFound(new { message = "视力记录不存在" });
        return Ok(new { message = "删除成功" });
    }

    // ============================================================
    // 口腔健康
    // ============================================================
    [HttpGet("oral/page")]
    public async Task<IActionResult> GetOralPage([FromQuery] CollectQueryDto query)
        => Ok(await _service.GetOralPagedAsync(Normalize(query)));

    [HttpGet("oral/stats")]
    public async Task<IActionResult> GetOralStats() => Ok(await _service.GetOralStatsAsync());

    [HttpGet("oral/by-student/{studentId:long}")]
    public async Task<IActionResult> GetOralHistory(long studentId) => Ok(await _service.GetOralHistoryAsync(studentId));

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
        if (dto == null || string.IsNullOrEmpty(dto.StudentNo))
            return BadRequest(new { message = "学号不能为空" });
        return Ok(await _service.CreateOralRecordAsync(dto));
    }

    [HttpPut("oral/{id:long}")]
    public async Task<IActionResult> UpdateOral(long id, [FromBody] OralRecordDto dto)
    {
        if (dto == null) return BadRequest(new { message = "请求体不能为空" });
        var updated = await _service.UpdateOralRecordAsync(id, dto);
        if (updated == null) return NotFound(new { message = "口腔记录不存在" });
        return Ok(updated);
    }

    [HttpPost("oral/{id:long}/status")]
    public async Task<IActionResult> ChangeOralStatus(long id, [FromBody] StatusChangeRequest req)
    {
        if (req == null) return BadRequest(new { message = "请求体不能为空" });
        var ok = await _service.ChangeOralStatusAsync(id, req);
        if (!ok) return NotFound(new { message = "口腔记录不存在" });
        return Ok(new { message = "状态已更新" });
    }

    [HttpDelete("oral/{id:long}")]
    public async Task<IActionResult> DeleteOral(long id)
    {
        var ok = await _service.DeleteOralRecordAsync(id);
        if (!ok) return NotFound(new { message = "口腔记录不存在" });
        return Ok(new { message = "删除成功" });
    }

    // ============================================================
    // 心理健康
    // ============================================================
    [HttpGet("mental/page")]
    public async Task<IActionResult> GetMentalPage([FromQuery] CollectQueryDto query)
        => Ok(await _service.GetMentalPagedAsync(Normalize(query)));

    [HttpGet("mental/stats")]
    public async Task<IActionResult> GetMentalStats() => Ok(await _service.GetMentalStatsAsync());

    [HttpGet("mental/by-student/{studentId:long}")]
    public async Task<IActionResult> GetMentalHistory(long studentId) => Ok(await _service.GetMentalHistoryAsync(studentId));

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
        if (dto == null || string.IsNullOrEmpty(dto.StudentNo))
            return BadRequest(new { message = "学号不能为空" });
        return Ok(await _service.CreateMentalRecordAsync(dto));
    }

    [HttpPut("mental/{id:long}")]
    public async Task<IActionResult> UpdateMental(long id, [FromBody] MentalRecordDto dto)
    {
        if (dto == null) return BadRequest(new { message = "请求体不能为空" });
        var updated = await _service.UpdateMentalRecordAsync(id, dto);
        if (updated == null) return NotFound(new { message = "心理记录不存在" });
        return Ok(updated);
    }

    [HttpPost("mental/{id:long}/status")]
    public async Task<IActionResult> ChangeMentalStatus(long id, [FromBody] StatusChangeRequest req)
    {
        if (req == null) return BadRequest(new { message = "请求体不能为空" });
        var ok = await _service.ChangeMentalStatusAsync(id, req);
        if (!ok) return NotFound(new { message = "心理记录不存在" });
        return Ok(new { message = "状态已更新" });
    }

    [HttpDelete("mental/{id:long}")]
    public async Task<IActionResult> DeleteMental(long id)
    {
        var ok = await _service.DeleteMentalRecordAsync(id);
        if (!ok) return NotFound(new { message = "心理记录不存在" });
        return Ok(new { message = "删除成功" });
    }

    // ============================================================
    // 健康体重
    // ============================================================
    [HttpGet("weight/page")]
    public async Task<IActionResult> GetWeightPage([FromQuery] CollectQueryDto query)
        => Ok(await _service.GetWeightPagedAsync(Normalize(query)));

    [HttpGet("weight/stats")]
    public async Task<IActionResult> GetWeightStats() => Ok(await _service.GetWeightStatsAsync());

    [HttpGet("weight/by-student/{studentId:long}")]
    public async Task<IActionResult> GetWeightHistory(long studentId) => Ok(await _service.GetWeightHistoryAsync(studentId));

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
        if (dto == null || string.IsNullOrEmpty(dto.StudentNo))
            return BadRequest(new { message = "学号不能为空" });
        return Ok(await _service.CreateWeightRecordAsync(dto));
    }

    [HttpPut("weight/{id:long}")]
    public async Task<IActionResult> UpdateWeight(long id, [FromBody] WeightRecordDto dto)
    {
        if (dto == null) return BadRequest(new { message = "请求体不能为空" });
        var updated = await _service.UpdateWeightRecordAsync(id, dto);
        if (updated == null) return NotFound(new { message = "体重记录不存在" });
        return Ok(updated);
    }

    [HttpPost("weight/{id:long}/status")]
    public async Task<IActionResult> ChangeWeightStatus(long id, [FromBody] StatusChangeRequest req)
    {
        if (req == null) return BadRequest(new { message = "请求体不能为空" });
        var ok = await _service.ChangeWeightStatusAsync(id, req);
        if (!ok) return NotFound(new { message = "体重记录不存在" });
        return Ok(new { message = "状态已更新" });
    }

    [HttpDelete("weight/{id:long}")]
    public async Task<IActionResult> DeleteWeight(long id)
    {
        var ok = await _service.DeleteWeightRecordAsync(id);
        if (!ok) return NotFound(new { message = "体重记录不存在" });
        return Ok(new { message = "删除成功" });
    }

    // ============================================================
    // 骨骼健康
    // ============================================================
    [HttpGet("bone/page")]
    public async Task<IActionResult> GetBonePage([FromQuery] CollectQueryDto query)
        => Ok(await _service.GetBonePagedAsync(Normalize(query)));

    [HttpGet("bone/stats")]
    public async Task<IActionResult> GetBoneStats() => Ok(await _service.GetBoneStatsAsync());

    [HttpGet("bone/by-student/{studentId:long}")]
    public async Task<IActionResult> GetBoneHistory(long studentId) => Ok(await _service.GetBoneHistoryAsync(studentId));

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
        if (dto == null || string.IsNullOrEmpty(dto.StudentNo))
            return BadRequest(new { message = "学号不能为空" });
        return Ok(await _service.CreateBoneRecordAsync(dto));
    }

    [HttpPut("bone/{id:long}")]
    public async Task<IActionResult> UpdateBone(long id, [FromBody] BoneRecordDto dto)
    {
        if (dto == null) return BadRequest(new { message = "请求体不能为空" });
        var updated = await _service.UpdateBoneRecordAsync(id, dto);
        if (updated == null) return NotFound(new { message = "骨骼记录不存在" });
        return Ok(updated);
    }

    [HttpPost("bone/{id:long}/status")]
    public async Task<IActionResult> ChangeBoneStatus(long id, [FromBody] StatusChangeRequest req)
    {
        if (req == null) return BadRequest(new { message = "请求体不能为空" });
        var ok = await _service.ChangeBoneStatusAsync(id, req);
        if (!ok) return NotFound(new { message = "骨骼记录不存在" });
        return Ok(new { message = "状态已更新" });
    }

    [HttpDelete("bone/{id:long}")]
    public async Task<IActionResult> DeleteBone(long id)
    {
        var ok = await _service.DeleteBoneRecordAsync(id);
        if (!ok) return NotFound(new { message = "骨骼记录不存在" });
        return Ok(new { message = "删除成功" });
    }

    // 规范化查询参数
    private static CollectQueryDto Normalize(CollectQueryDto q)
    {
        if (q.Page < 1) q.Page = 1;
        if (q.PageSize < 1) q.PageSize = 10;
        if (q.PageSize > 200) q.PageSize = 200;
        if (string.IsNullOrWhiteSpace(q.Status)) q.Status = "all";
        return q;
    }
}

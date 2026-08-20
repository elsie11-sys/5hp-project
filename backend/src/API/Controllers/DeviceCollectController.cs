using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// 设备自动采集专用接口
/// 路径前缀：/api/device/v1
/// 鉴权方式：请求头 X-Device-Key，与 appsettings.json 的 Device:ApiKey 匹配
///
/// 设备厂商对接说明：
///   1. 设备启动后 POST /api/device/v1/{dimension}/ingest
///   2. Header: X-Device-Key: <从设备管理后台下发的 Key>
///   3. Body: DeviceIngestRequest&lt;T&gt; (设备 SN + 采集时间 + 录入人 + 数据列表)
///   4. 返回 { total, success, failed, errors: [...] }
///
/// 5 维度的 T 字段说明（每条记录都至少包含学号 + 姓名 + 班级 + 检查日期 + 业务字段）：
///   - vision:   leftEye, rightEye, visionLevel
///   - oral:     decayedBabyTeeth, decayedPermanentTeeth, toothStage, jawDevelopment
///   - mental:   anxietyScore, depressionScore, learningAnxiety, interpersonalSensitivity
///   - weight:   height, weight, bmi, bmiLevel, waistCircumference, hipCircumference, whr
///   - bone:     boneDensity, boneLevel, boneAge, vitaminD, calciumLevel
/// </summary>
[ApiController]
[Route("api/device/v1")]
public class DeviceCollectController : ControllerBase
{
    private readonly IHealthArchiveService _service;
    private readonly IConfiguration _config;

    private const string HeaderKey = "X-Device-Key";

    public DeviceCollectController(IHealthArchiveService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    private bool Authorize()
    {
        var expected = _config["Device:ApiKey"] ?? "5hp-device-default-key";
        if (!Request.Headers.TryGetValue(HeaderKey, out var v)) return false;
        return string.Equals(v.ToString(), expected, StringComparison.Ordinal);
    }

    private IActionResult? AuthGuard()
    {
        if (!Authorize())
            return Unauthorized(new { code = 401, message = "设备 Key 无效或缺失" });
        return null;
    }

    [HttpPost("vision/ingest")]
    public async Task<IActionResult> IngestVision([FromBody] DeviceIngestRequest<VisionRecordDto> req)
    {
        var auth = AuthGuard();
        if (auth != null) return auth;
        if (req == null || string.IsNullOrEmpty(req.DeviceSn))
            return BadRequest(new { code = 400, message = "设备 SN 不能为空" });
        var resp = await _service.DeviceIngestVisionAsync(req);
        return Ok(resp);
    }

    [HttpPost("oral/ingest")]
    public async Task<IActionResult> IngestOral([FromBody] DeviceIngestRequest<OralRecordDto> req)
    {
        var auth = AuthGuard();
        if (auth != null) return auth;
        if (req == null || string.IsNullOrEmpty(req.DeviceSn))
            return BadRequest(new { code = 400, message = "设备 SN 不能为空" });
        return Ok(await _service.DeviceIngestOralAsync(req));
    }

    [HttpPost("mental/ingest")]
    public async Task<IActionResult> IngestMental([FromBody] DeviceIngestRequest<MentalRecordDto> req)
    {
        var auth = AuthGuard();
        if (auth != null) return auth;
        if (req == null || string.IsNullOrEmpty(req.DeviceSn))
            return BadRequest(new { code = 400, message = "设备 SN 不能为空" });
        return Ok(await _service.DeviceIngestMentalAsync(req));
    }

    [HttpPost("weight/ingest")]
    public async Task<IActionResult> IngestWeight([FromBody] DeviceIngestRequest<WeightRecordDto> req)
    {
        var auth = AuthGuard();
        if (auth != null) return auth;
        if (req == null || string.IsNullOrEmpty(req.DeviceSn))
            return BadRequest(new { code = 400, message = "设备 SN 不能为空" });
        return Ok(await _service.DeviceIngestWeightAsync(req));
    }

    [HttpPost("bone/ingest")]
    public async Task<IActionResult> IngestBone([FromBody] DeviceIngestRequest<BoneRecordDto> req)
    {
        var auth = AuthGuard();
        if (auth != null) return auth;
        if (req == null || string.IsNullOrEmpty(req.DeviceSn))
            return BadRequest(new { code = 400, message = "设备 SN 不能为空" });
        return Ok(await _service.DeviceIngestBoneAsync(req));
    }

    // 健康检查（设备用于探活，可不鉴权）
    [HttpGet("ping")]
    public IActionResult Ping() => Ok(new
    {
        code = 0,
        message = "ok",
        serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        dimensions = new[] { "vision", "oral", "mental", "weight", "bone" }
    });
}

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// 通用文件上传接口
/// 路径前缀：/api/upload
/// </summary>
[ApiController]
[Route("api/upload")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadController> _logger;

    /// <summary>允许的图片 MIME 类型</summary>
    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/bmp"
    };

    /// <summary>单文件最大 2MB（与前端校验保持一致）</summary>
    private const long MaxFileSize = 2 * 1024 * 1024;

    public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
    {
        _env = env;
        _logger = logger;
    }

    /// <summary>
    /// 上传头像
    /// POST /api/upload/avatar  (multipart/form-data, field = "file")
    /// 返回 { url, fileName, size }：url 是可直接访问的相对路径，例如 /uploads/avatars/xxx.png
    /// </summary>
    [HttpPost("avatar")]
    [RequestSizeLimit(4 * 1024 * 1024)] // 给一点 buffer 给 form 头
    public async Task<IActionResult> UploadAvatar(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "请选择要上传的图片" });
        }
        if (file.Length > MaxFileSize)
        {
            return BadRequest(new { message = $"图片大小不能超过 {MaxFileSize / 1024 / 1024}MB" });
        }
        if (!AllowedImageTypes.Contains(file.ContentType))
        {
            return BadRequest(new { message = "仅支持 JPG/PNG/GIF/WEBP/BMP 格式图片" });
        }

        try
        {
            // wwwroot 可能不存在（首次运行时），兜底到 ContentRoot/wwwroot
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var uploadsDir = Path.Combine(webRoot, "uploads", "avatars");
            Directory.CreateDirectory(uploadsDir);

            // 文件名：yyyyMMddHHmmss_8位随机串.扩展名，避免冲突
            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext)) ext = ".png";
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            // 返回相对 URL，前端拼接 baseURL 即可访问
            var url = $"/uploads/avatars/{fileName}";
            return Ok(new { url, fileName, size = file.Length });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "上传头像失败");
            return StatusCode(500, new { message = "上传失败：" + ex.Message });
        }
    }
}

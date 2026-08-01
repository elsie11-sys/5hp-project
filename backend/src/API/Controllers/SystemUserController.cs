using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using System.Text;

namespace API.Controllers;

// 1. 将 [controller] 改为固定的 "user"，以匹配前端 /api/user/... 的请求
[ApiController]
[Route("api/user")]
public class SystemUserController : ControllerBase
{
    private readonly IUserService _userService;

    public SystemUserController(IUserService userService)
    {
        _userService = userService;
    }

    // ================= 适配前端框架的专属接口 =================

    // GET: api/user/info
    // 获取当前登录用户信息（Vben Admin 登录时必调接口）
       [HttpGet("info")]
    public IActionResult GetUserInfo()
    {
        var mockUserInfo = new
        {
            userId = 1,             // Vben 通常识别 userId 而不是 id
            username = "admin",
            realName = "超级管理员",
            avatar = "https://unavatar.io/github", // 补上头像字段
            desc = "拥有所有权限",
            roles = new[] {         // 角色通常要求是对象数组
                new { roleName = "超级管理员", value = "admin" }
            }
        };

        return Ok(mockUserInfo);
    }
    // ================= 原有的 CRUD 业务接口 =================

    // GET: api/user/paged?page=1&pageSize=10&keyword=xxx
    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null,
        [FromQuery] string? orgIds = null)
    {
        var result = await _userService.GetPagedUsersAsync(page, pageSize, keyword, orgIds);
        return Ok(new { items = result.Items, total = result.Total });
    }

    // GET: api/user
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // GET: api/user/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(long id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "用户不存在" });
        }
        return Ok(user);
    }

    // POST: api/user
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdUser = await _userService.CreateUserAsync(userDto);

        // 返回 201 Created，并带上数据
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    // PUT: api/user/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(long id, [FromBody] UserDto userDto)
    {
        if (id != userDto.Id)
        {
            return BadRequest(new { message = "URL中的ID与请求体中的ID不匹配" });
        }

        try
        {
            var updatedUser = await _userService.UpdateUserAsync(userDto);
            return Ok(updatedUser);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // DELETE: api/user/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success)
        {
            return NotFound(new { message = "用户不存在，删除失败" });
        }

        return NoContent();
    }

    // ================= 完善功能 =================

    // POST: api/user/{id}/reset-password  body: { "newPassword": "123456" }
    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(long id, [FromBody] ResetPasswordRequest? body)
    {
        var newPwd = string.IsNullOrWhiteSpace(body?.NewPassword) ? "123456" : body!.NewPassword!;
        var ok = await _userService.ResetPasswordAsync(id, newPwd);
        if (!ok) return NotFound(new { message = "用户不存在" });
        return Ok(new { message = $"已重置用户 {id} 的密码" });
    }

    // POST: api/user/batch-delete  body: { "ids": [1,2,3] }
    [HttpPost("batch-delete")]
    public async Task<IActionResult> BatchDelete([FromBody] BatchDeleteRequest request)
    {
        if (request?.Ids == null || request.Ids.Count == 0)
        {
            return BadRequest(new { message = "请选择要删除的用户" });
        }
        var count = await _userService.BatchDeleteAsync(request.Ids);
        return Ok(new { message = $"已删除 {count} 个用户", deletedCount = count });
    }

    // GET: api/user/template   —— 下载 Excel 导入模板
    // GET: api/user/export?keyword=xxx&orgIds=1,2,3
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] string? keyword = null, [FromQuery] string? orgIds = null)
    {
        var items = await _userService.ExportUsersAsync(keyword, orgIds);

        var rows = items.Select(u => new
        {
            用户编号 = u.Id,
            账号     = u.Username,
            姓名     = u.RealName,
            性别     = u.Gender == 1 ? "男" : u.Gender == 2 ? "女" : "未知",
            角色     = u.RoleId,
            机构ID   = u.OrgId,
            手机号码 = u.PhoneNumber,
            邮箱     = u.Email,
            状态     = u.Status == 1 ? "启用" : "禁用"
        });

        using var ms = new MemoryStream();
        await MiniExcel.SaveAsAsync(ms, rows);
        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"用户导出_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }

    [HttpGet("template")]
    public async Task<IActionResult> DownloadTemplate()
    {
        var headers = new[]
        {
            new
            {
                用户名     = "示例：张三",
                姓名       = "示例：张三",
                性别       = "男 / 女 / 未知",
                角色       = "国家管理员 / 省级管理员 / 市级管理员 / 县级管理员 / 学校管理员 / 校医 / 班主任",
                所属机构   = "国家教育部 / 江苏省教育厅 / 浙江省教育厅 / 南京市教育局 / 苏州市教育局 / 鼓楼区教育局 / 南京市第一中学 / 南京市金陵中学",
                手机号     = "13800138000",
                邮箱       = "zhangsan@example.com",
                密码       = "123456",
                状态       = "启用 / 禁用",
            }
        };

        // MiniExcel 1.35 没有 SaveAsBytesAsync，改为写到 MemoryStream
        using var ms = new MemoryStream();
        await MiniExcel.SaveAsAsync(ms, headers);
        var bytes = ms.ToArray();

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"用户导入模板_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    // POST: api/user/import   —— 接收 .xlsx / .xls 文件
    [HttpPost("import")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB
    public async Task<IActionResult> ImportUsers(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "请选择要上传的 Excel 文件" });
        }

        // 文件名 / 扩展名校验
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".xlsx" && ext != ".xls" && ext != ".csv")
        {
            return BadRequest(new { message = "仅支持 .xlsx / .xls / .csv 格式" });
        }

        List<UserImportRow> rows;
        try
        {
            // 把上传文件读到内存里
            using var input = new MemoryStream();
            await file.CopyToAsync(input);
            var bytes = input.ToArray();

            if (ext == ".csv")
            {
                // CSV 自己解析（MiniExcel 的 CSV reader 在中文 Windows 上有编码 bug）
                rows = ParseCsv(bytes);
            }
            else
            {
                // xlsx / xls 走 MiniExcel
                using var stream = new MemoryStream(bytes);
                var raw = await stream.QueryAsync();
                rows = new List<UserImportRow>();
                foreach (var item in raw)
                {
                    if (item is IDictionary<string, object> dict)
                    {
                        rows.Add(MapDictToRow(dict));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"解析 Excel 失败：{ex.Message}" });
        }

        if (rows.Count == 0)
        {
            return BadRequest(new { message = "Excel 中没有数据行" });
        }

        var result = await _userService.BulkCreateAsync(rows);
        return Ok(result);
    }

    /// <summary>
    /// 从 dynamic 行里取字符串字段，依次尝试多个候选列名
    /// </summary>
    private static string? GetStr(IDictionary<string, object> dict, params string[] keys)
    {
        foreach (var k in keys)
        {
            if (dict.TryGetValue(k, out var v) && v != null)
            {
                var s = v.ToString();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            }
        }
        return null;
    }

    /// <summary>
    /// 把 dynamic 字典（xlsx 路径）映射成 UserImportRow
    /// </summary>
    private static UserImportRow MapDictToRow(IDictionary<string, object> dict) => new()
    {
        Username    = GetStr(dict, "用户名", "username", "Username"),
        RealName    = GetStr(dict, "姓名", "realName", "RealName"),
        Gender      = GetStr(dict, "性别", "gender", "Gender"),
        Role        = GetStr(dict, "角色", "role", "Role"),
        Org         = GetStr(dict, "所属机构", "org", "Org"),
        PhoneNumber = GetStr(dict, "手机号", "phoneNumber", "PhoneNumber"),
        Email       = GetStr(dict, "邮箱", "email", "Email"),
        Password    = GetStr(dict, "密码", "password", "Password"),
        Status      = GetStr(dict, "状态", "status", "Status"),
    };

    /// <summary>
    /// 解析 CSV（自己实现，处理 UTF-8 BOM + 简单 CSV 格式）
    /// 不依赖 MiniExcel，避开其 CSV reader 的 GBK 编码 bug
    /// </summary>
    private static List<UserImportRow> ParseCsv(byte[] bytes)
    {
        // 去掉 UTF-8 BOM（如果有）
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            bytes = bytes.Skip(3).ToArray();
        }
        var text = Encoding.UTF8.GetString(bytes);
        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .ToList();
        if (lines.Count < 2) return new List<UserImportRow>();

        // 第一行是表头
        var headers = SplitCsvLine(lines[0]);
        var rows = new List<UserImportRow>();
        for (int i = 1; i < lines.Count; i++)
        {
            var fields = SplitCsvLine(lines[i]);
            var dict = new Dictionary<string, object>();
            for (int j = 0; j < headers.Count && j < fields.Count; j++)
            {
                dict[headers[j].Trim()] = fields[j];
            }
            rows.Add(MapDictToRow(dict));
        }
        return rows;
    }

    /// <summary>
    /// 简单 CSV 字段拆分（支持双引号包裹的字段）
    /// </summary>
    private static List<string> SplitCsvLine(string line)
    {
        var result = new List<string>();
        var sb = new System.Text.StringBuilder();
        bool inQuote = false;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (inQuote)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                    else inQuote = false;
                }
                else sb.Append(c);
            }
            else
            {
                if (c == ',') { result.Add(sb.ToString()); sb.Clear(); }
                else if (c == '"' && sb.Length == 0) inQuote = true;
                else sb.Append(c);
            }
        }
        result.Add(sb.ToString());
        return result;
    }
}

public class ResetPasswordRequest
{
    public string? NewPassword { get; set; }
}

using System.Diagnostics;
using System.Text;

using Application.DTOs;
using Application.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

/// <summary>
/// 全局操作日志过滤器：自动把每个请求的概要写进 sys_operation_log
/// <para>
/// 设计要点：
/// 1) 用 IAsyncActionFilter 包住 next()，才能计时 + 读取响应
/// 2) operator 从 Header <c>X-Operator</c> 读（前端 requestClient 拦截器自动塞），
///    没传则兜底 "anonymous"；后续接入 JWT 时改成从 ClaimTypes.Name 取
/// 3) type 按 HTTP method 推断：GET→query / POST→create / PUT→update / DELETE→delete
/// 4) module 从 Controller 名 + Action 名组合（如 "SysRoleController.GetPagedRoles" → "角色管理 / 列表"）
/// 5) 自身日志接口 / Swagger / favicon 等静态资源直接跳过，避免无限递归和噪音
/// 6) 写日志用 Task.Run 异步执行 + 在任务里开新 DI scope（关键！否则请求结束后
///    Scoped 的 DbContext 已被 dispose，背景任务里的 _context.SaveChanges 必失败）
/// </para>
/// </summary>
public class OperationLogFilter : IAsyncActionFilter
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OperationLogFilter> _logger;

    public OperationLogFilter(IServiceScopeFactory scopeFactory, ILogger<OperationLogFilter> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var http = context.HttpContext;
        var path = http.Request.Path.Value ?? string.Empty;

        // 1) 跳过自身日志接口（写日志的接口不能再写日志，否则会无限递归）
        //    跳过 Swagger / 健康检查 / 静态资源
        if (ShouldSkip(path))
        {
            await next();
            return;
        }

        // 2) 取请求元数据
        var method = http.Request.Method;
        var ip = GetClientIp(http);
        var userAgent = http.Request.Headers.UserAgent.ToString();
        var url = path + http.Request.QueryString.Value;
        var actionDesc = GetControllerActionName(context);

        // 3) 请求参数（query + 路由 + body 摘要；body 太大可能影响性能，先只读 query/route）
        var requestParams = BuildRequestParams(context, http);

        var sw = Stopwatch.StartNew();
        Exception? thrown = null;
        int statusCode = 200;
        string? responseSummary = null;

        try
        {
            var executed = await next();
            sw.Stop();

            if (executed.Exception != null && !executed.ExceptionHandled)
            {
                thrown = executed.Exception;
                statusCode = 500;
            }
            else
            {
                statusCode = http.Response.StatusCode;
                // 响应内容只取前 500 字，避免日志炸
                responseSummary = TryReadResponseSummary(executed.Result, http);
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            thrown = ex;
            statusCode = 500;
            throw; // 继续抛给 GlobalExceptionFilter
        }
        finally
        {
            // 4) 写日志（不要 await 主流程太久，后台 fire-and-forget 也行；
            //    但失败要 try/catch 兜住，不能让日志写失败把正常请求搞挂）
            try
            {
                var operatorName = ResolveOperator(http, thrown);
                var isSuccess = thrown == null && statusCode < 400;

                // biz_data 取值策略：
                //   1) Controller 通过 HttpContext.Items["BizData"] 注入了 → 优先用它的
                //   2) 否则由 Filter 自动拼一个默认摘要 JSON
                var bizData = ResolveBizData(http, context, actionDesc, requestParams, responseSummary, isSuccess);

                var log = new OperationLogCreateRequest
                {
                    Operator = operatorName,
                    Type = InferType(method),
                    Module = actionDesc.controller,
                    Content = $"{actionDesc.controller} / {actionDesc.action}",
                    Ip = ip,
                    Status = isSuccess ? 1 : 0,
                    CostMs = (int)sw.ElapsedMilliseconds,
                    RequestUrl = url,
                    RequestMethod = method,
                    Method = $"{context.Controller.GetType().FullName}.{context.ActionDescriptor.DisplayName}",
                    RequestParams = Truncate(requestParams, 2000),
                    ResponseParams = Truncate(responseSummary, 2000),
                    ErrorMsg = thrown?.Message,
                    UserAgent = Truncate(userAgent, 500),
                    Location = null, // IP 归属地需要第三方库，先不实现
                    BizData = Truncate(bizData, 4000),
                };

                // 不 await 主流程：写日志失败不能拖垮请求
                // 关键：在后台任务里开一个新的 DI scope，否则请求结束后 Scoped 的
                // IApplicationDbContext 已 dispose，_logService.CreateAsync 会抛
                // "Cannot access a disposed object" 被静默吞掉
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var logService = scope.ServiceProvider.GetRequiredService<IOperationLogService>();
                        await logService.CreateAsync(log);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "[OperationLogFilter] 写操作日志失败: {Path}", path);
                    }
                });
            }
            catch (Exception ex)
            {
                // 再兜一层：构建日志对象本身出错也不能影响主请求
                _logger.LogWarning(ex, "[OperationLogFilter] 构造日志失败: {Path}", path);
            }
        }
    }

    // ============== 辅助方法 ==============

    /// <summary>需要跳过日志的路径（自身接口 + Swagger + 静态资源 + 健康检查）</summary>
    private static bool ShouldSkip(string path)
    {
        if (string.IsNullOrEmpty(path)) return true;
        if (path.StartsWith("/api/system/log/operation", StringComparison.OrdinalIgnoreCase)) return true;
        if (path.StartsWith("/api/system/log/login", StringComparison.OrdinalIgnoreCase)) return true;
        if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)) return true;
        if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase)) return true;
        if (path.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase)) return true;
        // 静态资源（.js .css .png .ico .map 等）
        var ext = Path.GetExtension(path);
        if (!string.IsNullOrEmpty(ext) && ext.Length <= 5 &&
            (ext.StartsWith(".js") || ext.StartsWith(".css") || ext.StartsWith(".png") ||
             ext.StartsWith(".jpg") || ext.StartsWith(".ico") || ext.StartsWith(".svg") ||
             ext.StartsWith(".woff") || ext.StartsWith(".map") || ext.StartsWith(".html")))
        {
            return true;
        }
        return false;
    }

    /// <summary>从 Controller / Action 描述里提取一个友好名称</summary>
    private static (string controller, string action) GetControllerActionName(ActionExecutingContext context)
    {
        string controller = "未知模块";
        string action = "未知操作";

        if (context.ActionDescriptor is ControllerActionDescriptor cad)
        {
            // Controller 名 → 中文章段名
            controller = cad.ControllerName switch
            {
                "SysUser" => "用户管理",
                "SysRole" => "角色管理",
                "SysMenu" => "菜单管理",
                "SysOrg" => "组织管理",
                "SysDict" => "字典管理",
                "SysOperationLog" => "操作日志",
                "HealthArchive" => "健康档案",
                "Student" => "学生档案",
                _ => cad.ControllerName,
            };

            // Action 名 → 友好描述
            action = cad.ActionName switch
            {
                "GetPaged" or "GetPagedRoles" or "GetPagedList" or "GetPagedAsync" => "列表",
                "GetById" or "GetRoleById" or "GetUserById" => "详情",
                "Create" or "CreateUser" or "CreateRole" => "新增",
                "Update" or "UpdateUser" or "UpdateRole" => "修改",
                "Delete" or "DeleteUser" or "DeleteRole" => "删除",
                "BatchDelete" => "批量删除",
                "Clear" => "清空",
                "Export" => "导出",
                "Login" => "登录",
                "Logout" => "登出",
                "ResetPassword" => "重置密码",
                "Import" or "ImportData" => "导入",
                "AssignMenuPermission" or "GetMenuPermission" => "权限分配",
                "GetDataPermission" or "AssignDataPermission" => "数据权限",
                _ => cad.ActionName,
            };
        }

        return (controller, action);
    }

    /// <summary>根据 HTTP method 推断操作类型（与前端 LOG_TYPE_MAP 对齐）</summary>
    private static string InferType(string method) => method.ToUpperInvariant() switch
    {
        "GET" => "query",
        "POST" => "create",
        "PUT" => "update",
        "DELETE" => "delete",
        "PATCH" => "update",
        _ => "other",
    };

    /// <summary>取操作人：优先 X-Operator Header，没传兜底 "anonymous"</summary>
    private static string ResolveOperator(HttpContext http, Exception? ex)
    {
        var fromHeader = http.Request.Headers["X-Operator"].ToString();
        if (!string.IsNullOrWhiteSpace(fromHeader)) return fromHeader.Trim();
        return "anonymous";
    }

    /// <summary>取客户端 IP（支持 X-Forwarded-For 反向代理场景）</summary>
    private static string? GetClientIp(HttpContext http)
    {
        var xff = http.Request.Headers["X-Forwarded-For"].ToString();
        if (!string.IsNullOrWhiteSpace(xff))
        {
            // XFF 可能是一串：client, proxy1, proxy2，取第一个
            var first = xff.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                           .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(first)) return first;
        }
        return http.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>组装请求参数摘要（query string + route values）</summary>
    private static string BuildRequestParams(ActionExecutingContext context, HttpContext http)
    {
        var parts = new List<string>();
        if (http.Request.Query.Count > 0)
        {
            parts.Add("query=" + string.Join("&", http.Request.Query.Select(kv =>
                $"{kv.Key}={kv.Value}")));
        }
        if (context.ActionArguments != null && context.ActionArguments.Count > 0)
        {
            // 路由参数（如 {id}）和 body（[FromBody]）
            foreach (var (k, v) in context.ActionArguments)
            {
                if (v == null) continue;
                var json = SafeSerialize(v);
                if (json.Length > 200) json = json[..200] + "…";
                parts.Add($"{k}={json}");
            }
        }
        return string.Join(" | ", parts);
    }

    /// <summary>读取响应摘要（取最多 500 字 JSON）</summary>
    private static string? TryReadResponseSummary(IActionResult? result, HttpContext http)
    {
        if (result is not ObjectResult objectResult) return null;
        if (objectResult.Value == null) return null;
        try
        {
            var json = SafeSerialize(objectResult.Value);
            return json.Length > 500 ? json[..500] + "…" : json;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 解析 biz_data：Controller 显式注入优先；否则 Filter 自动汇总一份
    /// </summary>
    private static string? ResolveBizData(
        HttpContext http,
        ActionExecutingContext context,
        (string controller, string action) actionDesc,
        string? requestParams,
        string? responseSummary,
        bool isSuccess)
    {
        // 1) Controller 主动注入（HttpContext.Items["BizData"]）
        if (http.Items.TryGetValue("BizData", out var raw) && raw != null)
        {
            return raw switch
            {
                string s => s,
                _ => SafeSerialize(raw),
            };
        }

        // 2) 默认汇总：让 biz_data 即使没业务上下文也有内容，方便排查
        var summary = new Dictionary<string, object?>
        {
            ["controller"] = actionDesc.controller,
            ["action"] = actionDesc.action,
            ["result"] = isSuccess ? "success" : "failed",
            ["argCount"] = context.ActionArguments?.Count ?? 0,
        };
        // 参数有内容时塞一份短的进 biz_data（避免 requestParams 重复）
        if (!string.IsNullOrEmpty(requestParams))
        {
            var shortArgs = requestParams.Length > 200 ? requestParams[..200] + "…" : requestParams;
            summary["args"] = shortArgs;
        }
        // 响应类型提示：paged / single / void
        if (!string.IsNullOrEmpty(responseSummary))
        {
            summary["responseType"] = ClassifyResponse(responseSummary);
        }
        return SafeSerialize(summary);
    }

    /// <summary>从响应 JSON 粗略判断"返回的是分页/单条/列表/空"等</summary>
    private static string ClassifyResponse(string responseJson)
    {
        if (responseJson.Contains("\"items\"") && responseJson.Contains("\"total\"")) return "paged";
        if (responseJson.Contains("\"items\"")) return "list";
        if (responseJson.StartsWith("{") && responseJson.EndsWith("}")) return "single";
        if (responseJson.StartsWith("[") && responseJson.EndsWith("]")) return "array";
        return "other";
    }

    /// <summary>安全序列化为 JSON（避免循环引用）</summary>
    private static string SafeSerialize(object value)
    {
        try
        {
            return System.Text.Json.JsonSerializer.Serialize(value, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            });
        }
        catch
        {
            return value.ToString() ?? string.Empty;
        }
    }

    private static string Truncate(string? s, int maxLen)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        return s.Length > maxLen ? s[..maxLen] + "…" : s;
    }
}

using System.Text.Json;

namespace API.Filters;

/// <summary>
/// Controller 操作日志上下文辅助
/// <para>
/// 在 Action 过程中调用本类的 <see cref="SetBizData"/>，
/// OperationLogFilter 会在响应结束后自动把 <c>biz_data</c> 字段写入 sys_operation_log。
/// </para>
/// <para>
/// 用法示例（角色删除）：
/// <code>
///   OperationLogContext.SetBizData(HttpContext, new {
///       roleId = id,
///       roleName = role.Name,
///       action = "delete"
///   });
/// </code>
/// </para>
/// </summary>
public static class OperationLogContext
{
    /// <summary>HttpContext.Items 里用的 key</summary>
    public const string BizDataKey = "BizData";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>
    /// 写入业务上下文（会被 Filter 序列化为 JSON 存到 sys_operation_log.biz_data）
    /// </summary>
    public static void SetBizData(HttpContext httpContext, object bizData)
    {
        if (httpContext == null || bizData == null) return;
        try
        {
            httpContext.Items[BizDataKey] = JsonSerializer.Serialize(bizData, JsonOptions);
        }
        catch
        {
            // 序列化失败就塞 ToString()，不让主流程挂
            httpContext.Items[BizDataKey] = bizData.ToString();
        }
    }

    /// <summary>
    /// 写入业务上下文（字符串形式，避免重复序列化）
    /// </summary>
    public static void SetBizDataJson(HttpContext httpContext, string bizDataJson)
    {
        if (httpContext == null || string.IsNullOrEmpty(bizDataJson)) return;
        httpContext.Items[BizDataKey] = bizDataJson;
    }

    /// <summary>
    /// 追加一个键值（多次调用会合并到同一个对象）
    /// </summary>
    public static void AddBizDataField(HttpContext httpContext, string key, object? value)
    {
        if (httpContext == null || string.IsNullOrEmpty(key)) return;

        Dictionary<string, object?>? dict = null;
        if (httpContext.Items.TryGetValue(BizDataKey, out var existing) && existing is string s)
        {
            try { dict = JsonSerializer.Deserialize<Dictionary<string, object?>>(s, JsonOptions); }
            catch { dict = new(); }
        }
        dict ??= new();
        dict[key] = value;
        httpContext.Items[BizDataKey] = JsonSerializer.Serialize(dict, JsonOptions);
    }
}

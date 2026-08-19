using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.DTOs;

/// <summary>
/// DateTime? 的 JSON 序列化器：输出 <c>yyyy-MM-dd HH:mm:ss</c>（不带毫秒、不带时区后缀）
/// <para>
/// 默认 <c>System.Text.Json</c> 用 "O" 格式输出 DateTime，会把亚秒精度和时区都带上，
/// 比如 <c>2026-08-18T16:10:11.0049420+08:00</c>。操作日志场景下用不到这个精度，
/// 截到秒既符合业务习惯（操作日志一般按分钟看），又少占带宽。
/// </para>
/// <para>
/// 用法：<c>[JsonConverter(typeof(DateTimeTextConverter))]</c> on the DTO property</para>
/// </summary>
public class DateTimeTextConverter : JsonConverter<DateTime?>
{
    private const string Format = "yyyy-MM-dd HH:mm:ss";

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s)) return null;
            // 兼容前端传 ISO 字符串（带 T / 时区）和 "yyyy-MM-dd HH:mm:ss" 两种格式
            if (DateTime.TryParse(s, out var dt)) return dt;
            return null;
        }
        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }
        // Kind=Unspecified 直接当本地时间输出；
        // Kind=Utc / Local 都先转本地再格式化，避免出现 +08:00 / Z 后缀
        var local = value.Value.Kind == DateTimeKind.Utc
            ? value.Value.ToLocalTime()
            : value.Value;
        writer.WriteStringValue(local.ToString(Format));
    }
}

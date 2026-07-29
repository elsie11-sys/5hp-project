using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

/// <summary>
/// 统一响应包装器：把控制器直接 Ok()/NotFound() 的结果包成
/// { code: 0, data, message } 格式，匹配前端的 Vben request 拦截器期望。
/// </summary>
public class ResultWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // 1) 异常已经被 GlobalExceptionFilter 吃掉，这里只处理成功 + 4xx 业务错误
        if (context.Result is ObjectResult objectResult)
        {
            var statusCode = objectResult.StatusCode ?? 200;

            // 业务错误（4xx）也包成统一格式
            if (statusCode >= 400)
            {
                // 尝试从 value 里拿 message 字段，没有就用 statusCode 文案
                string errMsg = statusCode switch
                {
                    400 => "请求参数错误",
                    404 => "资源不存在",
                    401 => "未授权",
                    403 => "无权限",
                    _   => "请求失败"
                };
                if (objectResult.Value is not null)
                {
                    // 兼容 { message = "..." } 或 { error = "..." }
                    var valType = objectResult.Value.GetType();
                    var msgProp = valType.GetProperty("message") ?? valType.GetProperty("Message")
                                ?? valType.GetProperty("error")   ?? valType.GetProperty("Error");
                    if (msgProp?.GetValue(objectResult.Value) is string s && !string.IsNullOrWhiteSpace(s))
                    {
                        errMsg = s;
                    }
                }

                context.Result = new ObjectResult(new
                {
                    code = statusCode,
                    data = (object?)null,
                    message = errMsg
                })
                {
                    StatusCode = StatusCodes.Status200OK // 业务错误用 200 + 业务 code 表达，避免浏览器拦截
                };
                await next();
                return;
            }

            // 成功路径（200/201/204）包成 { code: 0, data, message: "ok" }
            var data = objectResult.Value;
            context.Result = new ObjectResult(new
            {
                code = 0,
                data,
                message = "ok"
            })
            {
                StatusCode = statusCode
            };
        }

        await next();
    }
}

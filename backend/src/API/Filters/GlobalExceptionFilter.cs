using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

/// <summary>
/// 全局异常过滤器：把任何未捕获异常包装为
/// { code: 500, data: null, message: "..." }，
/// 同时控制台打印堆栈，方便排错。
/// </summary>
public class GlobalExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "[未捕获异常] {Path}", context.HttpContext.Request.Path);

        var message = context.Exception switch
        {
            KeyNotFoundException knf => knf.Message,
            ArgumentException ae     => ae.Message,
            _                         => $"服务器内部错误: {context.Exception.Message}"
        };

        context.Result = new ObjectResult(new
        {
            code = context.Exception is KeyNotFoundException ? 404 : 500,
            data = (object?)null,
            message
        })
        {
            StatusCode = StatusCodes.Status200OK // 用 200 + 业务 code 表达错误，避免前端被浏览器拦截
        };

        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }
}

using MaaDownloadServer.Services.IServices;

namespace MaaDownloadServer.Middleware;

public class UpdateCheckMiddleware
{
    private readonly ILogger<UpdateCheckMiddleware> _logger;
    private readonly RequestDelegate _next;

    public UpdateCheckMiddleware(RequestDelegate next, ILogger<UpdateCheckMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IResourceManagerService resourceManagerService)
    {
        if (resourceManagerService.IsReplacingLocalFiles() is false)
        {
            await _next(context);
        }

        _logger.LogDebug("服务器更新中，已拒绝一个请求");
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
    }
}

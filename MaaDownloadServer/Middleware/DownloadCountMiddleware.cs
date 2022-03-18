using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Middleware;

public class DownloadCountMiddleware
{

    private readonly ILogger<DownloadCountMiddleware> _logger;
    private readonly RequestDelegate _next;

    public DownloadCountMiddleware(RequestDelegate next, ILogger<DownloadCountMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, MaaDownloadServerDbContext dbContext)
    {
        await _next(context);

        var requestPath = context.Request.Path;
        if (requestPath.StartsWithSegments(new PathString("/files")) is false)
        {
            return;
        }

        if (context.Response.StatusCode != StatusCodes.Status200OK)
        {
            return;
        }

        _logger.LogDebug("中间件监测到文件下载请求，路径为 {P}", requestPath);

        var fileName = requestPath.Value?.Replace("/files/", "");

        if (fileName is null)
        {
            _logger.LogWarning("下载计数中间件找不到文件或 Path 解析失败，当前 Path：{P}", requestPath);
            return;
        }

        var fileId = fileName.Split(".")[0];

        var res = await dbContext.PublicContents
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(fileId));

        if (res is null)
        {
            _logger.LogWarning("下载计数中间件找不到文件或 Path 解析失败，当前 Path：{P}", requestPath);
            return;
        }

        var tag = res.Tag.ParseFromTagString();

        var existed = await dbContext.DownloadCounts
            .FirstOrDefaultAsync(x =>
                x.ComponentName == tag.Component &&
                x.FromVersion == tag.Version.ToString() &&
                (tag.Type == PublicContentTagType.FullPackage || x.ToVersion == tag.Target.ToString()));

        if (existed is not null)
        {
            existed.Count++;
            dbContext.Update(existed);
            await dbContext.SaveChangesAsync();
            return;
        }

        var item = new DownloadCount
        {
            Id = Guid.NewGuid(),
            ComponentName = tag.Component,
            FromVersion = tag.Version.ToString(),
            ToVersion = tag.Type is PublicContentTagType.FullPackage ? "" : tag.Target.ToString(),
            Count = 1
        };

        await dbContext.DownloadCounts.AddAsync(item);
        await dbContext.SaveChangesAsync();
    }
}

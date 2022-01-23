using Microsoft.EntityFrameworkCore;
using Quartz;

namespace MaaDownloadServer.Jobs;

public class PublicContentCheckJob : IJob
{
    private readonly ILogger<PublicContentCheckJob> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly MaaDownloadServerDbContext _dbContext;

    public PublicContentCheckJob(
        ILogger<PublicContentCheckJob> logger,
        IConfigurationService configurationService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _configurationService = configurationService;
        _dbContext = dbContext;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("开始执行 Public Content 检查任务");
        var now = DateTime.Now;

        var outdatedPublicContents = await _dbContext.PublicContents
            .Where(x => x.Duration < now)
            .ToListAsync();

        _logger.LogInformation("找到 {ODCount} 个过期的 Public Content", outdatedPublicContents.Count);

        var pendingRemove = new List<PublicContent>();
        foreach (var pc in outdatedPublicContents)
        {
            try
            {
                var path = Path.Combine(_configurationService.GetPublicDirectory(), $"{pc.Id}.zip");
                if (File.Exists(path))
                {
                    File.Delete(path);
                    pendingRemove.Add(pc);
                    _logger.LogDebug("删除过期的 Public Content {Id}", pc.Id);
                    continue;
                }
                _logger.LogWarning("删除过期文件 ID 为 {ID}，但是文件 {Path} 不存在", pc.Id, path);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "删除 ID 为 {Id} 的 Public Content 失败", pc.Id);
            }
        }

        _dbContext.PublicContents.RemoveRange(pendingRemove);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("成功删除 {RealDeleted}/{AllDeleted} 个过期的 Public Content",
            pendingRemove.Count, outdatedPublicContents.Count);
        if (pendingRemove.Count != outdatedPublicContents.Count)
        {
            _logger.LogWarning("未能删除所有过期的 Public Content，删除失败 {Failed} 个",
                outdatedPublicContents.Count - pendingRemove.Count);
        }
    }
}

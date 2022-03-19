using Microsoft.EntityFrameworkCore;
using Semver;

namespace MaaDownloadServer.Services.Controller;

public class VersionService : IVersionService
{
    private readonly MaaDownloadServerDbContext _dbContext;

    public VersionService(
        MaaDownloadServerDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    /// <summary>
    /// 获取对应平台和架构的某个版本
    /// </summary>
    /// <param name="componentName">组件名</param>
    /// <param name="platform">平台</param>
    /// <param name="architecture">架构</param>
    /// <param name="version">版本</param>
    /// <returns></returns>
    public async Task<Package> GetVersion(string componentName, Platform platform, Architecture architecture, SemVersion version)
    {
        var package = await _dbContext.Packages
            .Include(x => x.Resources)
            .Where(x => x.Component == componentName)
            .Where(x => x.Platform == platform && x.Architecture == architecture && x.Version == version.ToString())
            .FirstOrDefaultAsync();
        return package;
    }
}

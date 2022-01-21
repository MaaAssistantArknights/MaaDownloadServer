using Microsoft.EntityFrameworkCore;
using Semver;

namespace MaaDownloadServer.Services;

public class VersionService : IVersionService
{
    private readonly ICacheService _cacheService;
    private readonly MaaDownloadServerDbContext _dbContext;
    private readonly ILogger<VersionService> _logger;

    public VersionService(ICacheService cacheService,
        MaaDownloadServerDbContext dbContext,
        ILogger<VersionService> logger)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// 检查版本是否存在
    /// </summary>
    /// <param name="version">版本号</param>
    /// <returns></returns>
    public async Task<bool> IsVersionExist(SemVersion version)
    {
        var exist = await _dbContext.Packages
            .FirstOrDefaultAsync(x => x.Version == version.ToString());
        return exist is not null;
    }

    /// <summary>
    /// 获取对应平台和架构的最新版本
    /// </summary>
    /// <param name="platform">平台</param>
    /// <param name="architecture">架构</param>
    /// <returns></returns>
    public async Task<(string, DateTime)> GetLatestVersion(Platform platform, Architecture architecture)
    {
        var cacheKey = _cacheService.GetLatestVersionKey(platform, architecture);
        if (_cacheService.Contains(cacheKey))
        {
            var (cachedVersionString, cachedVersionPublishTime) = _cacheService.Get<(string, DateTime)>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return cachedVersionString == "NotExist"
                ? (null, cachedVersionPublishTime)
                : (cachedVersionString, cachedVersionPublishTime);
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var package = await _dbContext.Packages
            .Where(x => x.Platform == platform && x.Architecture == architecture)
            .OrderByDescending(x => SemVersion.Parse(x.Version, false))
            .FirstOrDefaultAsync();
        if (package is null)
        {
            _cacheService.Add(cacheKey, ("NotExist", DateTime.Now));
            return (null, DateTime.Now);
        }
        _cacheService.Add(cacheKey, (package.Version, package.PublishTime));
        return (package.Version, package.PublishTime);
    }

    /// <summary>
    /// 获取对应平台和架构的某个版本
    /// </summary>
    /// <param name="platform">平台</param>
    /// <param name="architecture">架构</param>
    /// <param name="version">版本</param>
    /// <returns></returns>
    public async Task<Package> GetVersion(Platform platform, Architecture architecture, SemVersion version)
    {
        var cacheKey = _cacheService.GetVersionCacheKey(platform, architecture, version.ToString());
        if (_cacheService.Contains(cacheKey))
        {
            var cachedPackage = _cacheService.Get<Package>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return cachedPackage;
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var package = await _dbContext.Packages
            .Include(x => x.Resources)
            .Where(x => x.Platform == platform && x.Architecture == architecture && x.Version == version.ToString())
            .FirstOrDefaultAsync();
        if (package is null)
        {
            _cacheService.Add(cacheKey, ("NotExist", DateTime.Now));
            return null;
        }
        _cacheService.Add(cacheKey, package);
        return package;
    }

    /// <summary>
    /// 获取所有支持的平台
    /// </summary>
    /// <returns></returns>
    public async Task<List<Platform>> GetSupportedPlatforms()
    {
        var cacheKey = _cacheService.GetAllSupportedPlatformsKey();
        if (_cacheService.Contains(cacheKey))
        {
            var cachedSupportedPlatforms = _cacheService.Get<List<Platform>>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return cachedSupportedPlatforms;
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var supportedPlatforms = await _dbContext.Packages
            .Select(x => x.Platform)
            .Distinct()
            .ToListAsync();
        _cacheService.Add(cacheKey, supportedPlatforms);
        return supportedPlatforms;
    }

    /// <summary>
    /// 获取对应平台支持的所有架构
    /// </summary>
    /// <param name="platform">平台</param>
    /// <returns></returns>
    public async Task<List<Architecture>> GetSupportedArchitectures(Platform platform)
    {
        var cacheKey = _cacheService.GetPlatformSupportedArchitecturesKey(platform);
        if (_cacheService.Contains(cacheKey))
        {
            var cachedSupportedArchitectures = _cacheService.Get<List<Architecture>>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return cachedSupportedArchitectures;
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var supportedArchitectures = await _dbContext.Packages
            .Where(x => x.Platform == platform)
            .Select(x => x.Architecture)
            .Distinct()
            .ToListAsync();
        _cacheService.Add(cacheKey, supportedArchitectures, "supported-architectures");
        return supportedArchitectures;
    }

    /// <summary>
    /// 获取对应平台和架构的所有版本
    /// </summary>
    /// <param name="platform">平台</param>
    /// <param name="architecture">架构</param>
    /// <param name="page">页码</param>
    /// <returns></returns>
    public async Task<Dictionary<string, DateTime>> GetVersions(Platform platform, Architecture architecture, int page)
    {
        var cacheKey = _cacheService.GetVersionsCacheKey(platform, architecture, page);
        if (_cacheService.Contains(cacheKey))
        {
            var cachedVersions = _cacheService.Get<Dictionary<string, DateTime>>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return cachedVersions;
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var versions = await _dbContext.Packages
            .Where(x => x.Platform == platform && x.Architecture == architecture)
            .ToListAsync();
        versions = versions
            .OrderByDescending(x => SemVersion.Parse(x.Version))
            .Skip(10 * (page - 1))
            .Take(10)
            .ToList();
        var result = versions.ToDictionary(x => x.Version, x => x.PublishTime);
        _cacheService.Add(cacheKey, result, $"{platform}-{architecture}-versions");
        return result;
    }
}

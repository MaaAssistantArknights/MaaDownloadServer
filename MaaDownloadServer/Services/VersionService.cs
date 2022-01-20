using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Semver;

namespace MaaDownloadServer.Services;

public class VersionService : IVersionService
{
    private readonly IMemoryCache _cache;
    private readonly MaaDownloadServerDbContext _dbContext;
    private readonly ILogger<VersionService> _logger;

    public VersionService(IMemoryCache cache, MaaDownloadServerDbContext dbContext, ILogger<VersionService> logger)
    {
        _cache = cache;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// 获取对应平台和架构的最新版本
    /// </summary>
    /// <param name="platform">平台</param>
    /// <param name="architecture">架构</param>
    /// <returns></returns>
    public async Task<(string, DateTime)> GetLatestVersion(Platform platform, Architecture architecture)
    {
        var cacheKey = CacheKeyUtil.GetLatestVersionKey(platform, architecture);
        if (_cache.TryGetValue(cacheKey, out _))
        {
            var (cachedVersionString, cachedVersionPublishTime) = _cache.Get<(string, DateTime)>(cacheKey);
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
            _cache.Set(cacheKey, ("NotExist", DateTime.Now));
            return (null, DateTime.Now);
        }
        _cache.Set(cacheKey, (package.Version, package.PublishTime));
        return (package.Version, package.PublishTime);
    }

    /// <summary>
    /// 获取对应平台和架构的某个版本
    /// </summary>
    /// <param name="platform">平台</param>
    /// <param name="architecture">架构</param>
    /// <param name="version">版本</param>
    /// <returns></returns>
    public async Task<(string, DateTime)> GetVersion(Platform platform, Architecture architecture, SemVersion version)
    {
        var cacheKey = CacheKeyUtil.GetVersionCacheKey(platform, architecture, version.ToString());
        if (_cache.TryGetValue(cacheKey, out _))
        {
            var (cachedVersionString, cachedVersionPublishTime) = _cache.Get<(string, DateTime)>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return cachedVersionString == "NotExist"
                ? (null, cachedVersionPublishTime)
                : (cachedVersionString, cachedVersionPublishTime);
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var package = await _dbContext.Packages
            .Where(x => x.Platform == platform && x.Architecture == architecture && x.Version == version.ToString())
            .FirstOrDefaultAsync();
        if (package is null)
        {
            _cache.Set(cacheKey, ("NotExist", DateTime.Now));
            return (null, DateTime.Now);
        }
        _cache.Set(cacheKey, (package.Version, package.PublishTime));
        return (package.Version, package.PublishTime);
    }

    /// <summary>
    /// 获取所有支持的平台
    /// </summary>
    /// <returns></returns>
    public async Task<List<Platform>> GetSupportedPlatforms()
    {
        var cacheKey = CacheKeyUtil.GetAllSupportedPlatformsKey();
        if (_cache.TryGetValue(cacheKey, out _))
        {
            var cachedSupportedPlatforms = _cache.Get<List<Platform>>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return cachedSupportedPlatforms;
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var supportedPlatforms = await _dbContext.Packages
            .Select(x => x.Platform)
            .Distinct()
            .ToListAsync();
        _cache.Set(cacheKey, supportedPlatforms);
        return supportedPlatforms;
    }

    /// <summary>
    /// 获取对应平台支持的所有架构
    /// </summary>
    /// <param name="platform">平台</param>
    /// <returns></returns>
    public async Task<List<Architecture>> GetSupportedArchitectures(Platform platform)
    {
        var cacheKey = CacheKeyUtil.GetPlatformSupportedArchitecturesKey(platform);
        if (_cache.TryGetValue(cacheKey, out _))
        {
            var cachedSupportedArchitectures = _cache.Get<List<Architecture>>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return cachedSupportedArchitectures;
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var supportedArchitectures = await _dbContext.Packages
            .Where(x => x.Platform == platform)
            .Select(x => x.Architecture)
            .Distinct()
            .ToListAsync();
        _cache.Set(cacheKey, supportedArchitectures);
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
        var cacheKey = CacheKeyUtil.GetVersionsCacheKey(platform, architecture, page);
        if (_cache.TryGetValue(cacheKey, out _))
        {
            var cachedVersions = _cache.Get<Dictionary<string, DateTime>>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return cachedVersions;
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var versions = await _dbContext.Packages
            .Where(x => x.Platform == platform && x.Architecture == architecture)
            .OrderByDescending(x => SemVersion.Parse(x.Version, false))
            .Skip(page * 10)
            .Take(10)
            .ToListAsync();
        var result = versions.ToDictionary(x => x.Version, x => x.PublishTime);
        _cache.Set(cacheKey, result);
        return result;
    }
}

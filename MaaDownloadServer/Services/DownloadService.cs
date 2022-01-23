using Microsoft.EntityFrameworkCore;
using Semver;

namespace MaaDownloadServer.Services;

public class DownloadService : IDownloadService
{
    private readonly ILogger<DownloadService> _logger;
    private readonly ICacheService _cacheService;
    private readonly IFileSystemService _fileSystemService;
    private readonly MaaDownloadServerDbContext _dbContext;

    public DownloadService(
        ILogger<DownloadService> logger,
        ICacheService cacheService,
        IFileSystemService fileSystemService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _cacheService = cacheService;
        _fileSystemService = fileSystemService;
        _dbContext = dbContext;
    }

    public async Task<PublicContent> GetFullPackage(Platform platform, Architecture architecture, SemVersion version)
    {
        var cacheKey = _cacheService.GetDownloadFullPackageKey(platform, architecture, version.ToString());
        PublicContent pc;
        if (_cacheService.Contains(cacheKey))
        {
            pc = _cacheService.Get<PublicContent>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
        }
        else
        {
            _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
            var tag = new PublicContentTag(PublicContentTagType.FullPackage, platform, architecture, version).ParseToTagString();
            pc = await _dbContext.PublicContents.FirstOrDefaultAsync(x => x.Tag == tag);
            if (pc is not null)
            {
                _cacheService.Add(cacheKey, pc, PublicContentTagType.FullPackage);
            }
        }
        return pc;
    }

    public async Task<PublicContent> GetUpdatePackage(Platform platform, Architecture architecture, SemVersion from, SemVersion to)
    {
        var cacheKey = _cacheService.GetDownloadUpdatePackageKey(platform, architecture, from.ToString(), to.ToString());
        PublicContent pc;
        if (_cacheService.Contains(cacheKey))
        {
            pc = _cacheService.Get<PublicContent>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
        }
        else
        {
            _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
            var tag = new PublicContentTag(PublicContentTagType.UpdatePackage, platform, architecture, from, to).ParseToTagString();
            pc = await _dbContext.PublicContents.FirstOrDefaultAsync(x => x.Tag == tag);
            if (pc is null)
            {
                var fromPackage = await _dbContext.Packages
                    .FirstOrDefaultAsync(x => x.Platform == platform && x.Architecture == architecture && x.Version == from);
                var toPackage = await _dbContext.Packages
                    .FirstOrDefaultAsync(x => x.Platform == platform && x.Architecture == architecture && x.Version == to);
                if (fromPackage is null || toPackage is null)
                {
                    return null;
                }
                var diff = _fileSystemService.GetUpdateDiff(fromPackage, toPackage);
                var pcs = await _fileSystemService.AddUpdatePackages(new List<UpdateDiff> { diff });
                pc = pcs.First();
            }
            _cacheService.Add(cacheKey, pc, PublicContentTagType.UpdatePackage);
        }
        return pc;
    }
}

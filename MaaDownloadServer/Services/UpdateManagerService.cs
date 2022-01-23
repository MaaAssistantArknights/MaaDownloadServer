using System.Net;
using Microsoft.EntityFrameworkCore;
using Semver;

namespace MaaDownloadServer.Services;

public class UpdateManagerService : IUpdateManagerService
{
    private readonly ILogger<UpdateManagerService> _logger;
    private readonly IFileSystemService _fileSystemService;
    private readonly IConfiguration _configuration;
    private readonly IConfigurationService _configurationService;
    private readonly ICacheService _cacheService;
    private readonly MaaDownloadServerDbContext _dbContext;

    private Guid _jobId;
    private DirectoryInfo _downloadDirectory;
    private DirectoryInfo _tempDirectory;

    public UpdateManagerService(
        ILogger<UpdateManagerService> logger,
        IFileSystemService fileSystemService,
        IConfiguration configuration,
        IConfigurationService configurationService,
        ICacheService cacheService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _fileSystemService = fileSystemService;
        _configuration = configuration;
        _configurationService = configurationService;
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<bool> Update(List<DownloadContentInfo> downloadContentInfos, Guid jobId, SemVersion version, DateTime publishTime, string updateLog)
    {
        _jobId = jobId;
        _logger.LogInformation("新的更新服务器启动，开始更新，JobId：{jobId}", _jobId);

        _downloadDirectory = new DirectoryInfo(Path.Combine(_configurationService.GetDownloadDirectory(), jobId.ToString()));
        _tempDirectory = new DirectoryInfo(Path.Combine(_configurationService.GetTempDirectory(), jobId.ToString()));
        _downloadDirectory.Create();
        _tempDirectory.Create();

        try
        {
            // Step 1 - 下载到 Downloads/{jobId} (DownloadService)
            _logger.LogInformation("Step 1 - 开始下载，JobId：{jobId}", _jobId);
            Download(downloadContentInfos);

            // Step 2 - 逐个解压，解压到 Temps/{jobId}/{dZipId} (FileSystemService)
            _logger.LogInformation("Step 2 - 开始解压，JobId：{jobId}", _jobId);
            foreach (var info in downloadContentInfos)
            {
                _logger.LogTrace("开始解压，JobId：{jobId}，dZipId：{dZipId}", _jobId, info.Id);
                _fileSystemService.UnZipDownloadFile(jobId, info.Id);
            }

            // Step 3 - 完整包加入 Public (FileSystemService)
            _logger.LogInformation("Step 3 - 完整包加入 Public，JobId：{jobId}", _jobId);
            await _fileSystemService.AddFullPackage(_jobId, version,
                downloadContentInfos.ToDictionary(
                    x => (x.Platform, x.Architecture),
                    y => y.Id));

            // Step 4 - 遍历所有解压出来的文件夹，建立 版本-资源（Hash + 相对路径）的索引 （2个） (UpdateManagerService)
            _logger.LogInformation("Step 4 - 开始建立 版本-ResInfo 索引，JobId：{jobId}", _jobId);
            var versionToResourceInfo = new Dictionary<(Guid, Platform, Architecture), List<ResourceInfo>>();
            foreach (var (id, _, p, a) in downloadContentInfos)
            {
                _logger.LogDebug("建立 版本-ResInfo 索引，JobId：{jobId}，dZipId：{dZipId}", _jobId, id);
                var fis =
                    Directory.GetFiles(Path.Combine(_tempDirectory.FullName, id.ToString()),
                            "*", SearchOption.AllDirectories)
                        .Select(x => new FileInfo(x))
                        // .DS_Store / desktop.ini / thumbs.db 属于系统文件，排除三者，防止因失误打包进来
                        .Where(x => x.Name.ToLower() is not ".ds_store" or "desktop.ini" or "thumbs.db")
                        // __MACOSX 属于 MacOs 使用的 APFS 文件系统使用的文件夹，排除此项，防止因失误打包进来
                        .Where(x => x.FullName.Contains("__MACOSX") is false);
                var resInfos = (from fi in fis
                                let relativePath = fi.FullName
                                    .Replace(fi.Name, string.Empty)
                                    .Replace($"/{id}", string.Empty)
                                    .Replace(_tempDirectory.FullName, string.Empty)
                                let hash = HashUtil.ComputeFileMd5Hash(fi.FullName)
                                select new ResourceInfo(fi.FullName, relativePath, hash)).ToList();
                versionToResourceInfo.Add((id, p, a), resInfos);
                _logger.LogDebug("已完成 版本-ResInfo 索引，JobId：{jobId}，dZipId：{dZipId}", _jobId, id);
            }
            var allResourceInfos = versionToResourceInfo.SelectMany(x => x.Value).ToList();

            // Step 5 - 比对所有文件的 Hash 和相对路径，去掉重复 (UpdateManagerService)
            _logger.LogInformation("Step 5 - 开始比对所有文件的 Hash 和相对路径，JobId：{jobId}", _jobId);
            var newResourceInfosUnChecked = allResourceInfos
                .DistinctBy(x => (x.Hash, x.RelativePath))
                .ToList();

            // Step 6 - 把剩下的文件的 Hash 和相对路径和数据库比对，去掉重复 (UpdateManagerService)
            _logger.LogInformation("Step 7 - 开始与数据库比对文件的 Hash 和相对路径，JobId：{jobId}", _jobId);
            var newResourceInfos = new List<ResourceInfo>();
            foreach (var info in newResourceInfosUnChecked)
            {
                var exist = await _dbContext.Resources
                    .FirstOrDefaultAsync(x => x.Hash == info.Hash && x.Path == info.RelativePath);
                if (exist is null)
                {
                    newResourceInfos.Add(info);
                }
            }

            // Step 7 - 移动所有的新增资源到 Resources，添加进数据库 (FileSystemService)
            _logger.LogInformation("Step 7 - 开始移动所有的新增资源到 Resources，添加进数据库，JobId：{jobId}", _jobId);
            await _fileSystemService.AddNewResources(newResourceInfos);

            // Step 8 - 创建 Package 实体并存入数据库 (UpdateManagerService)
            _logger.LogInformation("Step 8 - 开始创建 Package 实体并存入数据库，JobId：{jobId}", _jobId);
            var newPackageList = new List<Package>();
            foreach (var ((id, p, a), resInfos) in versionToResourceInfo)
            {
                var package = new Package(id, version.ToString(), p, a, publishTime, updateLog);
                var resources = _dbContext.Resources
                    .AsEnumerable()
                    .Where(x => resInfos.Exists(y => x.Hash == y.Hash && x.Path == y.RelativePath))
                    .ToList();
                package.Resources = resources;
                newPackageList.Add(package);
            }
            await _dbContext.Packages.AddRangeAsync(newPackageList);
            await _dbContext.SaveChangesAsync();

            // Step 9 - 检索数据库获取前三个版本的资源数据  (UpdateManagerService)
            _logger.LogInformation("Step 9 - 开始检索数据库获取前三个版本的资源数据，JobId：{jobId}", _jobId);
            var recentVersionPackages = new List<Package>();
            foreach (var newPackage in newPackageList)
            {
                var recentPackages = await _dbContext.Packages
                    .AsNoTracking()
                    .Include(x => x.Resources)
                    .Where(x => x.Platform == newPackage.Platform && x.Architecture == newPackage.Architecture)
                    .Where(x => x.PublishTime < publishTime)
                    .OrderByDescending(x => x.PublishTime)
                    .Take(3)
                    .ToListAsync();
                recentVersionPackages.AddRange(recentPackages);
            }

            // Step 10 - 遍历 版本-资源（Hash + 相对路径）的索引，逐个和前三个版本的每一个做比对，得到增量更新列表 (UpdateManagerService)
            _logger.LogInformation("Step 10 - 开始和前三个版本做对比建立 Diff，JobId：{jobId}", _jobId);
            var updateDiffs = GetUpdateDiffs(newPackageList, recentVersionPackages);

            // Step 11 - 通过增量更新文件列表打包文件（复制到 Temps/{zipId} 打包到 Temps/{zipId}.zip）并写入数据库 (FileSystemService)
            _logger.LogInformation("Step 11 - 开始通过增量更新文件列表打包文件，JobId：{jobId}", _jobId);
            var _ = await _fileSystemService.AddUpdatePackages(updateDiffs);

            // Step 12 - 更新 版本-平台-架构 信息删缓存 (UpdateManagerService)
            _logger.LogInformation("Step 12 - 删缓存");
            var supportedPlatforms = _cacheService.Get<List<Platform>>(_cacheService.GetAllSupportedPlatformsKey());
            if (supportedPlatforms is not null && newPackageList.Exists(x => supportedPlatforms.Contains(x.Platform)) is false)
            {
                _cacheService.Remove(_cacheService.GetAllSupportedPlatformsKey());
            }
            _cacheService.RemoveAll("supported-architectures");
            foreach (var package in newPackageList)
            {
                _cacheService.RemoveAll($"{package.Platform}-{package.Architecture}-versions");
                _cacheService.Remove(_cacheService.GetLatestVersionKey(package.Platform, package.Architecture));
            }

            // Step 13 - 完成
            _logger.LogInformation("Step 13 - 获取新 Release 完成，清理临时文件夹，JobId：{jobId}", _jobId);
            CleanUp();
        }
        catch (Exception e)
        {
            CleanUp();
            _logger.LogError(e, "更新失败，JobId：{jobId}", _jobId);
            return false;
        }

        return true;
    }

    private void CleanUp()
    {
        if (_downloadDirectory.Exists)
        {
            _downloadDirectory.Delete(true);
        }

        if (_tempDirectory.Exists)
        {
            _tempDirectory.Delete(true);
        }
    }

    private void Download(IEnumerable<DownloadContentInfo> downloadContentInfos)
    {
        var proxy = _configuration["MaaServer:GithubQuery:Proxy"];
        var httpClient = proxy is null or ""
            ? new HttpClient()
            : new HttpClient(new HttpClientHandler { Proxy = new WebProxy(proxy) });

        downloadContentInfos.AsParallel().ForAll(info =>
        {
            var filePath = Path.Combine(_downloadDirectory.FullName, $"{info.Id}.zip");
            var responseResult = httpClient.GetAsync(info.DownloadUrl);
            using var memStream = responseResult.Result.Content.ReadAsStreamAsync().Result;
            using var fileStream = File.Create(filePath);
            memStream.CopyTo(fileStream);
        });

        // if (downloadTaskPool.All(x => x.IsCompletedSuccessfully) is false)
        // {
        //     var failedUrls = downloadTaskPool
        //         .Where(x => x.IsCompletedSuccessfully is false)
        //         .Select(x => x.Id)
        //         .Select(x => taskIdToFileIdIndex[x])
        //         .Select(x => downloadContentInfos.First(y => y.Id == x))
        //         .Select(x => x.DownloadUrl)
        //         .Aggregate((x, y) => $"{x}; {y}");
        //     throw new Exception($"下载失败：{failedUrls}");
        // }
    }

    private List<UpdateDiff> GetUpdateDiffs(IEnumerable<Package> thisVersionPackages, IReadOnlyCollection<Package> recentVersionPackages)
    {
        var updateDiffs = new List<UpdateDiff>();
        foreach (var thisVersionPackage in thisVersionPackages)
        {
            foreach (var recentVersionPackage in recentVersionPackages)
            {
                if (thisVersionPackage.Architecture != recentVersionPackage.Architecture ||
                    thisVersionPackage.Platform != recentVersionPackage.Platform)
                {
                    continue;
                }

                _logger.LogDebug("开始计算 Diff {P}-{A} {V1} -> {V2} ，JobId：{jobId}",
                    thisVersionPackage.Platform, thisVersionPackage.Architecture,
                    recentVersionPackage.Version, thisVersionPackage.Version, _jobId);
                // 新资源包括了 旧版本和新版本中同路径、同文件名但是 Hash 不同的文件；旧版本不存在但是新版本存在的文件
                // 路径、文件名、Hash 三者任意一个不同，这 ID 不同，因此匹配 ID 即可
                var diff = _fileSystemService.GetUpdateDiff(recentVersionPackage, thisVersionPackage);
                updateDiffs.Add(diff);
                _logger.LogInformation("从 {P}-{A} {V1} -> {V2} 的 Diff 计算完成，新增 {New}，移除 {Remove}，JobId：{jobId}",
                    thisVersionPackage.Platform, thisVersionPackage.Architecture,
                    recentVersionPackage.Version, thisVersionPackage.Version,
                    diff.NewResources.Count, diff.UnNeededResources.Count, _jobId);
            }
        }
        return updateDiffs;
    }
}

using System.Net;
using Microsoft.EntityFrameworkCore;
using Semver;

#pragma warning disable CS0618

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
            _logger.LogDebug("Step 1 - 开始下载，JobId：{jobId}", _jobId);
            await Download(downloadContentInfos);

            // Step 2 - 逐个解压，解压到 Temps/{jobId}/{dZipId} (FileSystemService)
            _logger.LogDebug("Step 2 - 开始解压，JobId：{jobId}", _jobId);
            foreach (var info in downloadContentInfos)
            {
                _logger.LogTrace("开始解压，JobId：{jobId}，dZipId：{dZipId}", _jobId, info.Id);
                _fileSystemService.UnZipDownloadFile(jobId, info.Id);
            }

            // Step 3 - 完整包加入 Public (FileSystemService)
            _logger.LogDebug("Step 3 - 完整包加入 Public，JobId：{jobId}", _jobId);
            await _fileSystemService.AddFullPackage(_jobId, version,
                downloadContentInfos.ToDictionary(
                    x => (x.Platform, x.Architecture),
                    y => y.Id));

            // Step 4 - 遍历所有解压出来的文件夹，建立 版本-资源（Hash + 相对路径）的索引 （2个） (UpdateManagerService)
            _logger.LogDebug("Step 4 - 开始建立 版本-ResInfo 索引，JobId：{jobId}", _jobId);
            var versionToResourceInfo = new Dictionary<(Guid, Platform, Architecture), List<ResourceInfo>>();
            foreach (var (id, _, p, a) in downloadContentInfos)
            {
                _logger.LogDebug("建立 版本-ResInfo 索引，JobId：{jobId}，dZipId：{dZipId}", _jobId, id);
                var fis =
                    Directory.GetFiles(Path.Combine(_downloadDirectory.FullName, id.ToString()),
                            "*", SearchOption.AllDirectories)
                        .Select(x => new FileInfo(x))
                        // .DS_Store / desktop.ini / thumbs.db 属于系统文件，排除三者，防止因失误打包进来
                        .Where(x => x.Name.ToLower() is not ".ds_store" or "desktop.ini" or "thumbs.db")
                        // __MACOSX 属于 MacOs 使用的 APFS 文件系统使用的文件夹，排除此项，防止因失误打包进来
                        .Where(x => x.FullName.Contains("__MACOSX"));
                var resInfos = (from fi in fis
                        let relativePath = fi.FullName
                            .Replace(fi.Name, string.Empty)
                            .Replace($"/{id.ToString()}", string.Empty)
                            .Replace(_downloadDirectory.FullName, string.Empty)
                        let hash = HashUtil.ComputeFileMd5Hash(fi.FullName)
                        select new ResourceInfo(fi.FullName, relativePath, hash)).ToList();
                versionToResourceInfo.Add((id, p, a), resInfos);
                _logger.LogTrace("已完成 版本-ResInfo 索引，JobId：{jobId}，dZipId：{dZipId}", _jobId, id);
            }
            var allResourceInfos = versionToResourceInfo.SelectMany(x => x.Value).ToList();

            // Step 5 - 比对所有文件的 Hash 和相对路径，去掉重复 (UpdateManagerService)
            _logger.LogDebug("Step 5 - 开始比对所有文件的 Hash 和相对路径，JobId：{jobId}", _jobId);
            var newResourceInfos = allResourceInfos
                .DistinctBy(x => (x.Hash, x.RelativePath))
                .ToList();

            // Step 6 - 把剩下的文件的 Hash 和相对路径和数据库比对，去掉重复 (UpdateManagerService)
            _logger.LogDebug("Step 7 - 开始与数据库比对文件的 Hash 和相对路径，JobId：{jobId}", _jobId);
            newResourceInfos = newResourceInfos
                .Where(x =>
                    _dbContext.Resources
                        .AsNoTracking()
                        .FirstOrDefault(y => y.Path == x.RelativePath && x.Hash == y.Hash) is null)
                .ToList();

            // Step 7 - 移动所有的新增资源到 Resources，添加进数据库 (FileSystemService)
            _logger.LogDebug("Step 7 - 开始移动所有的新增资源到 Resources，添加进数据库，JobId：{jobId}", _jobId);
            await _fileSystemService.AddNewResources(newResourceInfos);

            // Step 8 - 创建 Package 实体并存入数据库 (UpdateManagerService)
            _logger.LogDebug("Step 8 - 开始创建 Package 实体并存入数据库，JobId：{jobId}", _jobId);
            var newPackageList = new List<Package>();
            foreach (var ((id, p, a), resInfos) in versionToResourceInfo)
            {
                var package = new Package(id, version.ToString(), p, a, publishTime, updateLog);
                var resources = _dbContext.Resources
                    .Where(x => resInfos.Exists(y => x.Path == y.RelativePath && x.Hash == y.Hash))
                    .ToList();
                package.Resources = resources;
                newPackageList.Add(package);
            }
            await _dbContext.Packages.AddRangeAsync(newPackageList);
            await _dbContext.SaveChangesAsync();

            // Step 9 - 删除 Downloads/{jobId} 文件夹下的内容 (FileSystemService)
            _logger.LogDebug("Step 9 - 开始删除 Downloads/{jobId} 文件夹下的内容，JobId：{jobId}", _jobId, _jobId);
            _fileSystemService.CleanDownloadDirectory(_jobId);

            // Step 10 - 检索数据库获取前三个版本的资源数据  (UpdateManagerService)
            _logger.LogDebug("Step 10 - 开始检索数据库获取前三个版本的资源数据，JobId：{jobId}", _jobId);
            var recentVersionPackages = await _dbContext.Packages
                .AsNoTracking()
                .Include(x => x.Resources)
                .Where(x => x.PublishTime < publishTime)
                .OrderByDescending(x => x.PublishTime)
                .Take(3)
                .ToListAsync();

            // Step 11 - 遍历 版本-资源（Hash + 相对路径）的索引，逐个和前三个版本的每一个做比对，得到增量更新列表 (UpdateManagerService)
            _logger.LogDebug("Step 11 - 开始和前三个版本做对比建立 Diff，JobId：{jobId}", _jobId);
            var updateDiffs = GetUpdateDiffs(newPackageList, recentVersionPackages);

            // Step 12 - 通过增量更新文件列表打包文件（复制到 Temps/{zipId} 打包到 Temps/{zipId}.zip）并写入数据库 (FileSystemService)
            _logger.LogDebug("Step 12 - 开始通过增量更新文件列表打包文件，JobId：{jobId}", _jobId);
            var _ = await _fileSystemService.AddUpdatePackages(updateDiffs);

            // Step 13 - 更新 版本-平台-架构 信息删缓存 (UpdateManagerService)
            _logger.LogDebug("Step 13 - 删缓存");
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
        _downloadDirectory.Delete();
        _tempDirectory.Delete();
    }

    private async Task Download(IReadOnlyCollection<DownloadContentInfo> downloadContentInfos)
    {
        var proxy = _configuration["MaaServer:GithubQuery:Proxy"];
        var webClient = new WebClient();
        if (proxy is not null or "")
        {
            webClient.Proxy = new WebProxy(proxy);
        }

        var taskIdToFileIdIndex = new Dictionary<int, Guid>();

        var downloadTaskPool = new List<Task>();
        foreach (var downloadContentInfo in downloadContentInfos)
        {
            var filePath = Path.Combine(_downloadDirectory.FullName, $"{downloadContentInfo.Id}.zip");
            var t = webClient.DownloadFileTaskAsync(downloadContentInfo.DownloadUrl, filePath);
            taskIdToFileIdIndex.Add(t.Id, downloadContentInfo.Id);
            downloadTaskPool.Add(t);
        }

        await Task.WhenAll(downloadTaskPool);
        if (downloadTaskPool.All(x => x.IsCompletedSuccessfully) is false)
        {
            var failedUrls = downloadTaskPool
                .Where(x => x.IsCompletedSuccessfully is false)
                .Select(x => x.Id)
                .Select(x => taskIdToFileIdIndex[x])
                .Select(x => downloadContentInfos.First(y => y.Id == x))
                .Select(x => x.DownloadUrl)
                .Aggregate((x, y) => $"{x}; {y}");
            throw new Exception($"下载失败：{failedUrls}");
        }
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
                var newRes = thisVersionPackage.Resources
                    .Where(x => recentVersionPackage.Resources.Exists(y => y.Id != x.Id))
                    .ToList();
                // 不需要的指 旧版本存在，但是新版本中，同路径、同文件名的文件不存在的资源
                var unNeededRes = recentVersionPackage.Resources
                    .Where(x => thisVersionPackage.Resources
                        .Exists(y => (y.Path == x.Path && y.FileName == x.FileName) is false))
                    .ToList();
                var diff = new UpdateDiff(recentVersionPackage.Version, thisVersionPackage.Version,
                    thisVersionPackage.Platform, thisVersionPackage.Architecture,
                    newRes, unNeededRes);
                updateDiffs.Add(diff);
                _logger.LogInformation("从 {P}-{A} {V1} -> {V2} 的 Diff 计算完成，新增 {New}，移除 {Remove}，JobId：{jobId}",
                    thisVersionPackage.Platform, thisVersionPackage.Architecture,
                    recentVersionPackage.Version, thisVersionPackage.Version,
                    newRes.Count, unNeededRes.Count, _jobId);
            }
        }
        return updateDiffs;
    }
}

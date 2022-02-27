using System.IO.Compression;
using System.Net;
using System.Text.Json;
using MaaDownloadServer.External;
using MaaDownloadServer.Model.External;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Semver;

namespace MaaDownloadServer.Jobs;

public class PackageUpdateJob : IJob
{
    private readonly ILogger<PackageUpdateJob> _logger;
    private readonly ILogger<Python> _pyLogger;
    private readonly IFileSystemService _fileSystemService;
    private readonly IConfiguration _configuration;
    private readonly IConfigurationService _configurationService;
    private readonly ICacheService _cacheService;
    private readonly MaaDownloadServerDbContext _dbContext;

    private DirectoryInfo _downloadDirectory;
    private DirectoryInfo _tempDirectory;

    public PackageUpdateJob(
        ILogger<PackageUpdateJob> logger,
        ILogger<Python> pyLogger,
        IFileSystemService fileSystemService,
        IConfiguration configuration,
        IConfigurationService configurationService,
        ICacheService cacheService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _pyLogger = pyLogger;
        _fileSystemService = fileSystemService;
        _configuration = configuration;
        _configurationService = configurationService;
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        context.MergedJobDataMap.TryGetValue("configuration", out var componentConfigurationObject);

        if (componentConfigurationObject is not ComponentConfiguration componentConfiguration)
        {
            _logger.LogCritical("Component configuration 为 Null，无法执行任务");
            return;
        }

        var componentConfigurationString = JsonSerializer.Serialize(componentConfiguration);

        var jobId = Guid.NewGuid();

        _logger.LogInformation("开始组件包 {name} 更新检查任务，操作ID：{id}", componentConfiguration.Name, jobId);

        try
        {
            #region STEP 0: 准备

            _logger.LogInformation("[{id}] STEP 0: 准备", jobId);

            var pyExecutable = Path.Combine(
                _configuration["MaaServer:DataDirectories:RootPath"],
                _configuration["MaaServer:DataDirectories:SubDirectories:VirtualEnvironments"],
                componentConfiguration.Name,
                "bin",
                "python");

            _downloadDirectory =
                new DirectoryInfo(Path.Combine(_configurationService.GetDownloadDirectory(), jobId.ToString()));
            _tempDirectory =
                new DirectoryInfo(Path.Combine(_configurationService.GetTempDirectory(), jobId.ToString()));
            _downloadDirectory.Create();
            _tempDirectory.Create();

            #endregion

            #region STEP 1: 请求 Metadata API

            _logger.LogInformation("[{id}] STEP 1: 请求 Metadata API", jobId);
            var apis = componentConfiguration.MetadataUrl;
            foreach (var (k, vo) in componentConfiguration.UrlPlaceholder)
            {
                if (vo is not JsonElement element)
                {
                    _logger.LogCritical("[{id}] 在 Metadata API 请求中，参数 {key} 的值无法转换为 JsonElement，无法执行任务", jobId, k);
                    return;
                }

                if (element.ValueKind is not JsonValueKind.String)
                {
                    _logger.LogCritical("[{id}] 在 Metadata API 请求中，参数 {key} 的值不是 String，无法执行任务", jobId, k);
                    return;
                }

                var v = element.GetString();

                apis = apis.Select(x => x.Replace("{" + k + "}", v)).ToList();
            }

            var proxyUrl = "";
            if (componentConfiguration.UseProxy)
            {
                var proxy = _configuration["MaaServer:Network:Proxy"];
                if (proxy is not null or "")
                {
                    proxyUrl = proxy;
                }
            }

            using var client = proxyUrl is ""
                ? new HttpClient()
                : new HttpClient(new HttpClientHandler() { Proxy = new WebProxy(proxyUrl), UseProxy = true });
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36 Edg/97.0.1072.62");

            var packageInfos = new List<string>();
            foreach (var api in apis)
            {
                var retryCount = 3;
                while (retryCount != 0)
                {
                    var response = await client.GetAsync(api);
                    if (response.IsSuccessStatusCode is false)
                    {
                        retryCount--;
                        continue;
                    }

                    var payload = await response.Content.ReadAsStringAsync();
                    packageInfos.Add(payload);
                    break;
                }
            }

            #endregion

            #region STEP 2: 运行Python脚本获取下载信息列表

            _logger.LogInformation("[{id}] STEP 2: 获取下载元数据", jobId);
            var getMetadataScript = Path.Combine(
                _configuration["MaaServer:DataDirectories:RootPath"],
                _configuration["MaaServer:DataDirectories:SubDirectories:Scripts"],
                componentConfiguration.Name,
                componentConfiguration.Scripts.GetDownloadInfo);
            var getMetadataScriptArgs = new List<string> { componentConfigurationString };
            getMetadataScriptArgs.AddRange(packageInfos);
            var getMetadataScriptResponse = Python.Run(_pyLogger, pyExecutable, getMetadataScript, getMetadataScriptArgs);
            if (getMetadataScriptResponse is null)
            {
                throw new Exception("获取下载元数据失败");
            }

            var allDownloadContentInfos = JsonSerializer.Deserialize<List<DownloadContentInfo>>(getMetadataScriptResponse);
            if (allDownloadContentInfos is null)
            {
                _logger.LogError("[{id}] 解析下载元数据失败，原始字符串：{str}", jobId, getMetadataScriptResponse);
                throw new Exception("解析下载元数据失败");
            }

            #endregion

            #region STEP 3: 检查版本号，检查数据库

            _logger.LogInformation("[{id}] STEP 3: 检查版本号，检查数据库", jobId);
            var downloadContentInfos = new List<DownloadContentInfo>();
            foreach (var downloadContentInfo in allDownloadContentInfos)
            {
                var semVerParsed = SemVersion.TryParse(downloadContentInfo.Version, out var semVer);
                if (semVerParsed is false)
                {
                    _logger.LogError("[{id}] 无法解析版本号 {version}", jobId, downloadContentInfo.Version);
                    throw new Exception("无法解析版本号");
                }

                var versionExistInDatabase = await _dbContext.Packages.AsNoTracking().FirstOrDefaultAsync(x =>
                    x.Component == componentConfiguration.Name &&
                    x.Architecture == downloadContentInfo.Architecture &&
                    x.Platform == downloadContentInfo.Platform &&
                    x.Version == semVer.ToString());
                if (versionExistInDatabase is null)
                {
                    downloadContentInfos.Add(downloadContentInfo);
                }
            }

            if (downloadContentInfos.Count == 0)
            {
                _logger.LogInformation("[{id}] 组件 {cName} 无版本变更, 退出更新任务", jobId, componentConfiguration.Name);
                return;
            }

            #endregion

            #region STEP 4, 5: 下载文件 & 校验文件，校验失败则返回重试，最多尝试下载3次

            _logger.LogInformation("[{id}] STEP 4, 5: 下载文件 & 校验文件", jobId);
            var pendingDownloadContents = downloadContentInfos.ToList();

            var downloadRetryTimes = 3;
            while (downloadRetryTimes > 0)
            {
                downloadRetryTimes--;
                Download(pendingDownloadContents);

                var downloadedContents = Directory.GetFiles(_downloadDirectory.FullName);
                foreach (var downloadedContent in downloadedContents)
                {
                    var idStr = downloadedContent.Split('/').Last()[..36];
                    var id = Guid.Parse(idStr);
                    var info = downloadContentInfos.First(x => x.Id == id);
                    if (info.ChecksumType is not ChecksumType.None)
                    {
                        var hash = HashUtil.ComputeFileHash(info.ChecksumType, downloadedContent).ToLower();
                        if (hash != info.Checksum.ToLower())
                        {
                            _logger.LogError("[{id}] 文件 {file} 校验失败，原始文件哈希值：{hash}，校验值：{checksum}", jobId,
                                downloadedContent, hash, info.Checksum);
                            File.Delete(downloadedContent);
                        }
                    }

                    pendingDownloadContents.RemoveAll(x => x.Id == id);
                }

                if (pendingDownloadContents.Count == 0)
                {
                    break;
                }
            }

            if (pendingDownloadContents.Count != 0)
            {
                _logger.LogError("[{id}] 下载失败，未下载的文件：{files}", jobId, string.Join(", ", pendingDownloadContents.Select(x => $"[{x.Id}]{x.DownloadUrl}")));
                throw new Exception("下载失败");
            }

            #endregion

            #region STEP 6: 执行AfterDownloadProcess

            _logger.LogInformation("[{id}] STEP 6: 执行AfterDownloadProcess", jobId);
            switch (componentConfiguration.AfterDownloadProcess.Operation)
            {
                case AfterDownloadProcessOperation.Unzip:
                    _logger.LogDebug("[{id}] 执行AfterDownloadProcess：解压", jobId);
                    foreach (var downloadContentInfo in downloadContentInfos)
                    {
                        var filePath = Path.Combine(_downloadDirectory.FullName, $"{downloadContentInfo.Id}.zip");
                        var target = Path.Combine(_tempDirectory.FullName, downloadContentInfo.Id.ToString());
                        ZipFile.ExtractToDirectory(filePath, target);
                    }
                    break;
                case AfterDownloadProcessOperation.None:
                    _logger.LogDebug("[{id}] 执行AfterDownloadProcess：无操作", jobId);
                    break;
                case AfterDownloadProcessOperation.Custom:
                    _logger.LogDebug("[{id}] 执行AfterDownloadProcess：自定义", jobId);
                    var afterDownloadProcessArgs = JsonSerializer.Serialize(componentConfiguration.AfterDownloadProcess.Args);
                    var afterDownloadProcessScript = Path.Combine(
                        _configuration["MaaServer:DataDirectories:RootPath"],
                        _configuration["MaaServer:DataDirectories:SubDirectories:Scripts"],
                        componentConfiguration.Name,
                        componentConfiguration.Scripts.AfterDownloadProcess);
                    foreach (var dci in downloadContentInfos)
                    {
                        var target = Path.Combine(_tempDirectory.FullName, dci.Id.ToString());
                        var source = Path.Combine(_downloadDirectory.FullName, dci.Id.ToString() + "." + dci.FileExtension);
                        Directory.CreateDirectory(target);
                        Python.Run(_pyLogger, pyExecutable, afterDownloadProcessScript,
                            new[] { afterDownloadProcessArgs, source, target });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(componentConfiguration), "未知的 AfterDownloadProcessOperation");
            }

            #endregion

            #region STEP 7: 将完整包加入PublicContent

            _logger.LogInformation("[{id}] STEP 7: 将完整包加入PublicContent", jobId);
            foreach (var downloadContentInfo in downloadContentInfos)
            {
                await _fileSystemService.AddFullPackage(jobId, componentConfiguration.Name, downloadContentInfo);
            }

            #endregion

            #region STEP 8: 执行BeforeAddProcess

            _logger.LogInformation("[{id}] STEP 8: 执行BeforeAddProcess", jobId);
            switch (componentConfiguration.BeforeAddProcess.Operation)
            {
                case BeforeAddProcessOperation.Zip:
                    _logger.LogDebug("[{id}] 执行BeforeAddProcess：压缩", jobId);
                    var zipArgs = componentConfiguration.BeforeAddProcess.Args;
                    var pendingZippedArgs = zipArgs.GetProperty("zip").EnumerateArray().ToList();
                    var pendingZippedList = new Dictionary<string, (List<string>, List<string>)>();
                    foreach (var pendingZippedArg in pendingZippedArgs)
                    {
                        var name = pendingZippedArg.GetProperty("name").GetString();
                        var files = pendingZippedArg.GetProperty("files").EnumerateArray()
                            .Select(x => x.GetString()).ToList();
                        var folders = pendingZippedArg.GetProperty("folders").EnumerateArray()
                            .Select(x => x.GetString()).ToList();
                        if (name is null)
                        {
                            throw new JsonException("Zip file name is null");
                        }

                        if (files.Contains(null))
                        {
                            throw new JsonException("File list contains null");
                        }

                        if (folders.Contains(null))
                        {
                            throw new JsonException("Folder list contains null");
                        }
                        pendingZippedList.Add(name, (files, folders));
                    }
                    foreach (var downloadContentPath in
                             downloadContentInfos.Select(downloadContentInfo => Path.Combine(_tempDirectory.FullName, downloadContentInfo.Id.ToString())))
                    {
                        foreach (var (name, (files, folders)) in pendingZippedList)
                        {
                            var f = files.Select(x => Path.Combine(downloadContentPath, x));
                            var d = folders.Select(x => Path.Combine(downloadContentPath, x));
                            var zipFilePath = Path.Combine(downloadContentPath, name + ".zip");
                            _fileSystemService.CreateZipFile(f, d, zipFilePath, CompressionLevel.SmallestSize, true);
                        }
                    }
                    break;
                case BeforeAddProcessOperation.None:
                    _logger.LogDebug("[{id}] 执行BeforeAddProcess：不执行", jobId);
                    break;
                case BeforeAddProcessOperation.Custom:
                    _logger.LogDebug("[{id}] 执行BeforeAddProcess：自定义", jobId);
                    var beforeAddProcessArgs = JsonSerializer.Serialize(componentConfiguration.BeforeAddProcess.Args);
                    var beforeAddProcessScript = Path.Combine(
                        _configuration["MaaServer:DataDirectories:RootPath"],
                        _configuration["MaaServer:DataDirectories:SubDirectories:Scripts"],
                        componentConfiguration.Name,
                        componentConfiguration.Scripts.BeforeAddProcess);
                    foreach (var downloadContentPath in
                             downloadContentInfos.Select(downloadContentInfo => Path.Combine(_tempDirectory.FullName, downloadContentInfo.Id.ToString())))
                    {
                        Python.Run(_pyLogger, pyExecutable, beforeAddProcessScript,
                            new[] { downloadContentPath, beforeAddProcessArgs });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(componentConfiguration), "未知的 BeforeAddProcessOperation");
            }

            #endregion

            #region STEP 9: 遍历资源，计算MD5 Hash建立ResourceInfo，可能需要运行Python脚本计算相对路径，第一次去除重复

            _logger.LogInformation("[{id}] STEP 9: 遍历资源，计算MD5 Hash建立ResourceInfo", jobId);
            var packageToResources = new Dictionary<Guid, List<ResourceInfo>>();
            foreach (var downloadContentInfo in downloadContentInfos)
            {
                packageToResources.Add(downloadContentInfo.Id, new List<ResourceInfo>());

                var downloadContentDirectory =
                    new DirectoryInfo(Path.Combine(_tempDirectory.FullName, downloadContentInfo.Id.ToString()));
                var files = downloadContentDirectory.GetFiles("*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var fileRelativePath = file.FullName
                        .Replace(downloadContentDirectory.FullName, "")
                        .Replace(file.Name, "");
                    var fileHash = HashUtil.ComputeFileMd5Hash(file.FullName);
                    if (componentConfiguration.Scripts.RelativePathCalculation is not "" or null)
                    {
                        _logger.LogInformation("[{id}] 运行 Python 脚本进行相对路径计算", jobId);
                        var relativePathCalculationScript = Path.Combine(
                            _configuration["MaaServer:DataDirectories:RootPath"],
                            _configuration["MaaServer:DataDirectories:SubDirectories:Scripts"],
                            componentConfiguration.Name,
                            componentConfiguration.Scripts.RelativePathCalculation);
                        fileRelativePath = Python.Run(_pyLogger, pyExecutable, relativePathCalculationScript,
                            new[] { file.FullName, downloadContentDirectory.FullName });
                    }
                    packageToResources[downloadContentInfo.Id].Add(new ResourceInfo(file.FullName, fileRelativePath, fileHash));
                }
            }

            var resourceInfos = packageToResources.Values
                .SelectMany(x => x)
                .ToList()
                .DistinctBy(x => (x.Hash, x.RelativePath))
                .ToList();


            #endregion

            #region STEP 10: Resource与数据库比对，去除重复

            _logger.LogInformation("[{id}] STEP 10: Resource与数据库比对，去除重复", jobId);
            var resourceInfosNoDuplicate = new List<ResourceInfo>();
            foreach (var resourceInfo in resourceInfos)
            {
                var exist = await _dbContext.Resources.AsNoTracking().AnyAsync(x =>
                    x.Hash == resourceInfo.Hash && x.Path == resourceInfo.RelativePath);
                if (exist is false)
                {
                    resourceInfosNoDuplicate.Add(resourceInfo);
                }
            }

            #endregion

            #region STEP 11: 添加Resource进入数据库

            _logger.LogInformation("[{id}] STEP 11: 添加Resource进入数据库", jobId);
            await _fileSystemService.AddNewResources(resourceInfosNoDuplicate);

            #endregion

            #region STEP 12: 根据RelativePath，FileName和Hash从数据库检索包文件并建立包对象添加进入数据库

            _logger.LogInformation("[{id}] STEP 12: 根据RelativePath，FileName和Hash从数据库检索包文件并建立包对象添加进入数据库", jobId);
            var packages = new List<Package>();

            foreach (var (id, ris) in packageToResources)
            {
                var info = downloadContentInfos.First(x => x.Id == id);
                var package = new Package(id, componentConfiguration.Name, info.Version, info.Platform,
                    info.Architecture, info.UpdateTime, info.UpdateLog)
                {
                    Resources = new List<Resource>()
                };

                foreach (var (path, relativePath, hash) in ris)
                {
                    var r = await _dbContext.Resources
                        .FirstOrDefaultAsync(x => x.Hash == hash && x.Path == relativePath);
                    if (r is null)
                    {
                        _logger.LogError("[{id}] 找不到资源文件：{fn}({hash}) 位于 {rPath}",
                            jobId, new FileInfo(path).Name, hash, relativePath);
                        throw new Exception("找不到资源文件");
                    }
                    package.Resources.Add(r);
                }

                packages.Add(package);
            }

            await _dbContext.Packages.AddRangeAsync(packages);
            await _dbContext.SaveChangesAsync();

            #endregion

            #region STEP 13: 获取近期3个版本的包对象，比对Resource并打包更新包

            _logger.LogInformation("[{id}] STEP 13: 获取近期3个版本的包对象，比对Resource并打包更新包", jobId);
            var recentVersionPackages = new List<Package>();
            foreach (var package in packages)
            {
                var recentPackages = (await _dbContext.Packages
                        .AsNoTracking()
                        .Include(x => x.Resources)
                        .Where(x => x.Component == package.Component)
                        .Where(x => x.Platform == package.Platform && x.Architecture == package.Architecture)
                        .ToListAsync())
                    .Where(x => SemVersion.Parse(x.Version) < SemVersion.Parse(package.Version))
                    .OrderByDescending(x => SemVersion.Parse(x.Version))
                    .Take(3)
                    .ToList();
                recentVersionPackages.AddRange(recentPackages);
            }

            var diffs = GetUpdateDiffs(packages, recentVersionPackages, jobId.ToString());

            await _fileSystemService.AddUpdatePackages(diffs);

            #endregion

            #region STEP 14: 删缓存

            _logger.LogInformation("[{id}] STEP 14: 删缓存", jobId);

            // 删除 所有组件 缓存
            _cacheService.Remove(_cacheService.GetAllComponentsKey());

            // 删除 组件名Group 缓存
            _cacheService.RemoveAll(componentConfiguration.Name);

            #endregion

            #region STEP 15: 完成

            _logger.LogInformation("[{id}] STEP 15: 完成", jobId);
            CleanUp();

            #endregion
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "组件包 {name} 更新检查任务执行失败", componentConfiguration.Name);
            CleanUp();
        }
    }

    private void Download(IEnumerable<DownloadContentInfo> downloadContentInfos)
    {
        var proxy = _configuration["MaaServer:Network:Proxy"];
        var httpClient = proxy is null or ""
            ? new HttpClient()
            : new HttpClient(new HttpClientHandler { Proxy = new WebProxy(proxy) });

        downloadContentInfos.AsParallel().ForAll(info =>
        {
            var filePath = Path.Combine(_downloadDirectory.FullName, $"{info.Id}.{info.FileExtension}");
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

    private List<UpdateDiff> GetUpdateDiffs(IEnumerable<Package> thisVersionPackages, IReadOnlyCollection<Package> recentVersionPackages, string jobId)
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

                _logger.LogDebug("开始计算 Diff {C}-{P}-{A} {V1} -> {V2} ，JobId：{jobId}",
                    thisVersionPackage.Component, thisVersionPackage.Platform, thisVersionPackage.Architecture,
                    recentVersionPackage.Version, thisVersionPackage.Version, jobId);
                // 新资源包括了 旧版本和新版本中同路径、同文件名但是 Hash 不同的文件；旧版本不存在但是新版本存在的文件
                // 路径、文件名、Hash 三者任意一个不同，这 ID 不同，因此匹配 ID 即可
                var diff = _fileSystemService.GetUpdateDiff(recentVersionPackage, thisVersionPackage);
                updateDiffs.Add(diff);
                _logger.LogInformation("从 {C}-{P}-{A} {V1} -> {V2} 的 Diff 计算完成，新增 {New}，移除 {Remove}，JobId：{jobId}",
                    thisVersionPackage.Component, thisVersionPackage.Platform, thisVersionPackage.Architecture,
                    recentVersionPackage.Version, thisVersionPackage.Version,
                    diff.NewResources.Count, diff.UnNeededResources.Count, jobId);
            }
        }

        return updateDiffs;
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
}

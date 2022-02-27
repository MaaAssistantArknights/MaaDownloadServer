using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using Semver;

namespace MaaDownloadServer.Services.Base;

public class FileSystemService : IFileSystemService
{
    private readonly MaaDownloadServerDbContext _dbContext;
    private readonly ILogger<FileSystemService> _logger;
    private readonly IConfigurationService _configurationService;

    public FileSystemService(
        MaaDownloadServerDbContext dbContext,
        ILogger<FileSystemService> logger,
        IConfigurationService configurationService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _configurationService = configurationService;
    }

    /// <inheritdoc />
    public string CreateZipFile(string sourceFolder, string targetName, CompressionLevel level = CompressionLevel.NoCompression, bool deleteSource = false)
    {
        if (targetName is null)
        {
            throw new ArgumentNullException(nameof(targetName));
        }

        if (Directory.Exists(sourceFolder) is false)
        {
            throw new DirectoryNotFoundException($"文件夹 {sourceFolder} 不存在");
        }

        if (File.Exists(targetName))
        {
            throw new FileNotFoundException($"文件 {targetName} 已存在");
        }

        ZipFile.CreateFromDirectory(sourceFolder, targetName, level, false);
        if (deleteSource)
        {
            Directory.Delete(sourceFolder, true);
        }

        return targetName;
    }

    /// <inheritdoc />
    public string CreateZipFile(IEnumerable<string> sourceFiles, IEnumerable<string> sourceDirectories,
        string targetName, CompressionLevel level = CompressionLevel.NoCompression, bool deleteSource = false)
    {
        var randomId = Guid.NewGuid().ToString();
        var tempFolder = Path.Combine(_configurationService.GetTempDirectory(), randomId);
        Directory.CreateDirectory(tempFolder);
        var fileEnumerable = sourceFiles as string[] ?? sourceFiles.ToArray();
        var directoryEnumerable = sourceDirectories as string[] ?? sourceDirectories.ToArray();
        foreach (var sourceFile in fileEnumerable)
        {
            var fi = new FileInfo(sourceFile);
            if (fi.Exists)
            {
                fi.CopyTo(Path.Combine(tempFolder, fi.Name));
                continue;
            }
            Directory.Delete(tempFolder, true);
            throw new FileNotFoundException($"文件 {sourceFile} 不存在");
        }

        foreach (var sourceDirectory in directoryEnumerable)
        {
            var di = new DirectoryInfo(sourceDirectory);
            if (di.Exists)
            {
                di.CopyTo(Path.Combine(tempFolder, di.Name));
                continue;
            }
            Directory.Delete(tempFolder, true);
            throw new DirectoryNotFoundException($"文件夹 {sourceDirectory} 不存在");
        }

        var result = CreateZipFile(tempFolder, targetName, level, deleteSource);

        if (deleteSource is false)
        {
            return result;
        }

        foreach (var sourceFile in fileEnumerable)
        {
            if (File.Exists(sourceFile))
            {
                File.Delete(sourceFile);
            }
        }

        foreach (var sourceDirectory in directoryEnumerable)
        {
            if (Directory.Exists(sourceDirectory))
            {
                Directory.Delete(sourceDirectory, true);
            }
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<PublicContent> AddFullPackage(Guid jobId, string componentName, DownloadContentInfo downloadContentInfo)
    {
        var path = Path.Combine(
            _configurationService.GetDownloadDirectory(),
            jobId.ToString(),
            $"{downloadContentInfo.Id}.{downloadContentInfo.FileExtension}");
        if (File.Exists(path) is false)
        {
            _logger.LogError("正在准备复制完整包至 Public 但是文件 {Path} 不存在", path);
            return null;
        }

        var hash = HashUtil.ComputeFileMd5Hash(path);
        var publicContentTag = new PublicContentTag(PublicContentTagType.FullPackage, downloadContentInfo.Platform,
            downloadContentInfo.Architecture, componentName, SemVersion.Parse(downloadContentInfo.Version));

        var pc = new PublicContent(
            downloadContentInfo.Id,
            downloadContentInfo.FileExtension,
            publicContentTag.ParseToTagString(),
            DateTime.Now,
            hash,
            DateTime.Now.AddDays(_configurationService.GetPublicContentAutoBundledDuration()));
        var targetPath = Path.Combine(
            _configurationService.GetPublicDirectory(),
            $"{downloadContentInfo.Id}.{downloadContentInfo.FileExtension}");
        File.Copy(path, targetPath);
        await _dbContext.PublicContents.AddAsync(pc);
        await _dbContext.SaveChangesAsync();
        return pc;
    }

    /// <inheritdoc />
    public async Task AddNewResources(List<ResourceInfo> res)
    {
        var resources = new List<Resource>();
        foreach (var (path, relativePath, hash) in res)
        {
            var id = Guid.NewGuid();
            var name = Path.GetFileName(path);
            _logger.LogDebug("添加新的资源文件 [{id}] {Path} ({Hash})", id, name, hash);
            resources.Add(new Resource(id, name, relativePath, hash));
            Debug.Assert(path != null, "r.Path != null");
            File.Move(path, Path.Combine(
                _configurationService.GetResourcesDirectory(), hash));
        }
        await _dbContext.Resources.AddRangeAsync(resources);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public void CleanDownloadDirectory(Guid jobId)
    {
        var di = new DirectoryInfo(Path.Combine(_configurationService.GetDownloadDirectory(), jobId.ToString()));
        if (di.Exists is false)
        {
            return;
        }

        _logger.LogInformation("正在清理下载目录 Job = {jobId}", jobId);
        di.Delete(true);
    }

    /// <inheritdoc />
    public UpdateDiff GetUpdateDiff(Package fromPackage, Package toPackage)
    {
        // 在新版本中选择 路径/文件名/Hash 三者存在不同的文件，为新增文件
        // 可能是二进制文件更新导致 Hash 不同，可能是文件移动导致路径不同，可能是文件重命名导致文件名不同
        var newRes = (from r in toPackage.Resources
                      let isOld = fromPackage.Resources.Exists(x => x.Hash == r.Hash && x.FileName == r.FileName && x.Path == r.Path)
                      where isOld is false
                      select r).ToList();
        // 不需要的指 旧版本存在，但是新版本中，同路径、同文件名、同 Hash 的文件不存在的资源
        var unNeededRes = (from r in fromPackage.Resources
                           let isOld = toPackage.Resources.Exists(x => x.Hash == r.Hash && x.FileName == r.FileName && x.Path == r.Path)
                           where isOld is false
                           select r).ToList();
        var diff = new UpdateDiff(fromPackage.Version, toPackage.Version,
            toPackage.Platform, toPackage.Architecture,
            newRes, unNeededRes);
        return diff;
    }

    /// <inheritdoc />
    public async Task<List<PublicContent>> AddUpdatePackages(List<UpdateDiff> diffs)
    {
        var pcs = new List<PublicContent>();
        foreach (var diff in diffs)
        {
            var id = Guid.NewGuid();
            _logger.LogInformation("打包更新包 {StartVer} -> {TargetVer}，ID = {Id}",
                diff.StartVersion, diff.TargetVersion, id);
            var tempFolder = new DirectoryInfo(Path.Combine(_configurationService.GetTempDirectory(), id.ToString()));
            tempFolder.Create();
            var pcTag = new PublicContentTag(PublicContentTagType.UpdatePackage, diff.Platform, diff.Architecture,
                diff.StartVersion, diff.TargetVersion).ParseToTagString();
            foreach (var newResource in diff.NewResources)
            {
                File.Copy(Path.Combine(_configurationService.GetResourcesDirectory(), newResource.Hash),
                    Path.Combine(tempFolder.FullName, newResource.FileName));
                _logger.LogDebug("打包更新包 {Id}，复制新资源 {ResName} ({Hash})",
                    id, newResource.FileName, newResource.Hash);
            }
            var updatePackageLog = JsonSerializer.Serialize(diff);
            await File.WriteAllTextAsync(Path.Combine(tempFolder.FullName, "update_log.json"), updatePackageLog);
            var zipFile = Path.Combine(_configurationService.GetTempDirectory(), $"{id}.zip");
            ZipFile.CreateFromDirectory(tempFolder.FullName, zipFile);
            var hash = HashUtil.ComputeFileMd5Hash(zipFile);
            pcs.Add(new PublicContent(id, "zip", pcTag, DateTime.Now, hash, DateTime.Now.AddDays(_configurationService.GetPublicContentDefaultDuration())));
            File.Move(zipFile, Path.Combine(_configurationService.GetPublicDirectory(), $"{id}.zip"));
            tempFolder.Delete(true);
            _logger.LogInformation("已打包更新包 {Id}，MD5校验 = {Hash}", id, hash);
        }
        await _dbContext.PublicContents.AddRangeAsync(pcs);
        await _dbContext.SaveChangesAsync();
        return pcs;
    }
}

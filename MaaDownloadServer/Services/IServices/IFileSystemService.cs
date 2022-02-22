using System.IO.Compression;
using Semver;

namespace MaaDownloadServer.Services.IServices;

public interface IFileSystemService
{
    /// <summary>
    /// 创建压缩包
    /// </summary>
    /// <param name="sourceFolder">源文件夹</param>
    /// <param name="targetName">目标文件位置，扩展名必须为 .zip</param>
    /// <param name="level">压缩等级</param>
    /// <param name="deleteSource">是否删除源</param>
    /// <returns>创建的压缩包文件路径</returns>
    string CreateZipFile(string sourceFolder, string targetName, CompressionLevel level = CompressionLevel.NoCompression, bool deleteSource = false);

    /// <summary>
    /// 创建压缩包
    /// </summary>
    /// <param name="sourceFiles">源文件</param>
    /// <param name="sourceDirectories">源文件夹</param>
    /// <param name="targetName">目标文件位置，扩展名必须为 .zip</param>
    /// <param name="level">压缩等级</param>
    /// <param name="deleteSource">是否删除源</param>
    /// <returns></returns>
    string CreateZipFile(IEnumerable<string> sourceFiles, IEnumerable<string> sourceDirectories, string targetName, CompressionLevel level = CompressionLevel.NoCompression, bool deleteSource = false);

    /// <summary>
    /// 添加完整包至 Public
    /// </summary>
    /// <param name="jobId">本次 Job 的 Id，用于寻找完整包位置</param>
    /// <param name="componentName">组件名</param>
    /// <param name="downloadContentInfo">下载元数据</param>
    /// <returns>PublicContent 实体</returns>
    Task<PublicContent> AddFullPackage(Guid jobId, string componentName, DownloadContentInfo downloadContentInfo);

    /// <summary>
    /// 添加新的资源文件
    /// </summary>
    /// <param name="res">资源文件信息表</param>
    /// <returns></returns>
    Task AddNewResources(List<ResourceInfo> res);

    /// <summary>
    /// 清空下载目录
    /// </summary>
    /// <param name="jobId">本次 Job 的 Id</param>
    void CleanDownloadDirectory(Guid jobId);

    /// <summary>
    /// 获取更新 Diff
    /// </summary>
    /// <param name="fromPackage">起始版本</param>
    /// <param name="toPackage">目标版本</param>
    /// <returns></returns>
    UpdateDiff GetUpdateDiff(Package fromPackage, Package toPackage);

    /// <summary>
    /// 创建更新包
    /// </summary>
    /// <param name="diffs">更新 Diff 列表</param>
    /// <returns>PublicContent 实体 List</returns>
    public Task<List<PublicContent>> AddUpdatePackages(List<UpdateDiff> diffs);
}

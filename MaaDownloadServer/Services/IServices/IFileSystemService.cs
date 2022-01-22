using Semver;

namespace MaaDownloadServer.Services.IServices;

public interface IFileSystemService
{
    /// <summary>
    /// 解压下载下来的文件
    /// </summary>
    /// <param name="jobId">本次 Job 的 Id</param>
    /// <param name="fileId">文件 Id</param>
    /// <returns>文件 Id</returns>
    Guid UnZipDownloadFile(Guid jobId, Guid fileId);

    /// <summary>
    /// 添加完整包至 Public
    /// </summary>
    /// <param name="jobId">本次 Job 的 Id，用于寻找完整包位置</param>
    /// <param name="version">版本</param>
    /// <param name="files">平台-架构->文件 Id 表，用于寻找完整包位置，并作为数据库主键</param>
    /// <returns>PublicContent 实体</returns>
    Task<List<PublicContent>> AddFullPackage(Guid jobId, SemVersion version, Dictionary<(Platform, Architecture), Guid> files);

    /// <summary>
    /// 添加新的资源文件
    /// </summary>
    /// <param name="res">资源文件信息表</param>
    /// <returns>新增的资源列表</returns>
    Task<List<Resource>> AddNewResources(List<ResourceInfo> res);

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

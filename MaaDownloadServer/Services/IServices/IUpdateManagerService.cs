using Semver;

namespace MaaDownloadServer.Services.IServices;

public interface IUpdateManagerService
{
    Task<bool> Update(List<DownloadContentInfo> downloadContentInfos, Guid jobId, SemVersion version, DateTime publishTime);
}

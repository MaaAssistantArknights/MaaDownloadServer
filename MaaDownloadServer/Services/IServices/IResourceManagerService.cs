using Semver;

namespace MaaDownloadServer.Services.IServices;

public interface IResourceManagerService
{
    SemVersion GetLocalVersion();
    Task<bool> DownloadUpdates(Dictionary<PlatformArchCombination, (string, DateTime)> downloadUrls, SemVersion version);
}

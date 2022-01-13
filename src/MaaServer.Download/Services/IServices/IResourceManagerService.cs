using Semver;

namespace MaaServer.Download.Services.IServices;

public interface IResourceManagerService
{
    bool IsReplacingLocalFiles();
    SemVersion GetLocalVersion();
    Task Initialize();
    Task<bool> DownloadUpdates(Dictionary<PlatformArchCombination, string> downloadUrls, SemVersion version);
}

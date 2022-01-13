using MaaDownloadServer.Model.General;
using Semver;

namespace MaaDownloadServer.Services.IServices;

public interface IResourceManagerService
{
    bool IsReplacingLocalFiles();
    SemVersion GetLocalVersion();
    Task<bool> DownloadUpdates(Dictionary<PlatformArchCombination, string> downloadUrls, SemVersion version);
}

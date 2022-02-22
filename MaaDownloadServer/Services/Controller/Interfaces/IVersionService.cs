using Semver;

namespace MaaDownloadServer.Services.Controller.Interfaces;

public interface IVersionService
{
    Task<bool> IsVersionExist(string componentName, SemVersion version);
    Task<(string, DateTime)> GetLatestVersion(string componentName, Platform platform, Architecture architecture);
    Task<Package> GetVersion(string componentName, Platform platform, Architecture architecture, SemVersion semVersion);
    Task<List<Platform>> GetSupportedPlatforms(string componentName);
    Task<List<Architecture>> GetSupportedArchitectures(string componentName, Platform platform);
    Task<Dictionary<string, DateTime>> GetVersions(string componentName, Platform platform, Architecture architecture, int page);
}

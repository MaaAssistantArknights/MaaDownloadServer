using Semver;

namespace MaaDownloadServer.Services.Controller.Interfaces;

public interface IVersionService
{
    Task<Package> GetVersion(string componentName, Platform platform, Architecture architecture, SemVersion semVersion);
}

using Semver;

namespace MaaDownloadServer.Services.IServices;

public interface IDownloadService
{
    Task<PublicContent> GetFullPackage(string componentName, Platform platform, Architecture architecture, SemVersion version);
    Task<PublicContent> GetUpdatePackage(string componentName, Platform platform, Architecture architecture, SemVersion from, SemVersion to);
}

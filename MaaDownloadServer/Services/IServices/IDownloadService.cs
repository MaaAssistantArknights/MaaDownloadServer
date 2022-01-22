using Semver;

namespace MaaDownloadServer.Services.IServices;

public interface IDownloadService
{
    Task<PublicContent> GetFullPackage(Platform platform, Architecture architecture, SemVersion version);
    Task<PublicContent> GetUpdatePackage(Platform platform, Architecture architecture, SemVersion from, SemVersion to);
}

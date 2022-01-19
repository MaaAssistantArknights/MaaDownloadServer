using Semver;

namespace MaaDownloadServer.Model.General;

public record DownloadContentInfo(Guid Id, Guid JobId, string DownloadUrl, SemVersion Version, Platform Platform, Architecture Architecture)
{
    public Guid Id { get; set; } = Id;
    public string DownloadUrl { get; set; } = DownloadUrl;
    public SemVersion Version { get; set; } = Version;
    public Platform Platform { get; set; } = Platform;
    public Architecture Architecture { get; set; } = Architecture;
    public Guid JobId { get; set; } = JobId;
}

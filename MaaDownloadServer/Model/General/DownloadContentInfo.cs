using Semver;

namespace MaaDownloadServer.Model.General;

public record DownloadContentInfo(Guid Id, string DownloadUrl, Platform Platform, Architecture Architecture)
{
    public Guid Id { get; set; } = Id;
    public string DownloadUrl { get; set; } = DownloadUrl;
    public Platform Platform { get; set; } = Platform;
    public Architecture Architecture { get; set; } = Architecture;
}

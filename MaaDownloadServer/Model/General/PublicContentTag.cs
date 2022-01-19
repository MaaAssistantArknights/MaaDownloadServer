using Semver;

namespace MaaDownloadServer.Model.General;

public record PublicContentTag(PublicContentTagType Type, Platform Platform, Architecture Architecture, SemVersion Version, SemVersion Target = null)
{
    public PublicContentTagType Type { get; set; } = Type;
    public Platform Platform { get; set; } = Platform;
    public Architecture Architecture { get; set; } = Architecture;
    public SemVersion Version { get; set; } = Version;
    public SemVersion Target { get; set; } = Target;
}

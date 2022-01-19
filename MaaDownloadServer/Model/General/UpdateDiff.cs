using System.Text.Json.Serialization;
using Semver;

namespace MaaDownloadServer.Model.General;

public record UpdateDiff(SemVersion StartVersion, SemVersion TargetVersion, Platform Platform, Architecture Architecture, List<Resource> NewResources, List<Resource> UnNeededResources)
{
    [JsonPropertyName("start_version")]
    public SemVersion StartVersion { get; set; } = StartVersion;
    [JsonPropertyName("target_version")]
    public SemVersion TargetVersion { get; set; } = TargetVersion;
    [JsonPropertyName("platform")]
    public Platform Platform { get; set; } = Platform;
    [JsonPropertyName("architecture")]
    public Architecture Architecture { get; set; } = Architecture;
    [JsonPropertyName("new_resources")]
    public List<Resource> NewResources { get; set; } = NewResources;
    [JsonPropertyName("unneeded_resources")]
    public List<Resource> UnNeededResources { get; set; } = UnNeededResources;
}

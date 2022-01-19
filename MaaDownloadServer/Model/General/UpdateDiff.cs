using System.Text.Json.Serialization;
using Semver;

namespace MaaDownloadServer.Model.General;

public record UpdateDiff(SemVersion StartVersion, SemVersion TargetVersion, Platform Platform, Architecture Architecture, List<Resource> SameResources, List<Resource> NewResources, List<Resource> ReplaceResources, List<Resource> UnNeededResources)
{
    [JsonPropertyName("start_version")]
    public SemVersion StartVersion { get; set; } = StartVersion;
    [JsonPropertyName("target_version")]
    public SemVersion TargetVersion { get; set; } = TargetVersion;
    [JsonPropertyName("platform")]
    public Platform Platform { get; set; } = Platform;
    [JsonPropertyName("architecture")]
    public Architecture Architecture { get; set; } = Architecture;
    [JsonPropertyName("same_resources")]
    public List<Resource> SameResources { get; set; } = SameResources;
    [JsonPropertyName("new_resources")]
    public List<Resource> NewResources { get; set; } = NewResources;
    [JsonPropertyName("replace_resources")]
    public List<Resource> ReplaceResources { get; set; } = ReplaceResources;
    [JsonPropertyName("unneeded_resources")]
    public List<Resource> UnNeededResources { get; set; } = UnNeededResources;
}

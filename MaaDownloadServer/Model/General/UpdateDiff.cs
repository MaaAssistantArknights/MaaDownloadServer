using System.Text.Json.Serialization;
using Semver;

namespace MaaDownloadServer.Model.General;

public record UpdateDiff(string StartVersion, string TargetVersion, Platform Platform, Architecture Architecture, List<Resource> NewResources, List<Resource> UnNeededResources)
{
    [JsonPropertyName("start_version")]
    public string StartVersion { get; set; } = StartVersion;

    [JsonPropertyName("target_version")]
    public string TargetVersion { get; set; } = TargetVersion;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("platform")]
    public Platform Platform { get; set; } = Platform;

    [JsonPropertyName("architecture")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Architecture Architecture { get; set; } = Architecture;

    [JsonPropertyName("new_resources")]
    public List<Resource> NewResources { get; set; } = NewResources;

    [JsonPropertyName("unneeded_resources")]
    public List<Resource> UnNeededResources { get; set; } = UnNeededResources;
}

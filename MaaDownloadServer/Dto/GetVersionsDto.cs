using System.Text.Json.Serialization;
using MaaDownloadServer.Dto.General;

namespace MaaDownloadServer.Dto;

public record GetVersionsDto(string Platform, string Architecture, List<VersionMetadata> Version)
{
    [JsonPropertyName("platform")]
    public string Platform { get; set; } = Platform;

    [JsonPropertyName("arch")]
    public string Architecture { get; set; } = Architecture;

    [JsonPropertyName("versions")]
    public List<VersionMetadata> Version { get; set; } = Version;
}

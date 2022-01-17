using System.Text.Json.Serialization;
using MaaDownloadServer.Dto.General;

namespace MaaDownloadServer.Dto;

public record GetVersionDto(string Platform, string Arch, VersionMetadata VersionMetadata)
{
    [JsonPropertyName("platform")]
    public string Platform { get; set; } = Platform;

    [JsonPropertyName("arch")]
    public string Arch { get; set; } = Arch;

    [JsonPropertyName("version_metadata")]
    public VersionMetadata VersionMetadata { get; set; } = VersionMetadata;
}

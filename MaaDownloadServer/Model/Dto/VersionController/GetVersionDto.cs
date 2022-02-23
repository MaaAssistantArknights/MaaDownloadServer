using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetVersionDto(string Platform, string Arch, VersionDetail VersionDetail)
{
    [JsonPropertyName("platform")]
    public string Platform { get; set; } = Platform;

    [JsonPropertyName("arch")]
    public string Arch { get; set; } = Arch;

    [JsonPropertyName("details")]
    public VersionDetail VersionDetail { get; set; } = VersionDetail;
}

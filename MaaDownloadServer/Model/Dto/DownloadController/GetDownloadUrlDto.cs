using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetDownloadUrlDto(string Platform, string Architecture, string Version, string Url, string Hash)
{
    [JsonPropertyName("platform")]
    public string Platform { get; set; } = Platform;

    [JsonPropertyName("arch")]
    public string Architecture { get; set; } = Architecture;

    [JsonPropertyName("version")]
    public string Version { get; set; } = Version;

    [JsonPropertyName("url")]
    public string Url { get; set; } = Url;

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = Hash;
}

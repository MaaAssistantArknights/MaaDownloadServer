using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetDownloadUrlDto(
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("arch")] string Architecture,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("hash")] string Hash);

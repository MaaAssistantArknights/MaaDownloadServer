using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetVersionDto(
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("arch")] string Arch,
    [property: JsonPropertyName("details")] VersionDetail VersionDetail);

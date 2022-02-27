using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetVersionsDto(
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("arch")] string Architecture,
    [property: JsonPropertyName("versions")] List<VersionMetadata> Version);

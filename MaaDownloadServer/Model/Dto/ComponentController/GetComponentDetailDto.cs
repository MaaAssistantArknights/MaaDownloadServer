using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetComponentDetailDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("versions")] List<ComponentVersions> Versions,
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("limit")] int Limit);

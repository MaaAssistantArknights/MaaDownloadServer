using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record ComponentDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description);

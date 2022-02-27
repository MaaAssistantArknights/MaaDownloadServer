using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record Existence(
    [property: JsonPropertyName("cn")] ExistenceContent Cn,
    [property: JsonPropertyName("jp")] ExistenceContent Jp,
    [property: JsonPropertyName("kr")] ExistenceContent Kr,
    [property: JsonPropertyName("us")] ExistenceContent Us);

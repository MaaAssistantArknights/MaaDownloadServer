using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetItemDto(
    [property: JsonPropertyName("id")] string ItemId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("usage")] string Usage,
    [property: JsonPropertyName("obtain")] string ObtainMethod,
    [property: JsonPropertyName("rarity")] int Rarity,
    [property: JsonPropertyName("image")] string Image,
    [property: JsonPropertyName("category")] List<string> Category);

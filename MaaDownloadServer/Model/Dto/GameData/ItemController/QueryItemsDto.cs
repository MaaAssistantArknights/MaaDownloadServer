using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record QueryItemsDto(
    [property: JsonPropertyName("items")] List<GetItemDto> Items,
    [property: JsonPropertyName("total")] int TotalResults,
    [property: JsonPropertyName("limit")] int Limit,
    [property: JsonPropertyName("page")] int Page);

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record QueryZoneDto(
    [property: JsonPropertyName("zones")] List<GetZoneDto> Zones,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("limit")] int Limit,
    [property: JsonPropertyName("page")] int Page);

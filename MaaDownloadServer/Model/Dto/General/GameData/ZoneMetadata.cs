using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record ZoneMetadata(
    [property: JsonPropertyName("zone_id")] string ZoneId,
    [property: JsonPropertyName("zone_name")] string ZoneName,
    [property: JsonPropertyName("zone_type")] string ZoneType,
    [property: JsonPropertyName("background")] string Background);

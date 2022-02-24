using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetZoneDto(
    [property: JsonPropertyName("zone_metadata")] ZoneMetadata ZoneMetadata,
    [property: JsonPropertyName("existence")] Existence Existence,
    [property: JsonPropertyName("zone_i18n")] ArkI18N ZoneI18N,
    [property: JsonPropertyName("stages")] List<StageMetadata> Stages);

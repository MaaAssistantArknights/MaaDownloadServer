using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetStageDto(
    [property: JsonPropertyName("stage_metadata")] StageMetadata StageMetadata,
    [property: JsonPropertyName("zone_metadata")] ZoneMetadata ZoneMetadata,
    [property: JsonPropertyName("stage_i18n")] ArkI18N StageI18N,
    [property: JsonPropertyName("zone_i18n")] ArkI18N ZoneI18N,
    [property: JsonPropertyName("existence")] Existence Existence);

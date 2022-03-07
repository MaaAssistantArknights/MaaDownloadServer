using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetStageDto(
    [property: JsonPropertyName("stage_metadata")] StageMetadata StageMetadata,
    [property: JsonPropertyName("stage_i18n")] ArkI18N StageI18N,
    [property: JsonPropertyName("existence")] Existence Existence,
    [property: JsonPropertyName("drop_items")] List<StageDropItem> DropItems);

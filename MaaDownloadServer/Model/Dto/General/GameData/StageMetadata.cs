using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record StageMetadata(
    [property: JsonPropertyName("stage_id")] string StageId,
    [property: JsonPropertyName("stage_type")] string StageType,
    [property: JsonPropertyName("stage_code")] string StageCode,
    [property: JsonPropertyName("stage_ap_cost")] int StageApCost,
    [property: JsonPropertyName("min_clear_time")] long MinClearTime);

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.External;

public record ApiPenguinStage
{
    [JsonPropertyName("stageType")] public string StageType { get; set; }
    [JsonPropertyName("stageId")] public string StageId { get; set; }
    [JsonPropertyName("zoneId")] public string ZoneId { get; set; }
    [JsonPropertyName("code")] public string StageCode { get; set; }
    [JsonPropertyName("apCost")] public int StageApCost { get; set; }
    [JsonPropertyName("existence")] public ApiPenguinExistence Existence { get; set; }
    [JsonPropertyName("code_i18n")] public ApiPenguinI18N CodeI18N { get; set; }
    [JsonPropertyName("minClearTime")] public long? MinClearTime { get; set; }
    [JsonPropertyName("dropInfos")] public List<ApiPenguinDropInfos> DropInfos { get; set; }
    [JsonExtensionData] public Dictionary<string, object> ExtensionData { get; set; }
}

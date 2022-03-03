using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.External;

public record ApiPenguinZone
{
    [JsonPropertyName("zoneId")] public string ZoneId { get; set; }
    [JsonPropertyName("type")] public string ZoneType { get; set; }
    [JsonPropertyName("zoneName")] public string ZoneName { get; set; }
    [JsonPropertyName("existence")] public ApiPenguinExistence Existence { get; set; }
    [JsonPropertyName("background")] public string Background { get; set; }
    [JsonPropertyName("stages")] public List<string> Stages { get; set; }
    [JsonPropertyName("zoneName_i18n")] public ApiPenguinI18N ZoneNameI18N { get; set; }
    [JsonExtensionData] public Dictionary<string, object> ExtensionData { get; set; }
}

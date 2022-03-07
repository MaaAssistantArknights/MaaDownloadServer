using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.External;

public record ApiPenguinDropInfos
{
    [JsonPropertyName("itemId")] public string ItemId { get; set; }
    [JsonExtensionData] public Dictionary<string, object> ExtensionData { get; set; }
}

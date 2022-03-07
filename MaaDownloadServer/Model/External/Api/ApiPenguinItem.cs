using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.External;

public record ApiPenguinItem
{
    [JsonPropertyName("itemId")] public string ItemId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("sortId")] public int SortId { get; set; }
    [JsonPropertyName("rarity")] public int Rarity { get; set; }
    [JsonPropertyName("existence")] public ApiPenguinExistence Existence { get; set; }
    [JsonPropertyName("itemType")] public string ItemType { get; set; }
    [JsonPropertyName("name_i18n")] public ApiPenguinI18N NameI18N { get; set; }
    [JsonExtensionData] public Dictionary<string, object> ExtensionData { get; set; }
}

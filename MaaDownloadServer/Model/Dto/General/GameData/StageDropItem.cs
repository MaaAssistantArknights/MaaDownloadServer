using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record StageDropItem(
    [property: JsonPropertyName("item_id")] string ItemId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("sort_id")] int SortId,
    [property: JsonPropertyName("rarity")] int Rarity,
    [property: JsonPropertyName("item_type")] string ItemType,
    [property: JsonPropertyName("existence")] Existence Existence,
    [property: JsonPropertyName("name_i18n")] ArkI18N NameI18N);

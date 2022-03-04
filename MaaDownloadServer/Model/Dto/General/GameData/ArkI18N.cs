using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record ArkI18N(
    [property: JsonPropertyName("zh")] string Chinese,
    [property: JsonPropertyName("ko")] string Korean,
    [property: JsonPropertyName("ja")] string Japanese,
    [property: JsonPropertyName("en")] string English);

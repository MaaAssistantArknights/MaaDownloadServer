using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.External;

public record ApiPenguinI18N
{
    [JsonPropertyName("ko")] public string Korean { get; set; }
    [JsonPropertyName("ja")] public string Japanese { get; set; }
    [JsonPropertyName("en")] public string English { get; set; }
    [JsonPropertyName("zh")] public string Chinese { get; set; }
}

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.External;

public record ApiPenguinExistenceContent
{
    [JsonPropertyName("exist")] public bool Exist { get; set; }
    [JsonPropertyName("openTime")] public long? OpenTime { get; set; } = null;
    [JsonPropertyName("closeTime")] public long? CloseTime { get; set; } = null;
}

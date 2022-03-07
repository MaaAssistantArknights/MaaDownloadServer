using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.External;

public record ApiPenguinExistence
{
    [JsonPropertyName("US")] public ApiPenguinExistenceContent Us { get; set; }
    [JsonPropertyName("JP")] public ApiPenguinExistenceContent Jp { get; set; }
    [JsonPropertyName("CN")] public ApiPenguinExistenceContent Cn { get; set; }
    [JsonPropertyName("KR")] public ApiPenguinExistenceContent Kr { get; set; }
}

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto.General;

public record VersionMetadata(string Version, DateTime PublishTime)
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = Version;

    [JsonPropertyName("publish_time")]
    public DateTime PublishTime { get; set; } = PublishTime;
}

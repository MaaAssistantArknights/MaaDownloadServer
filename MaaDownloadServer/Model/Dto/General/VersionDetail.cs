using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record VersionDetail(string Version, DateTime PublishTime, string UpdateLog, List<ResourceMetadata> Resources)
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = Version;

    [JsonPropertyName("publish_time")]
    public DateTime PublishTime { get; set; } = PublishTime;

    [JsonPropertyName("update_log")]
    public string UpdateLog { get; set; } = UpdateLog;

    [JsonPropertyName("resources")]
    public List<ResourceMetadata> Resources { get; set; } = Resources;
}

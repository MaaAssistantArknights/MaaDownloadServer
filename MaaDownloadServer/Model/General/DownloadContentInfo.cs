using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.General;

public record DownloadContentInfo()
{
    [JsonIgnore]
    public Guid Id { get; init; } = Guid.NewGuid();

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; init; }

    [JsonPropertyName("platform")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Platform Platform { get; init; }

    [JsonPropertyName("arch")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Architecture Architecture { get; init; }

    [JsonPropertyName("file_extension")]
    public string FileExtension { get; set; }

    [JsonPropertyName("checksum")]
    public string Checksum { get; init; }

    [JsonPropertyName("checksum_type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChecksumType ChecksumType { get; init; }

    [JsonPropertyName("update_time")]
    public DateTime UpdateTime { get; set; }

    [JsonPropertyName("update_log")]
    public string UpdateLog { get; set; }
}

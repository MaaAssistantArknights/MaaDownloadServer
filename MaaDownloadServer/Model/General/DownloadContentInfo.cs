using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.General;

public record DownloadContentInfo
{
    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();

    [JsonPropertyName("version")]
    public string Version { get; init; }

    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; init; }

    [JsonPropertyName("platform")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Platform Platform { get; init; }

    [JsonPropertyName("arch")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Architecture Architecture { get; init; }

    [JsonPropertyName("file_extension")]
    public string FileExtension { get; init; }

    [JsonPropertyName("checksum")]
    public string Checksum { get; init; }

    [JsonPropertyName("checksum_type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChecksumType ChecksumType { get; init; }

    [JsonPropertyName("update_time")]
    public DateTime UpdateTime { get; init; }

    [JsonPropertyName("update_log")]
    public string UpdateLog { get; init; }
}

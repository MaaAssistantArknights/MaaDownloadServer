using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record VersionMetadata(
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("publish_time")] DateTime PublishTime);

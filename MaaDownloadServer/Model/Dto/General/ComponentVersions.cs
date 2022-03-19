using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record ComponentVersions(
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("publish_time")] DateTime PublishTime,
    [property: JsonPropertyName("update_log")] string UpdateLog,
    [property: JsonPropertyName("support")] List<ComponentSupport> Supports);

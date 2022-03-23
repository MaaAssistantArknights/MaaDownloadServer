using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record Announce(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("time")] DateTime Time,
    [property: JsonPropertyName("level"), JsonConverter(typeof(JsonStringEnumConverter))] AnnounceLevel Level,
    [property: JsonPropertyName("issuer")] string Issuer,
    [property: JsonPropertyName("message")] string Message);

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record ExistenceContent(
    [property: JsonPropertyName("exist")] bool Exist,
    [property: JsonPropertyName("open")] DateTime? Open,
    [property: JsonPropertyName("close")] DateTime? Close);

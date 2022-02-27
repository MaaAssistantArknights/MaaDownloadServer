using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record ResourceMetadata(
    [property: JsonPropertyName("file_name")] string FileName,
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("hash")] string Hash);

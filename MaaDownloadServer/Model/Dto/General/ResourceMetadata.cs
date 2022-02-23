using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record ResourceMetadata(string FileName, string Path, string Hash)
{
    [JsonPropertyName("file_name")]
    public string FileName { get; set; } = FileName;

    [JsonPropertyName("path")]
    public string Path { get; set; } = Path;

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = Hash;
}

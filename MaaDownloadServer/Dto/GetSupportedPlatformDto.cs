using System.Text.Json.Serialization;

namespace MaaDownloadServer.Dto;

public record GetSupportedPlatformDto(List<string> SupportPlatform)
{
    [JsonPropertyName("support_platform")]
    public List<string> SupportPlatform { get; set; } = SupportPlatform;
}

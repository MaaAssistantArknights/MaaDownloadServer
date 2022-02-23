using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetSupportedArchDto(string Platform, List<string> SupportArch)
{
    [JsonPropertyName("platform")]
    public string Platform { get; set; } = Platform;

    [JsonPropertyName("support_arch")]
    public List<string> SupportArch { get; set; } = SupportArch;
}

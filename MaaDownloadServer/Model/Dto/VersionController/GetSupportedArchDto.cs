using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetSupportedArchDto(
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("support_arch")] List<string> SupportArch);

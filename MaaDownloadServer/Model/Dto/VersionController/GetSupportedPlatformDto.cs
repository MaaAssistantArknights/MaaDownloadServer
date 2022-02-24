using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record GetSupportedPlatformDto(
    [property: JsonPropertyName("support_platform")] List<string> SupportPlatform);

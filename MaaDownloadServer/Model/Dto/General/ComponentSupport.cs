using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record ComponentSupport(
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("arch")] string Arch);

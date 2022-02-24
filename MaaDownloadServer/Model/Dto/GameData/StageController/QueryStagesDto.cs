using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Dto;

public record QueryStagesDto(
    [property: JsonPropertyName("stages")] List<GetStageDto> Stages,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("limit")] int Limit,
    [property: JsonPropertyName("page")] int Page);

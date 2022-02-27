namespace MaaDownloadServer.Services.Controller.Interfaces;

public interface IArkStageService
{
    Task<GetStageDto> GetStage(string code);
    Task<QueryStagesDto> QueryStages(IReadOnlyDictionary<string, string> query);
}

namespace MaaDownloadServer.Services.Controller.Interfaces;

public interface IComponentService
{
    Task<List<ComponentDto>> GetAllComponents();
    Task<GetComponentDetailDto> GetComponentDetail(string component, int limit, int page);
}

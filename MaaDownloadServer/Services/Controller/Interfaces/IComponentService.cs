namespace MaaDownloadServer.Services.Controller.Interfaces;

public interface IComponentService
{
    Task<List<ComponentDto>> GetAllComponents();
}

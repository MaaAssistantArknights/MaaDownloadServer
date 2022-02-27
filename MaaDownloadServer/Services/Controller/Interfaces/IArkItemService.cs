namespace MaaDownloadServer.Services.Controller.Interfaces;

public interface IArkItemService
{
    Task<GetItemDto> GetItem(string name);
    Task<QueryItemsDto> QueryItems(string name, int limit, int page);
}

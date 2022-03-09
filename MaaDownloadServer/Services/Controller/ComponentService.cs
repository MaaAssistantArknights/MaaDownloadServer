using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Services.Controller;

public class ComponentService : IComponentService
{
    private readonly MaaDownloadServerDbContext _dbContext;

    public ComponentService(MaaDownloadServerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ComponentDto>> GetAllComponents()
    {
        var componentInfosDbCache = await _dbContext.DatabaseCaches
            .Where(x => x.QueryId == "Component")
            .ToListAsync();

        var componentInfos = componentInfosDbCache
            .Select(x => JsonSerializer.Deserialize<ComponentDto>(x.Value))
            .ToList();

        return componentInfos;
    }
}

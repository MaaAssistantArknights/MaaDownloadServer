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

    public async Task<GetComponentDetailDto> GetComponentDetail(string component, int limit, int page)
    {
        var allComponents = await GetAllComponents();
        var componentMetaInfo = allComponents.FirstOrDefault(x => x.Name == component);

        if (componentMetaInfo is null)
        {
            return null;
        }

        var components = (await _dbContext.Packages
            .Where(x => x.Component == component)
            .OrderByDescending(x => x.PublishTime)
            .ToListAsync())
            .GroupBy(x => x.Version.ToString())
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToList();

        var versions = (from c in components
                        let packages = c.ToList()
                        select new ComponentVersions(c.Key, packages[0].PublishTime, packages[0].UpdateLog,
                    packages.Select(x => new ComponentSupport
                            (x.Platform.ToString(), x.Architecture.ToString())).ToList()))
                .ToList();

        var dto = new GetComponentDetailDto(componentMetaInfo.Name, componentMetaInfo.Description, versions, page, limit);
        return dto;
    }
}

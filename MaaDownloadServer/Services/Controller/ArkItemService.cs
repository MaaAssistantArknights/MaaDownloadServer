using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Services.Controller;

public class ArkItemService : IArkItemService
{
    private readonly ILogger<ArkItemService> _logger;
    private readonly ICacheService _cacheService;
    private readonly MaaDownloadServerDbContext _dbContext;

    public ArkItemService(
        ILogger<ArkItemService> logger,
        ICacheService cacheService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<GetItemDto> GetItem(string name)
    {
        var cacheKey = _cacheService.GetGameDataKey(GameDataType.Item, name);
        if (_cacheService.Contains(cacheKey))
        {
            var cachedItem = _cacheService.Get<ArkPrtsItem>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return EntityToDto(cachedItem);
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var item = await _dbContext.ArkPrtsItems.FirstOrDefaultAsync(x => x.Name == name);

        if (item is not null)
        {
            return EntityToDto(item);
        }

        _cacheService.Add(cacheKey, ("NotExist", DateTime.Now), GameDataType.Item);
        return null;
    }

    public async Task<QueryItemsDto> QueryItems(string name, int limit, int page)
    {
        var cacheKey = _cacheService.GetGameDataKey(GameDataType.Item, $"q-{name}-{limit}-{page}");
        if (_cacheService.Contains(cacheKey))
        {
            var (arkPrtsItems, item2) = _cacheService.Get<(List<ArkPrtsItem>, int)>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            var dtos = arkPrtsItems.Select(EntityToDto).ToList();
            return new QueryItemsDto(dtos, item2, limit, page);
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var count = _dbContext.ArkPrtsItems.Count(x => x.Name.Contains(name));
        var pagedItems = await _dbContext.ArkPrtsItems
            .Where(x => x.Name == name)
            .OrderBy(x => x.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        _cacheService.Add(cacheKey, (pagedItems, count), GameDataType.Item);
        var pagedDtos = pagedItems.Select(EntityToDto).ToList();
        return new QueryItemsDto(pagedDtos, count, limit, page);
    }

    private static GetItemDto EntityToDto(ArkPrtsItem item)
    {
        var dto = new GetItemDto
        (item.ItemId, item.Name, item.Description, item.Usage, item.ObtainMethod, item.Rarity, item.Image,
            item.Category);
        return dto;
    }
}

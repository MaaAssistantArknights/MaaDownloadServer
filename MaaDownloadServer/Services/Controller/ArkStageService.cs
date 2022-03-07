using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Services.Controller;

public class ArkStageService : IArkStageService
{
    private readonly ILogger<ArkStageService> _logger;
    private readonly ICacheService _cacheService;
    private readonly MaaDownloadServerDbContext _dbContext;

    public ArkStageService(
        ILogger<ArkStageService> logger,
        ICacheService cacheService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<GetStageDto> GetStage(string code)
    {
        var cacheKey = _cacheService.GetGameDataKey(GameDataType.Stage, code);
        if (_cacheService.Contains(cacheKey))
        {
            var (arkPenguinStage, arkPenguinItems) = _cacheService.Get<(ArkPenguinStage, List<ArkPenguinItem>)>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return EntityToDto(arkPenguinStage, arkPenguinItems);
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var stage = await _dbContext.ArkPenguinStages.FirstOrDefaultAsync(x => x.StageId == code) ??
                    await _dbContext.ArkPenguinStages.FirstOrDefaultAsync(x => x.StageCode == code);

        if (stage is not null)
        {
            var items = new List<ArkPenguinItem>();
            if (stage.DropItemIds.Count != 0)
            {
                items = _dbContext.ArkPenguinItems
                    .Where(x => stage.DropItemIds.Contains(x.ItemId))
                    .ToList();
            }
            _cacheService.Add(_cacheService.GetGameDataKey(GameDataType.Stage, stage.StageId), (stage, items), GameDataType.Stage);
            _cacheService.Add(_cacheService.GetGameDataKey(GameDataType.Stage, stage.StageCode), (stage, items), GameDataType.Stage);
            return EntityToDto(stage, items);
        }

        _cacheService.Add(cacheKey, ("NotExist", DateTime.Now), GameDataType.Stage);
        return null;
    }

    public async Task<QueryStagesDto> QueryStages(IReadOnlyDictionary<string, string> query)
    {
        var limit = 50;
        var page = 1;

        if (query.ContainsKey("limit"))
        {
            var parsedLimit = QueryValueParseToInt32(query["limit"], "limit");
            if (parsedLimit >= 0)
            {
                limit = (int)parsedLimit;
            }
        }

        if (query.ContainsKey("page"))
        {
            var parsedPage = QueryValueParseToInt32(query["page"], "page");
            if (parsedPage >= 0)
            {
                page = (int)parsedPage;
            }
        }

        var validQuerySection = new List<string> { $"page={page}", $"limit={limit}" };

        Expression<Func<ArkPenguinStage, bool>> expression = x => true;
        foreach (var (k, v) in query)
        {
            switch (k)
            {
                case "stage_id":
                    expression = expression.AndAlso(x => x.StageId.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "stage_type":
                    expression = expression.AndAlso(x => x.StageType.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "stage_code":
                    expression = expression.AndAlso(x => x.StageCode.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "ap_cost_lower_limit":
                    var apCostLowerLimit = QueryValueParseToInt32(v, "ap_cost_lower_limit");
                    if (apCostLowerLimit is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression = expression.AndAlso(x => x.StageApCost >= apCostLowerLimit);
                        validQuerySection.Add($"{k}={v}");
                    }
                    break;
                case "ap_cost_up_limit":
                    var apCostUpLimit = QueryValueParseToInt32(v, "ap_cost_up_limit");
                    if (apCostUpLimit is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression = expression.AndAlso(x => x.StageApCost <= apCostUpLimit);
                        validQuerySection.Add($"{k}={v}");
                    }
                    break;
                case "available_cn":
                    var availableCn = QueryValueParseToBoolean(v, "available_cn");
                    if (availableCn is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression = expression.AndAlso(x => x.CnExist == (bool)availableCn);
                        validQuerySection.Add($"{k}={v.ToLower()}");
                    }
                    break;
                case "available_us":
                    var availableUs = QueryValueParseToBoolean(v, "available_cn");
                    if (availableUs is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression = expression.AndAlso(x => x.UsExist == (bool)availableUs);
                        validQuerySection.Add($"{k}={v.ToLower()}");
                    }
                    break;
                case "available_kr":
                    var availableKr = QueryValueParseToBoolean(v, "available_cn");
                    if (availableKr is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression = expression.AndAlso(x => x.KrExist == (bool)availableKr);
                        validQuerySection.Add($"{k}={v.ToLower()}");
                    }
                    break;
                case "available_jp":
                    var availableJp = QueryValueParseToBoolean(v, "available_cn");
                    if (availableJp is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression = expression.AndAlso(x => x.JpExist == (bool)availableJp);
                        validQuerySection.Add($"{k}={v.ToLower()}");
                    }
                    break;
            }
        }

        validQuerySection = validQuerySection.OrderBy(x => x).ToList();

        var cacheKey = _cacheService.GetGameDataKey(GameDataType.Stage, $"p-{string.Join("-", validQuerySection)}");
        if (_cacheService.Contains(cacheKey))
        {
            var queryStagesDto = _cacheService.Get<QueryStagesDto>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return queryStagesDto;
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);

        var count = await _dbContext.ArkPenguinStages.CountAsync(expression);
        var stages = await _dbContext.ArkPenguinStages
            .Where(expression)
            .OrderBy(x => x.StageCode)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        var stageDtos = new List<GetStageDto>();
        foreach (var stage in stages)
        {
            if (stage.DropItemIds.Count != 0)
            {
                var items = _dbContext.ArkPenguinItems
                    .AsNoTracking()
                    .Where(x => stage.DropItemIds.Contains(x.ItemId))
                    .ToList();
                stageDtos.Add(EntityToDto(stage, items));
            }
            else
            {
                stageDtos.Add(EntityToDto(stage, new List<ArkPenguinItem>()));
            }
        }

        var queryDto = new QueryStagesDto(stageDtos, count, limit, page);

        _cacheService.Add(cacheKey, queryDto, GameDataType.Stage);

        return queryDto;
    }

    private static GetStageDto EntityToDto(ArkPenguinStage stage, IEnumerable<ArkPenguinItem> items)
    {
        var dto = new GetStageDto(
            new StageMetadata(stage.StageId, stage.StageType, stage.StageCode, stage.StageApCost, stage.MinClearTime),
            new ArkI18N(stage.ZhStageCodeI18N, stage.KoStageCodeI18N, stage.JaStageCodeI18N, stage.EnStageCodeI18N),
            new Existence(
                new ExistenceContent(stage.CnExist, stage.CnOpenTime, stage.CnCloseTime),
                new ExistenceContent(stage.JpExist, stage.JpOpenTime, stage.JpCloseTime),
                new ExistenceContent(stage.KrExist, stage.KrOpenTime, stage.KrCloseTime),
                new ExistenceContent(stage.UsExist, stage.UsOpenTime, stage.UsCloseTime)),
            items.Select(ToStageDropItem).ToList());
        return dto;
    }

    private static StageDropItem ToStageDropItem(ArkPenguinItem item)
    {
        var dto = new StageDropItem(
            item.ItemId, item.Name, item.SortId, item.Rarity, item.ItemType,
            new Existence(
                new ExistenceContent(item.CnExist, null, null),
                new ExistenceContent(item.JpExist, null, null),
                new ExistenceContent(item.KrExist, null, null),
                new ExistenceContent(item.UsExist, null, null)),
            new ArkI18N(item.ZhNameI18N, item.KoNameI18N, item.JaNameI18N, item.EnNameI18N));
        return dto;
    }

    private bool? QueryValueParseToBoolean(string value, string paramName)
    {
        var isParsed = bool.TryParse(value, out var parsedValue);
        if (isParsed is not false)
        {
            return parsedValue;
        }

        _logger.LogWarning("解析检索 Stage Query 组时出现错误, {p} 无法转换为 Boolean, 将被忽略, 当前值为 {cv}", paramName, value);
        return null;
    }

    private int? QueryValueParseToInt32(string value, string paramName)
    {
        var isParsed = int.TryParse(value, out var parsedValue);
        if (isParsed is not false)
        {
            return parsedValue;
        }

        _logger.LogWarning("解析检索 Stage Query 组时出现错误, {p} 无法转换为 Int32, 将被忽略, 当前值为 {cv}", paramName, value);
        return null;
    }
}

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
        var cacheKey = _cacheService.GetGameDataKey(GameDataType.Item, code);
        if (_cacheService.Contains(cacheKey))
        {
            var cachedItem = _cacheService.Get<ArkPenguinStage>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return EntityToDto(cachedItem);
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var item = await _dbContext.ArkPenguinStages.FirstOrDefaultAsync(x => x.StageCode == code);

        if (item is not null)
        {
            return EntityToDto(item);
        }

        _cacheService.Add(cacheKey, ("NotExist", DateTime.Now), GameDataType.Item);
        return null;
    }

    public async Task<QueryStagesDto> QueryStages(IReadOnlyDictionary<string, string> query)
    {
        var limit = 50;
        var page = 1;

        if (query.ContainsKey("limit"))
        {
            var parsedLimit = QueryValueParseToInt32(query["limit"], "limit");
            if (parsedLimit is not null)
            {
                limit = (int)parsedLimit;
            }
        }

        if (query.ContainsKey("page"))
        {
            var parsedPage = QueryValueParseToInt32(query["page"], "page");
            if (parsedPage is not null)
            {
                page = (int)parsedPage;
            }
        }

        var validQuerySection = new List<string>();

        Expression<Func<ArkPenguinStage, bool>> expression = x => true;
        foreach (var (k, v) in query)
        {
            switch (k)
            {
                case "stage_id":
                    expression.AndAlso(x => x.StageId.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "stage_type":
                    expression.AndAlso(x => x.StageType.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "stage_code":
                    expression.AndAlso(x => x.StageCode.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "ap_cost_lower_limit":
                    var apCostLowerLimit = QueryValueParseToInt32(v, "ap_cost_lower_limit");
                    if (apCostLowerLimit is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression.AndAlso(x => x.StageApCost >= apCostLowerLimit);
                        validQuerySection.Add($"{k}={v}");
                    }
                    break;
                case "ap_cost_up_limit":
                    var apCostUpLimit = QueryValueParseToInt32(v, "ap_cost_lower_limit");
                    if (apCostUpLimit is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression.AndAlso(x => x.StageApCost >= apCostUpLimit);
                        validQuerySection.Add($"{k}={v}");
                    }
                    break;
                case "zone_id":
                    expression.AndAlso(x => x.ZoneId.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "zone_name":
                    expression.AndAlso(x => x.ZoneName.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "zone_type":
                    expression.AndAlso(x => x.ZoneType.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "available_cn":
                    var availableCn = QueryValueParseToBoolean(v, "available_cn");
                    if (availableCn is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression.AndAlso(x => x.CnExist == (bool)availableCn);
                        validQuerySection.Add($"{k}={v.ToLower()}");
                    }
                    break;
                case "available_us":
                    var availableUs = QueryValueParseToBoolean(v, "available_cn");
                    if (availableUs is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression.AndAlso(x => x.UsExist == (bool)availableUs);
                        validQuerySection.Add($"{k}={v.ToLower()}");
                    }
                    break;
                case "available_kr":
                    var availableKr = QueryValueParseToBoolean(v, "available_cn");
                    if (availableKr is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression.AndAlso(x => x.KrExist == (bool)availableKr);
                        validQuerySection.Add($"{k}={v.ToLower()}");
                    }
                    break;
                case "available_jp":
                    var availableJp = QueryValueParseToBoolean(v, "available_cn");
                    if (availableJp is not null)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        expression.AndAlso(x => x.JpExist == (bool)availableJp);
                        validQuerySection.Add($"{k}={v.ToLower()}");
                    }
                    break;
            }
        }

        validQuerySection = validQuerySection.OrderBy(x => x).ToList();

        var cacheKey = _cacheService.GetGameDataKey(GameDataType.Stage, $"p-{string.Join("-", validQuerySection)}");
        if (_cacheService.Contains(cacheKey))
        {
            var (arkPenguinStages, cachedCounts) = _cacheService.Get<(List<ArkPenguinStage>, int)>(cacheKey);
            var cachedStageDtos = arkPenguinStages.Select(EntityToDto).ToList();
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return new QueryStagesDto(cachedStageDtos, cachedCounts, limit, page);
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);

        var count = await _dbContext.ArkPenguinStages.CountAsync(expression);
        var data = await _dbContext.ArkPenguinStages
            .Where(expression)
            .OrderBy(x => x.StageCode)
            .ToListAsync();
        var stages = data.Skip((page - 1) * limit).Take(limit).ToList();

        _cacheService.Add(cacheKey, (stages, count), GameDataType.Stage);

        var stageDtos = stages.Select(EntityToDto).ToList();
        return new QueryStagesDto(stageDtos, count, limit, page);
    }

    private static GetStageDto EntityToDto(ArkPenguinStage stage)
    {
        var dto = new GetStageDto(
            new StageMetadata(stage.StageId, stage.StageType, stage.StageCode, stage.StageApCost),
            new ZoneMetadata(stage.ZoneId, stage.ZoneName, stage.ZoneType),
            new ArkI18N(stage.ZhStageCodeI18N, stage.KoStageCodeI18N, stage.JaStageCodeI18N, stage.EnStageCodeI18N),
            new ArkI18N(stage.ZhZoneNameI18N, stage.KoZoneNameI18N, stage.JaZoneNameI18N, stage.EnZoneNameI18N),
            new Existence(
                new ExistenceContent(stage.CnExist, stage.CnOpenTime, stage.CnCloseTime),
                new ExistenceContent(stage.JpExist, stage.JpOpenTime, stage.JpCloseTime),
                new ExistenceContent(stage.KrExist, stage.KrOpenTime, stage.KrCloseTime),
                new ExistenceContent(stage.UsExist, stage.UsOpenTime, stage.UsCloseTime)));
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

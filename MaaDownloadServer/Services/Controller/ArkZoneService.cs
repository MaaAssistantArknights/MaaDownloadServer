using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Services.Controller;

public class ArkZoneService : IArkZoneService
{
    private readonly ILogger<ArkZoneService> _logger;
    private readonly ICacheService _cacheService;
    private readonly MaaDownloadServerDbContext _dbContext;

    public ArkZoneService(
        ILogger<ArkZoneService> logger,
        ICacheService cacheService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<GetZoneDto> GetZone(string zoneId)
    {
        var cacheKey = _cacheService.GetGameDataKey(GameDataType.Zone, zoneId);
        if (_cacheService.Contains(cacheKey))
        {
            var cachedZone = _cacheService.Get<GetZoneDto>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return cachedZone;
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);
        var zone = await _dbContext.ArkPenguinZones
            .Include(x => x.Stages)
            .FirstOrDefaultAsync(x => x.ZoneId == zoneId);

        if (zone is not null)
        {
            var dto = EntityToDto(zone);
            _cacheService.Add(cacheKey, dto, GameDataType.Zone);
            return dto;
        }

        _cacheService.Add(cacheKey, ("NotExist", DateTime.Now), GameDataType.Item);
        return null;
    }

    public async Task<QueryZoneDto> QueryZones(IReadOnlyDictionary<string, string> query)
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

        Expression<Func<ArkPenguinZone, bool>> expression = x => true;
        foreach (var (k, v) in query)
        {
            switch (k)
            {
                case "zone_id":
                    expression = expression.AndAlso(x => x.ZoneId.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "zone_name":
                    expression = expression.AndAlso(x => x.ZoneName.Contains(v));
                    validQuerySection.Add($"{k}={v}");
                    break;
                case "zone_type":
                    expression = expression.AndAlso(x => x.ZoneType.Contains(v));
                    validQuerySection.Add($"{k}={v}");
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

        var cacheKey = _cacheService.GetGameDataKey(GameDataType.Zone, $"p-{string.Join("-", validQuerySection)}");
        if (_cacheService.Contains(cacheKey))
        {
            var (arkPenguinZones, cachedCounts) = _cacheService.Get<(List<GetZoneDto>, int)>(cacheKey);
            _logger.LogDebug("Cache 命中 - {cacheKey}", cacheKey);
            return new QueryZoneDto(arkPenguinZones, cachedCounts, limit, page);
        }

        _logger.LogWarning("Cache 未命中 - {cacheKey}", cacheKey);

        var count = await _dbContext.ArkPenguinZones.CountAsync(expression);
        var allData = await _dbContext.ArkPenguinZones
            .AsNoTracking()
            .Include(x => x.Stages)
            .Where(expression)
            .OrderBy(x => x.ZoneId)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        var dtos = allData.Select(EntityToDto).ToList();
        _cacheService.Add(cacheKey, (dtos, count), GameDataType.Zone);

        return new QueryZoneDto(dtos, count, limit, page);
    }

    private static GetZoneDto EntityToDto(ArkPenguinZone zone)
    {
        var dto = new GetZoneDto(
            new Existence(
                new ExistenceContent(zone.CnExist, null, null),
                new ExistenceContent(zone.JpExist, null, null),
                new ExistenceContent(zone.KrExist, null, null),
                new ExistenceContent(zone.UsExist, null, null)),
            new ArkI18N(zone.ZhZoneNameI18N, zone.KoZoneNameI18N, zone.JaZoneNameI18N, zone.EnZoneNameI18N),
            zone.Stages.Select(stage =>
                new StageMetadata(stage.StageId, stage.StageType, stage.StageCode, stage.StageApCost, stage.MinClearTime))
                .ToList(),
            new ZoneMetadata(zone.ZoneId, zone.ZoneName, zone.ZoneType,
                string.IsNullOrEmpty(zone.Background) ? null : zone.BackgroundFileName));
        return dto;
    }

    private bool? QueryValueParseToBoolean(string value, string paramName)
    {
        var isParsed = bool.TryParse(value, out var parsedValue);
        if (isParsed is not false)
        {
            return parsedValue;
        }

        _logger.LogWarning("解析检索 Zone Query 组时出现错误, {p} 无法转换为 Boolean, 将被忽略, 当前值为 {cv}", paramName, value);
        return null;
    }

    private int? QueryValueParseToInt32(string value, string paramName)
    {
        var isParsed = int.TryParse(value, out var parsedValue);
        if (isParsed is not false)
        {
            return parsedValue;
        }

        _logger.LogWarning("解析检索 Zone Query 组时出现错误, {p} 无法转换为 Int32, 将被忽略, 当前值为 {cv}", paramName, value);
        return null;
    }
}

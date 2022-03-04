using Quartz;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Jobs;

public class GameDataUpdateJob : IJob
{
    private readonly ILogger<GameDataUpdateJob> _logger;
    private readonly ICacheService _cacheService;
    private readonly MaaDownloadServerDbContext _dbContext;

    private readonly DirectoryInfo _itemDirectory;
    private readonly DirectoryInfo _zoneDirectory;

    public GameDataUpdateJob(
        ILogger<GameDataUpdateJob> logger,
        IConfigurationService configurationService,
        ICacheService cacheService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _cacheService = cacheService;
        _dbContext = dbContext;

        var itemImageDirectoryPath = Path.Combine(configurationService.GetGameDataDirectory(), "items");
        var zoneImageDirectoryPath = Path.Combine(configurationService.GetGameDataDirectory(), "zones");
        _itemDirectory = new DirectoryInfo(itemImageDirectoryPath);
        _zoneDirectory = new DirectoryInfo(zoneImageDirectoryPath);

        if (_itemDirectory.Exists is false)
        {
            _itemDirectory.Create();
        }

        if (_zoneDirectory.Exists is false)
        {
            _zoneDirectory.Create();
        }
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("开始更新游戏数据");

        _logger.LogInformation("游戏数据更新: 开始更新企鹅物流数据站区域、关卡、掉落物信息");
        await PenguinZonesUpdate();
        _logger.LogInformation("游戏数据更新: 开始更新 PRTS 物品信息");
        await PrtsItemUpdate();
    }

    private async Task PrtsItemUpdate()
    {
        try
        {
            #region 获取和解析 PRTS Wiki 道具页面 Html

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://prts.wiki/");
            var response = await client.GetAsync("index.php?title=道具一览");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode is false)
            {
                throw new HttpRequestException($"请求失败, Status: {response.StatusCode}, Content: {content}");
            }

            var html = new HtmlDocument();
            html.LoadHtml(content);
            var doc = html.DocumentNode;
            // ReSharper disable once StringLiteralTypo
            var nodes = doc.QuerySelectorAll(".smwdata").ToList();

            _logger.LogInformation("获取和解析 PRTS Wiki 道具页面成功, 共 {count} 个节点", nodes.Count);

            #endregion

            var downloadContentInfo = new List<ItemImageDownloadInfo>();
            var prtsItems = new List<ArkPrtsItem>();

            // 放弃使用 AsParallel
            // AsParallel 似乎会漏数据, 500 个节点解析后会丢失 1 - 10 个
            foreach (var node in nodes)
            {
                var id = node.Attributes["data-id"].Value;
                var name = node.Attributes["data-name"].Value;
                var description = node.Attributes["data-description"].Value;
                var usage = node.Attributes["data-usage"].Value;
                var obtain = node.Attributes["data-obtain_approach"].Value;
                var rarityString = node.Attributes["data-rarity"].Value;
                var fileDownloadUrlWithoutProtocol = node.Attributes["data-file"].Value;
                var categoryString = node.Attributes["data-category"].Value;

                var fileDownloadUrl = "";
                if (fileDownloadUrlWithoutProtocol is not (null or ""))
                {
                    fileDownloadUrl = "https:" + fileDownloadUrlWithoutProtocol;
                }
                else
                {
                    _logger.LogWarning("更新 Items 时出现错误, 物品 {itemName} 不存在图片", name);
                }

                var file = $"{name}.png";

                var category = categoryString
                    .Split(",")
                    .Select(x => x.Trim())
                    .Select(x => x.Replace("分类:", ""))
                    .ToList();

                var rarityParsed = int.TryParse(rarityString, out var rarity);
                if (rarityParsed is false)
                {
                    rarity = -1;
                }

                var arkPrtsItem = new ArkPrtsItem
                {
                    Id = Guid.NewGuid(),
                    ItemId = id,
                    Name = name,
                    Description = description,
                    Usage = usage,
                    ObtainMethod = obtain,
                    Rarity = rarity,
                    Image = file,
                    ImageDownloadUrl = fileDownloadUrl,
                    Category = category
                };

                prtsItems.Add(arkPrtsItem);
            }

            foreach (var item in prtsItems)
            {
                var existed = await _dbContext.ArkPrtsItems.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name == item.Name && x.ItemId == item.ItemId);

                if (existed is null)
                {
                    await _dbContext.ArkPrtsItems.AddAsync(item);
                }
                else
                {
                    if (existed.Equal(item))
                    {
                        continue;
                    }

                    if (File.Exists(Path.Combine(_itemDirectory.FullName, existed.Image)))
                    {
                        File.Delete(Path.Combine(_itemDirectory.FullName, existed.Image));
                    }

                    _logger.LogDebug("旧物品数据: {ori}", existed);
                    _logger.LogDebug("新物品数据: {new}", item);

                    var cId = existed.Id;

                    existed = item with { Id = cId };
                    _dbContext.ArkPrtsItems.Update(existed);
                }

                if (item.ImageDownloadUrl is not (null or ""))
                {
                    downloadContentInfo.Add(new ItemImageDownloadInfo(item.ImageDownloadUrl, Path.Combine(_itemDirectory.FullName, item.Image)));
                }
            }

            var downloadHttpClient = new HttpClient();

            downloadContentInfo.AsParallel().ForAll(x =>
            {
                var (downloadUrl, filePath) = x;
                var responseResult = downloadHttpClient.GetAsync(downloadUrl);
                using var memStream = responseResult.Result.Content.ReadAsStreamAsync().Result;
                using var fileStream = File.Create(filePath);
                memStream.CopyTo(fileStream);
                _logger.LogDebug("更新 Ark PRTS Item 数据, 已下载: {fn}", filePath);
            });

            var addOrModify = await _dbContext.SaveChangesAsync();

            var removedItems = _dbContext.ArkPrtsItems
                .ToList()
                .Where(x => prtsItems.Exists(y => y.ItemId == x.ItemId && y.Name == x.Name) is false)
                .ToList();

            _dbContext.ArkPrtsItems.RemoveRange(removedItems);
            var deleted = await _dbContext.SaveChangesAsync();

            foreach (var removedItem in removedItems
                         .Where(removedItem => File.Exists(Path.Combine(_itemDirectory.FullName, removedItem.Image))))
            {
                _logger.LogInformation("Ark PRTS Items 删除数据: {del}", removedItem);
                File.Delete(Path.Combine(_itemDirectory.FullName, removedItem.Image));
            }

            _logger.LogInformation("更新 Ark PRTS Item 成功, 添加或更新条目 {au} 个, 删除条目 {d} 个", addOrModify, deleted);

            // 删缓存
            if (deleted != 0 || addOrModify != 0)
            {
                _cacheService.RemoveAll(GameDataType.Item);
                _logger.LogInformation("已删除 PRTS Item 组缓存");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("更新 PRTS Item 失败, Exception: {ex}", ex);
        }
    }

    private async Task PenguinZonesUpdate()
    {
        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://penguin-stats.io/PenguinStats/api/v2/");
            var items = await client.GetFromJsonAsync<List<ApiPenguinItem>>("items");
            var stages = await client.GetFromJsonAsync<List<ApiPenguinStage>>("stages");
            var zones = await client.GetFromJsonAsync<List<ApiPenguinZone>>("zones");

            if (zones is null || stages is null || items is null)
            {
                throw new NullReferenceException();
            }

            var bigZones = GameDataUpdateUtil.BuildZones(zones, stages);

            // Update Items
            var arkItems = items.Select(x => x.ToArkPenguinItem()).ToList();
            foreach (var item in arkItems)
            {
                var existed = await _dbContext.ArkPenguinItems.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ItemId == item.ItemId);

                if (existed is null)
                {
                    await _dbContext.ArkPenguinItems.AddAsync(item);
                }
                else
                {
                    if (existed == item)
                    {
                        continue;
                    }

                    _logger.LogDebug("旧企鹅物流物品数据: {ori}", existed);
                    _logger.LogDebug("新企鹅物流物品数据: {new}", item);

                    existed = item;
                    _dbContext.ArkPenguinItems.Update(existed);
                }
            }

            var addOrModifiedItemsChange = await _dbContext.SaveChangesAsync();

            var removedItems = _dbContext.ArkPenguinItems
                .ToList()
                .Where(x => arkItems.Exists(y => y.ItemId == x.ItemId) is false)
                .ToList();

            _dbContext.ArkPenguinItems.RemoveRange(removedItems);
            var removeItemsChange = await _dbContext.SaveChangesAsync();

            _logger.LogInformation("更新 Ark Penguin Item 成功, 添加或更新条目 {au} 个, 删除条目 {d} 个",
                addOrModifiedItemsChange, removeItemsChange);

            // Update Zones and Stages
            var backgroundDownloadFiles = new Dictionary<string, string>();
            foreach (var zone in bigZones)
            {
                var existed = await _dbContext.ArkPenguinZones.AsNoTracking()
                    .Include(x => x.Stages)
                    .FirstOrDefaultAsync(x => x.ZoneId == zone.ZoneId);

                if (existed is null)
                {
                    await _dbContext.ArkPenguinZones.AddAsync(zone);
                }
                else
                {
                    if (zone.EqualTo(existed))
                    {
                        continue;
                    }

                    if (File.Exists(Path.Combine(_zoneDirectory.FullName, existed.BackgroundFileName)))
                    {
                        File.Delete(Path.Combine(_zoneDirectory.FullName, existed.BackgroundFileName));
                    }

                    _logger.LogDebug("旧企鹅物流区域数据: {ori}", existed);
                    _logger.LogDebug("新企鹅物流区域数据: {new}", zone);

                    existed = zone;
                    _dbContext.ArkPenguinZones.Update(existed);
                }

                if (string.IsNullOrEmpty(zone.Background) is false)
                {
                    backgroundDownloadFiles.Add(Path.Combine(_zoneDirectory.FullName, zone.BackgroundFileName),
                        "https://penguin-stats.s3.amazonaws.com" + zone.Background);
                }
            }

            var downloadHttpClient = new HttpClient();

            backgroundDownloadFiles.AsParallel().ForAll(x =>
            {
                x.Deconstruct(out var filePath, out var downloadUrl);
                var responseResult = downloadHttpClient.GetAsync(downloadUrl);
                using var memStream = responseResult.Result.Content.ReadAsStreamAsync().Result;
                using var fileStream = File.Create(filePath);
                memStream.CopyTo(fileStream);
                _logger.LogDebug("更新 Ark Penguin Zone 数据, 已下载: {fn}", filePath);
            });

            var addOrModifiedZoneChange = await _dbContext.SaveChangesAsync();

            var removedZones = _dbContext.ArkPenguinZones
                .ToList()
                .Where(x => bigZones.Exists(y => y.ZoneId == x.ZoneId) is false)
                .ToList();

            _dbContext.ArkPenguinZones.RemoveRange(removedZones);
            var removeZonesChange = await _dbContext.SaveChangesAsync();

            _logger.LogInformation("更新 Ark Penguin Zones 成功, 添加或更新条目 {au} 个, 删除条目 {d} 个",
                addOrModifiedZoneChange, removeZonesChange);

            // 检查 Stages
            var removedStages = _dbContext.ArkPenguinStages
                .ToList()
                .Where(x => stages.Exists(y => y.StageId == x.StageId) is false)
                .ToList();
            _dbContext.RemoveRange(removedStages);
            var removeStageChange = await _dbContext.SaveChangesAsync();

            _logger.LogInformation("删除无引用 Ark Penguin Stages 成功, 删除条目 {d} 个", removeStageChange);
        }
        catch (Exception ex)
        {
            _logger.LogError("更新 Ark Stage 失败, Exception: {ex}", ex);
        }
    }

    #region PRTS.Wiki 物品信息获取工具类和函数

    private record ItemImageDownloadInfo(string DownloadUrl, string SavePath);

    #endregion
}

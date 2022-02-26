using Quartz;
using HtmlAgilityPack;
using System.Text.Json.Serialization;
using Fizzler.Systems.HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Jobs;

public class GameDataUpdateJob : IJob
{
    private readonly ILogger<GameDataUpdateJob> _logger;
    private readonly ICacheService _cacheService;
    private readonly MaaDownloadServerDbContext _dbContext;

    private readonly DirectoryInfo _itemDirectory;

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
        _itemDirectory = new DirectoryInfo(itemImageDirectoryPath);
        if (_itemDirectory.Exists is false)
        {
            _itemDirectory.Create();
        }
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("开始更新游戏数据");

        _logger.LogInformation("游戏数据更新: 开始更新关卡信息");
        await StageUpdate();
        _logger.LogInformation("游戏数据更新: 开始更新物品信息");
        await ItemUpdate();
    }

    private async Task ItemUpdate()
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

            nodes.AsParallel().ForAll(node =>
            {
                var id = node.Attributes["data-id"].Value;
                var name = node.Attributes["data-name"].Value;
                var description = node.Attributes["data-description"].Value;
                var usage = node.Attributes["data-usage"].Value;
                var obtain = node.Attributes["data-obtain_approach"].Value;
                var rarityString = node.Attributes["data-rarity"].Value;
                var fileDownloadUrl = "https:" + node.Attributes["data-file"].Value;
                var categoryString = node.Attributes["data-category"].Value;

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
            });

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
                    if (existed == item)
                    {
                        continue;
                    }

                    if (File.Exists(Path.Combine(_itemDirectory.FullName, existed.Image)))
                    {
                        File.Delete(Path.Combine(_itemDirectory.FullName, existed.Image));
                    }

                    var cId = existed.Id;

                    existed = item with { Id = cId };
                    _dbContext.ArkPrtsItems.Update(existed);
                }

                downloadContentInfo.Add(new ItemImageDownloadInfo(item.ImageDownloadUrl, Path.Combine(_itemDirectory.FullName, item.Image)));
            }

            var downloadHttpClient = new HttpClient();

            downloadContentInfo.AsParallel().ForAll(x =>
            {
                var (downloadUrl, filePath) = x;
                var responseResult = downloadHttpClient.GetAsync(downloadUrl);
                using var memStream = responseResult.Result.Content.ReadAsStreamAsync().Result;
                using var fileStream = File.Create(filePath);
                memStream.CopyTo(fileStream);
                _logger.LogDebug("更新 Ark Item 数据, 已下载: {fn}", filePath);
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
                File.Delete(Path.Combine(_itemDirectory.FullName, removedItem.Image));
            }

            _logger.LogInformation("更新 Ark Stage 成功, 添加或更新条目 {au} 个, 删除条目 {d} 个", addOrModify, deleted);

            // 删缓存
            if (deleted != 0 || addOrModify != 0)
            {
                _cacheService.RemoveAll(GameDataType.Item);
                _logger.LogInformation("已删除 Item 组缓存");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("更新 Item 失败, Exception: {ex}", ex);
        }
    }

    private async Task StageUpdate()
    {
        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://penguin-stats.io/PenguinStats/api/v2/");
            var stages = await client.GetFromJsonAsync<List<ApiPenguinStage>>("stages");
            var zones = await client.GetFromJsonAsync<List<ApiPenguinZone>>("zones");

            // Add or Modify Stages
            foreach (var stage in stages!)
            {
                var existed = await _dbContext.ArkPenguinStages.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.StageId == stage.StageId);
                var zone = zones!.FirstOrDefault(x => x.ZoneId == stage.ZoneId);
                if (zone is null)
                {
                    throw new Exception($"Zone {stage.ZoneId} not found");
                }

                var currentArkStage = stage.ToArkPenguinStage(zone);
                if (existed is null)
                {
                    await _dbContext.ArkPenguinStages.AddAsync(currentArkStage);
                }
                else
                {
                    if (existed == currentArkStage)
                    {
                        continue;
                    }

                    existed = currentArkStage;
                    _dbContext.ArkPenguinStages.Update(existed);
                }
            }
            var addOrModify = await _dbContext.SaveChangesAsync();

            // Remove Extra Stages
            var removedStages = _dbContext.ArkPenguinStages
                .ToList()
                .Where(x => stages.Exists(y => y.StageId == x.StageId) is false)
                .ToList();

            _dbContext.ArkPenguinStages.RemoveRange(removedStages);
            var deleted = await _dbContext.SaveChangesAsync();

            _logger.LogInformation("更新 Ark Stage 成功, 添加或更新条目 {au} 个, 删除条目 {d} 个", addOrModify, deleted);

            // 删缓存
            if (deleted != 0 || addOrModify != 0)
            {
                _cacheService.RemoveAll(GameDataType.Item);
                _logger.LogInformation("已删除 Stage 与 Zone 组缓存");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("更新 Ark Stage 失败, Exception: {ex}", ex);
        }
    }

    #region 企鹅物流关卡和区域信息 API 模型

    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable UnusedAutoPropertyAccessor.Global

    internal record ApiPenguinZone
    {
        [JsonPropertyName("zoneId")] public string ZoneId { get; set; }
        [JsonPropertyName("type")] public string ZoneType { get; set; }
        [JsonPropertyName("zoneName")] public string ZoneName { get; set; }
        [JsonPropertyName("existence")] public ApiPenguinExistence Existence { get; set; }
        [JsonPropertyName("stages")] public List<string> Stages { get; set; }
        [JsonPropertyName("zoneName_i18n")] public ApiPenguinI18N ZoneNameI18N { get; set; }
        [JsonExtensionData] public Dictionary<string, object> ExtensionData { get; set; }
    }

    internal record ApiPenguinStage
    {
        [JsonPropertyName("stageType")] public string StageType { get; set; }
        [JsonPropertyName("stageId")] public string StageId { get; set; }
        [JsonPropertyName("zoneId")] public string ZoneId { get; set; }
        [JsonPropertyName("code")] public string StageCode { get; set; }
        [JsonPropertyName("apCost")] public int StageApCost { get; set; }
        [JsonPropertyName("existence")] public ApiPenguinExistence Existence { get; set; }
        [JsonPropertyName("code_i18n")] public ApiPenguinI18N CodeI18N { get; set; }
        [JsonExtensionData] public Dictionary<string, object> ExtensionData { get; set; }
    }

    internal record ApiPenguinExistence
    {
        [JsonPropertyName("US")] public ApiPenguinExistenceContent Us { get; set; }
        [JsonPropertyName("JP")] public ApiPenguinExistenceContent Jp { get; set; }
        [JsonPropertyName("CN")] public ApiPenguinExistenceContent Cn { get; set; }
        [JsonPropertyName("KR")] public ApiPenguinExistenceContent Kr { get; set; }
    }

    internal record ApiPenguinExistenceContent
    {
        [JsonPropertyName("exist")] public bool Exist { get; set; }
        [JsonPropertyName("openTime")] public long? OpenTime { get; set; } = null;
        [JsonPropertyName("closeTime")] public long? CloseTime { get; set; } = null;
    }

    internal record ApiPenguinI18N
    {
        [JsonPropertyName("ko")] public string Korean { get; set; }
        [JsonPropertyName("ja")] public string Japanese { get; set; }
        [JsonPropertyName("en")] public string English { get; set; }
        [JsonPropertyName("zh")] public string Chinese { get; set; }
    }

    // ReSharper restore ClassNeverInstantiated.Global
    // ReSharper restore UnusedAutoPropertyAccessor.Global
    #endregion

    #region PRTS.Wiki 物品信息获取工具类和函数

    private record ItemImageDownloadInfo(string DownloadUrl, string SavePath);

    #endregion
}

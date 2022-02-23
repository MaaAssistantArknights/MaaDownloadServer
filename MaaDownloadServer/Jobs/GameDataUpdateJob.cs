using Quartz;
using HtmlAgilityPack;
using System.Reflection;
using Fizzler.Systems.HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Jobs;

public class GameDataUpdateJob : IJob
{
    private readonly ILogger<GameDataUpdateJob> _logger;
    private readonly MaaDownloadServerDbContext _dbContext;
    private readonly string _dataDirectoryPath;

    public GameDataUpdateJob(
        ILogger<GameDataUpdateJob> logger,
        IConfigurationService configurationService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        // TODO: DO NOT USE RESOURCE DIRECTORY
        _dataDirectoryPath = Path.Combine(configurationService.GetResourcesDirectory(), "gamedata");
        if (Directory.Exists(_dataDirectoryPath) is false)
        {
            Directory.CreateDirectory(_dataDirectoryPath);
        }

        var itemImageDirectoryPath = Path.Combine(_dataDirectoryPath, "items");
        if (Directory.Exists(itemImageDirectoryPath) is false)
        {
            Directory.CreateDirectory(itemImageDirectoryPath);
        }
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ItemUpdate();
        await StageUpdate();
    }

    private record ApiZone
    {
        public string ZoneId { get; init; } = default!;
        public int ZoneIndex { get; init; }
        public string Type { get; init; } = default!;
        public string SubType { get; init; }
        public string ZoneName { get; init; } = default!;
        public IList<string> Stages { get; init; } = default!;
        public string Background { get; init; } = default!;
    }

    private record ApiStage
    {
        internal record ApiExistence
        {
            internal record Existence
            {
                public bool Exist { get; init; } = default;
                public long? OpenTime { get; init; }
                public long? CloseTime { get; init; }
            }

            public Existence Us { get; init; } = default;
            public Existence Jp { get; init; } = default;
            public Existence Cn { get; init; } = default;
            public Existence Kr { get; init; } = default;
        }

        public string StageType { get; init; } = default;
        public string StageId { get; init; } = default;
        public string ZoneId { get; init; } = default;
        public string Code { get; init; } = default;
        public int ApCost { get; init; } = default;
        public ApiExistence Existence { get; init; }
        public int MinClearTime { get; init; } = default;
    }

    private async Task ItemUpdate()
    {
        try
        {
            using var client = new HttpClient(new HttpClientHandler());
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

            nodes.AsParallel().ForAll(node =>
            {
                var attrs = node.Attributes;
                var itemId = Convert.ToInt32(attrs["data-id"].Value);
                var name = attrs["data-name"].Value;
                lock (nodes)
                {
                    if (_dbContext.ArkItems.FirstOrDefault(item => item.Name == name) is not null)
                    {
                        return;
                    }
                }

                var url = $"https:{attrs["data-file"].Value}";

                using var httpClient = new HttpClient();
                var filePath = Path.Combine(_dataDirectoryPath, $"items/{name}.png");
                try
                {
                    var responseResult = httpClient.GetAsync(url);
                    using var memStream = responseResult.Result.Content.ReadAsStreamAsync().Result;
                    using var fileStream = File.Create(filePath);
                    memStream.CopyTo(fileStream);
                }
                catch (Exception)
                {
                    // Ignore
                    // TODO: Handle internal error
                }

                var item = new ArkItem(
                    itemId,
                    attrs["data-name"].Value,
                    attrs["data-description"].Value,
                    attrs["data-usage"].Value,
                    attrs["data-obtain_approach"].Value,
                    Convert.ToInt32(attrs["data-rarity"].Value),
                    $"{attrs["data-name"].Value}.png",
                    attrs["data-category"].Value
                );
                lock (nodes)
                {
                    _dbContext.ArkItems.Add(item);
                }
            });
            lock (nodes)
            {
                _dbContext.SaveChangesAsync().Wait();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("更新 Item 失败, Exception: {ex}", ex);
        }
    }

    private async Task StageUpdate()
    {
        using var client = new HttpClient(new HttpClientHandler());
        client.BaseAddress = new Uri("https://penguin-stats.io");

        var zones = (await client.GetFromJsonAsync<IList<ApiZone>>("/PenguinStats/api/v2/zones"))?.ToList();
        var stages = (await client.GetFromJsonAsync<IList<ApiStage>>("/PenguinStats/api/v2/stages"))?.ToList();

        if (stages is null)
        {
            return;
        }

        stages.AsParallel().ForAll(stage =>
        {
            ArkStage arkStage = new(
                stage.StageType,
                stage.ZoneId,
                stage.StageId,
                zones?.Find(zone => zone.ZoneId == stage.ZoneId)?.ZoneName ?? "",
                stage.Code,
                stage.ApCost,
                TimeStringify(stage.Existence?.Cn?.OpenTime, TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")),
                TimeStringify(stage.Existence?.Cn?.CloseTime, TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")),
                stage.Existence?.Cn?.Exist ?? false,
                TimeStringify(stage.Existence?.Jp?.OpenTime, TimeZoneInfo.FindSystemTimeZoneById("Japan")),
                TimeStringify(stage.Existence?.Jp?.CloseTime, TimeZoneInfo.FindSystemTimeZoneById("Japan")),
                stage.Existence?.Jp?.Exist ?? false
            );
            lock (stages)
            {
                var record = _dbContext.ArkStages.FirstOrDefault(v => stage.StageId == v.StageId);
                if (record is null)
                {
                    _dbContext.ArkStages.Add(arkStage);
                }
                else
                {
                    arkStage.Id = record.Id;
                    if (record.GetHashCode() == arkStage.GetHashCode())
                    {
                        return;
                    }

                    UpdateWithDiff(ref record, arkStage);
                    _dbContext.SaveChanges();
                }
            }
        });
        lock (stages)
        {
            _dbContext.SaveChangesAsync().Wait();
        }
    }

    private static void UpdateWithDiff<T>(ref T target, T source)
    {
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var targetValue = property.GetValue(target, null);
            var sourceValue = property.GetValue(source, null);

            if (targetValue != sourceValue && (targetValue is null || !targetValue.Equals(sourceValue)))
            {
                property.SetValue(target, sourceValue);
                // Console.WriteLine(string.Format("Property {0} changed from {1} to {2}", property.Name, targetValue, sourceValue));
            }
        }
    }

    private static string TimeStringify(long? timestamp, TimeZoneInfo zoneInfo)
    {
        if (timestamp is null)
        {
            return null;
        }

        var utc = DateTimeOffset.FromUnixTimeMilliseconds((long)timestamp).DateTime;
        utc += zoneInfo.BaseUtcOffset;
        return utc.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
    }
}

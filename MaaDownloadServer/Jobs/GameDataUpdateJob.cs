using Quartz;
using HtmlAgilityPack;
using System.Reflection;
using Fizzler.Systems.HtmlAgilityPack;
namespace MaaDownloadServer.Jobs;

public class GameDataUpdateJob : IJob
{
    private readonly ILogger<GameDataUpdateJob> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly MaaDownloadServerDbContext _dbContext;
    private readonly string _dataDirectoryPath;
    private record ApiZone
    {
        public string zoneId { get; init; } = default!;
        public int zoneIndex { get; init; }
        public string type { get; init; } = default!;
        public string? subType { get; init; }
        public string zoneName { get; init; } = default!;
        public IList<string> stages { get; init; } = default!;
        public string background { get; init; } = default!;
    }

    private record ApiStage
    {
        internal record ApiExistence
        {
            internal record Existence
            {
                public bool exist { get; init; } = default!;
                public Int64? openTime { get; init; }
                public Int64? closeTime { get; init; }
            }
            public Existence US { get; init; } = default!;
            public Existence JP { get; init; } = default!;
            public Existence CN { get; init; } = default!;
            public Existence KR { get; init; } = default!;
        }
        public string stageType { get; init; } = default!;
        public string stageId { get; init; } = default!;
        public string zoneId { get; init; } = default!;
        public string code { get; init; } = default!;
        public int apCost { get; init; } = default!;
        public ApiExistence? existence { get; init; }
        public int minClearTime { get; init; } = default!;
    }
    public GameDataUpdateJob(
        ILogger<GameDataUpdateJob> logger,
        IConfigurationService configurationService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _configurationService = configurationService;
        _dbContext = dbContext;
        _dataDirectoryPath = Path.Combine(_configurationService.GetResourcesDirectory(), "gamedata");
        if (!Directory.Exists(_dataDirectoryPath))
        {
            Directory.CreateDirectory(_dataDirectoryPath);
        }

        var itemImageDirectoryPath = Path.Combine(_dataDirectoryPath, "items");
        if (!Directory.Exists(itemImageDirectoryPath))
        {
            Directory.CreateDirectory(itemImageDirectoryPath);
        }
    }

    private void ItemUpdate()
    {
        var client = new HttpClient(new HttpClientHandler());
        client.BaseAddress = new Uri("https://prts.wiki/");
        var response = client.GetAsync("index.php?title=道具一览").Result;
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            _logger.LogError($"Error when Get {response.RequestMessage.RequestUri} {response.StatusCode}");
        }
        var results = response.Content.ReadAsStringAsync().Result;
        var html = new HtmlDocument();
        html.LoadHtml(results);
        var doc = html.DocumentNode;
        var nodes = doc.QuerySelectorAll(".smwdata");

        nodes.AsParallel().ForAll(node =>
        {
            var attrs = node.Attributes;
            var itemId = Convert.ToInt32(attrs["data-id"].Value);
            var name = attrs["data-name"].Value;
            lock (nodes)
            {
                if (_dbContext.ArkItems.FirstOrDefault(item => item.Name == name) != null)
                    return;
            }

            var url = String.Format("https:{0}", attrs["data-file"].Value);

            using (var httpClient = new HttpClient())
            {
                var filePath = Path.Combine(_dataDirectoryPath, $"items/{name}.png");
                try
                {
                    var responseResult = httpClient.GetAsync(url);
                    using var memStream = responseResult.Result.Content.ReadAsStreamAsync().Result;
                    using var fileStream = File.Create(filePath);
                    memStream.CopyTo(fileStream);
                }
                catch (System.Exception)
                {

                }
            }

            ArkItem item = new(
                itemId,
                attrs["data-name"].Value,
                attrs["data-description"].Value,
                attrs["data-usage"].Value,
                attrs["data-obtain_approach"].Value,
                Convert.ToInt32(attrs["data-rarity"].Value),
                String.Format("{0}.png", attrs["data-name"].Value),
                attrs["data-category"].Value
            );
            _dbContext.ArkItems.AddAsync(item);
        });
        _dbContext.SaveChanges();
    }

    private async Task StageUpdate()
    {
        var client = new HttpClient(new HttpClientHandler());
        client.BaseAddress = new Uri("https://penguin-stats.io");

        var zones = (await client.GetFromJsonAsync<IList<ApiZone>>("/PenguinStats/api/v2/zones"))?.ToList();
        var stages = (await client.GetFromJsonAsync<IList<ApiStage>>("/PenguinStats/api/v2/stages"))?.ToList();

        var TimeStringify = (Int64? timestamp, TimeZoneInfo zoneInfo) =>
        {
            if (timestamp is null)
            {
                return null;
            }
            var utc = DateTimeOffset.FromUnixTimeMilliseconds(timestamp ?? 0).DateTime;
            utc += zoneInfo.BaseUtcOffset;
            return utc.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
        };

        stages?.AsParallel().ForAll(stage =>
        {
            ArkStage arkStage = new(
                stage.stageType,
                stage.zoneId,
                stage.stageId,
                zones?.Find(zone => zone.zoneId == stage.zoneId)?.zoneName ?? "",
                stage.code,
                stage.apCost,
                TimeStringify(stage.existence?.CN?.openTime, TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")),
                TimeStringify(stage.existence?.CN?.closeTime, TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")),
                stage.existence?.CN.exist ?? false,
                TimeStringify(stage.existence?.JP?.openTime, TimeZoneInfo.FindSystemTimeZoneById("Japan")),
                TimeStringify(stage.existence?.JP?.closeTime, TimeZoneInfo.FindSystemTimeZoneById("Japan")),
                stage.existence?.JP.exist ?? false
            );
            lock (stages)
            {
                var record = _dbContext.ArkStages.FirstOrDefault(v => stage.stageId == v.StageId);
                if (record is null)
                {
                    _dbContext.ArkStages.AddAsync(arkStage);
                }
                else
                {
                    arkStage.Id = record.Id;
                    if (record.GetHashCode() != arkStage.GetHashCode())
                    {
                        UpdateWithDiff(ref record, arkStage);
                        _dbContext.SaveChanges();
                    }
                }
            }
        });
        _dbContext.SaveChanges();
    }

    private static void UpdateWithDiff<T>(ref T target, T source)
    {
        PropertyInfo[] properties = typeof(T).GetProperties();
        foreach (PropertyInfo property in properties)
        {
            object? targetv = property.GetValue(target, null);
            object? sourcev = property.GetValue(source, null);

            if (targetv != sourcev && (targetv == null || !targetv.Equals(sourcev)))
            {
                property.SetValue(target, sourcev);
                // Console.WriteLine(string.Format("Property {0} changed from {1} to {2}", property.Name, targetv, sourcev));
            }
        }
    }

    public async Task Execute(IJobExecutionContext context)
    {
        ItemUpdate();
        await StageUpdate();
    }
}

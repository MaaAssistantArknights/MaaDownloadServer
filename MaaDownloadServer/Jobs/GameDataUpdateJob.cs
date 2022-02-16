using Quartz;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace MaaDownloadServer.Jobs;

public class GameDataUpdateJob : IJob
{

    private readonly ILogger<GameDataUpdateJob> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly MaaDownloadServerDbContext _dbContext;
    private readonly string _dataDirectoryPath;

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

    private void PtrsGet()
    {
        var client = new HttpClient(new HttpClientHandler());
        client.BaseAddress = new Uri("https://prts.wiki/");
        HttpResponseMessage response = client.GetAsync("index.php?title=道具一览").Result;
        response.EnsureSuccessStatusCode();
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
            lock (this)
            {
                if (_dbContext.ArkItems.Where(item => item.Name == name).Count() != 0)
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
                catch (System.Exception) {
                    
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
    public Task Execute(IJobExecutionContext context)
    {
        PtrsGet();
        return Task.CompletedTask;
    }
}

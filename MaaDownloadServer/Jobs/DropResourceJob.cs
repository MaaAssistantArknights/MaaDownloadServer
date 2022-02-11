using Quartz;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace MaaDownloadServer.Jobs;

public class DropResourceJob : IJob
{

    private readonly ILogger<DropResourceJob> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly MaaDownloadServerDbContext _dbContext;
    private readonly string _itemDirectoryPath;

    public DropResourceJob(
        ILogger<DropResourceJob> logger,
        IConfigurationService configurationService,
        MaaDownloadServerDbContext dbContext)
    {
        _logger = logger;
        _configurationService = configurationService;
        _dbContext = dbContext;
        _itemDirectoryPath = Path.Combine(_configurationService.GetPublicDirectory(), "resource/item");
        if (!Directory.Exists(_itemDirectoryPath))
        {
            Directory.CreateDirectory(_itemDirectoryPath);
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
        foreach (var node in nodes)
        {
            var attrs = node.Attributes;
            var itemId = Convert.ToInt32(attrs["data-id"].Value);
            var name = attrs["data-name"].Value;
            if (_dbContext.ArkItems.Where(item => item.Name == name).Count() != 0) continue;


            var url = String.Format("https:{0}", attrs["data-file"].Value);

            using (var httpClient = new HttpClient())
            {
                var filePath = Path.Combine(_itemDirectoryPath, $"{name}.png");
                var responseResult = httpClient.GetAsync(url);
                using var memStream = responseResult.Result.Content.ReadAsStreamAsync().Result;
                using var fileStream = File.Create(filePath);
                memStream.CopyTo(fileStream);
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
        }
    }
    public Task Execute(IJobExecutionContext context)
    {
        PtrsGet();
        _dbContext.SaveChanges();
        return Task.CompletedTask;
    }
}

using System.Net;
using System.Text.Json;
using Quartz;
using Semver;

namespace MaaServer.Download.Jobs;

public class FetchGithubReleaseJob : IJob
{
    private readonly ILogger<FetchGithubReleaseJob> _logger;
    private readonly IConfiguration _configuration;
    private readonly IResourceManagerService _resourceManagerService;

    public FetchGithubReleaseJob(
        ILogger<FetchGithubReleaseJob> logger,
        IConfiguration configuration,
        IResourceManagerService resourceManagerService)
    {
        _logger = logger;
        _configuration = configuration;
        _resourceManagerService = resourceManagerService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("从 Github 获取最新 Release 版本");

        // 获取配置信息
        var url = _configuration["MaaServer:GithubQuery:ApiEndpoint"];
        var proxy = _configuration["MaaServer:GithubQuery:Proxy"];

        if (url is null or "")
        {
            _logger.LogError("未配置 Github Release 请求 API Endpoint");
            return;
        }

        // 构建 HttpClient
        var client = proxy is null or ""
            ? new HttpClient()
            : new HttpClient(new HttpClientHandler { Proxy = new WebProxy(proxy), UseProxy = true });

        // 请求 API
        var response = await client.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("请求 Github Release API 失败");
            return;
        }

        // 解析 JSON
        SemVersion version;
        var downloadUrls = new Dictionary<PlatformArchCombination, string>();
        var bodyStream = await response.Content.ReadAsStreamAsync();
        var doc = (await JsonDocument.ParseAsync(bodyStream)).RootElement;
        try
        {
            // 获取版本 Tag
            var tag = doc.GetProperty("tag_name").GetString()?.Remove(0, 1);
            var tagParsed = SemVersion.TryParse(tag, out version);
            if (tagParsed is false)
            {
                throw new ArgumentException("无法解析版本号：" + tag);
            }
            _logger.LogDebug("已获取版本号：v{Version}", tag);
            var localVersion = _resourceManagerService.GetLocalVersion();
            if (localVersion == version)
            {
                _logger.LogInformation("当前本地版本为最新版本，无需更新");
                return;
            }

            // 获取资源下载链接
            var assets = doc.GetProperty("assets").EnumerateArray();
            var index = 1;
            foreach (var asset in assets)
            {
                var name = asset.GetProperty("name").GetString();
                var downloadUrl = asset.GetProperty("browser_download_url").GetString();
                if (name is null || downloadUrl is null)
                {
                    _logger.LogWarning("获取第 {Index} 个资源文件名或下载链接失败", index);
                    continue;
                }
                _logger.LogDebug("获取到第 {Index} 个资源文件：{Name}", index, name);
                // e.g. MeoAssistantArkNight-Windows-x64-2.6.5.zip
                var split = name.Split("-");
                var platformString = split[1];
                var archString = split[2];
                var platform = platformString.ParseToPlatform();
                var arch = archString.ParseToArchitecture();
                if (platform is Platform.UnSupported || arch is Architecture.UnSupported)
                {
                    _logger.LogWarning("获取第 {Index} 个资源，平台或架构不受支持：{p}-{a}",
                        index, platformString, archString);
                    continue;
                }
                _logger.LogDebug("获取到第 {Index} 个资源，平台：{p}，架构：{a}",
                    index, platform.ToString(), arch.ToString());
                var combination = new PlatformArchCombination(platform, arch);
                downloadUrls.Add(combination, downloadUrl);
                index++;
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "解析 Github Release API 响应失败, 出现错误");
            return;
        }

        // 更新
        await _resourceManagerService.DownloadUpdates(downloadUrls, version);
    }
}

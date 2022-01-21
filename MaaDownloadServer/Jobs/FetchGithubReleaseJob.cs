using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Quartz;
using Semver;

namespace MaaDownloadServer.Jobs;

public class FetchGithubReleaseJob : IJob
{
    private readonly ILogger<FetchGithubReleaseJob> _logger;
    private readonly IVersionService _versionService;
    private readonly IUpdateManagerService _updateManagerService;
    private readonly IConfiguration _configuration;

    public FetchGithubReleaseJob(
        ILogger<FetchGithubReleaseJob> logger,
        IVersionService versionService,
        IUpdateManagerService updateManagerService,
        IConfiguration configuration)
    {
        _logger = logger;
        _versionService = versionService;
        _updateManagerService = updateManagerService;
        _configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobId = Guid.NewGuid();
        _logger.LogInformation("从 Github 获取最新 Release 版本，JobId: {JobId}", jobId);

        // 获取配置信息
        var url = _configuration["MaaServer:GithubQuery:ApiEndpoint"];
        var proxy = _configuration["MaaServer:GithubQuery:Proxy"];
        var packageName = _configuration["MaaServer:GithubQuery:PackageName"];

        if (url is null or "")
        {
            _logger.LogError("未配置 Github Release 请求 API Endpoint");
            return;
        }

        // 构建 HttpClient
        var client = proxy is null or ""
            ? new HttpClient()
            : new HttpClient(new HttpClientHandler { Proxy = new WebProxy(proxy), UseProxy = true });
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36 Edg/97.0.1072.62");

        // 请求 API
        var response = await client.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("请求 Github Release API 失败，状态码：{StatusCode}，Body：{Body}",
                response.StatusCode, errBody is null or "" ? "NULL" : errBody);
            return;
        }

        // 解析 JSON
        var updateTime = DateTime.MinValue;
        var version = new SemVersion(0);
        var downloadInfos = new List<DownloadContentInfo>();
        var updateLog = string.Empty;
        var bodyStream = await response.Content.ReadAsStreamAsync();
        var doc = (await JsonDocument.ParseAsync(bodyStream)).RootElement;
        try
        {
            // 获取资源下载链接
            var assets = doc.GetProperty("assets").EnumerateArray();
            var index = 1;
            foreach (var asset in assets)
            {
                // 获取和解析发布时间
                var updateTimeString = doc.GetProperty("published_at").GetString();
                if (updateTimeString is null)
                {
                    throw new ArgumentException("无法解析发布时间");
                }
                updateTime = DateTime.Parse(updateTimeString);
                _logger.LogDebug("已解析资源发布时间：{UpdateTime}", updateTime);

                // 获取版本号
                var versionString = doc.GetProperty("tag_name").GetString()?[1..];
                version = SemVersion.Parse(versionString);

                // 检查版本是否已经存在
                var exist = await _versionService.IsVersionExist(version);
                if (exist)
                {
                    _logger.LogWarning("版本 {Version} 已存在，跳过", version);
                    return;
                }

                // 获取更新日志
                updateLog = doc.GetProperty("body").GetString();

                // 获取和解析文件名
                var name = asset.GetProperty("name").GetString();
                var downloadUrl = asset.GetProperty("browser_download_url").GetString();
                if (name is null || downloadUrl is null)
                {
                    _logger.LogWarning("获取第 {Index} 个资源文件名或下载链接失败", index);
                    continue;
                }

                _logger.LogDebug("获取到第 {Index} 个资源文件：{Name}", index, name);
                // e.g. {PackageName}-Windows-x64-2.6.5.zip
                // e.g. {PackageName}-Windows-x64-2.6.5-alpha1+build10.zip
                var match = Regex.Match(name, $"{packageName}-(.+)-(.+).zip");
                if (match.Success is false)
                {
                    _logger.LogWarning("解析第 {Index} 个资源文件名 {Name} 失败，文件名格式匹配失败", index, name);
                    continue;
                }
                var infoString = name.Replace($"{packageName}-", "").Replace(".zip", "");
                // e.g Windows-x64-2.6.5
                // e.g Windows-x64-2.6.5-alpha1+build10
                var split = infoString.Split("-");
                var platformString = split[0];
                var archString = split[1];
                var platform = platformString.ParseToPlatform();
                var arch = archString.ParseToArchitecture();
                if (platform is Platform.UnSupported || arch is Architecture.UnSupported)
                {
                    _logger.LogWarning("获取第 {Index} 个资源，平台或架构不受支持：{p}-{a}",
                        index, platformString, archString);
                    continue;
                }

                _logger.LogDebug("获取到第 {Index} 个资源，平台：{p}，架构：{a}，发布时间：{UpdateTime}",
                    index, platform.ToString(), arch.ToString(), updateTime);
                downloadInfos.Add(new DownloadContentInfo(Guid.NewGuid(), downloadUrl, platform, arch));
                index++;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解析 Github Release API 响应失败, 出现错误");
            return;
        }

        await _updateManagerService.Update(downloadInfos, jobId, version, updateTime, updateLog);
    }
}

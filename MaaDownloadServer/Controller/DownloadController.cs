using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Controller;

[ApiController]
[Route("download/{platform}/{arch}")]
public class DownloadController : ControllerBase
{
    private readonly ILogger<DownloadController> _logger;
    private readonly IDownloadService _downloadService;
    private readonly IVersionService _versionService;
    private readonly IConfiguration _configuration;

    public DownloadController(ILogger<DownloadController> logger, IDownloadService downloadService, IConfiguration configuration, IVersionService versionService)
    {
        _logger = logger;
        _downloadService = downloadService;
        _configuration = configuration;
        _versionService = versionService;
    }

    [HttpGet("{version}")]
    public async Task<ActionResult<GetDownloadUrlDto>> GetFullPackageDownloadUrl(string platform, string arch,
        string version, [FromQuery] string component)
    {
        var pf = platform.ParseToPlatform();
        var a = arch.ParseToArchitecture();
        if (pf is Platform.UnSupported || a is Architecture.UnSupported)
        {
            _logger.LogWarning("传入 Platform 值 {Platform} 或 Arch 值 {Arch} 解析为不受支持", platform, arch);
            return NotFound();
        }

        PublicContent pc;

        string realVersion;
        if (version is "latest")
        {
            var latestVersion = await GetLatestVersion(component, pf, a);

            if (latestVersion is null)
            {
                return NotFound();
            }

            pc = await _downloadService.GetFullPackage(component, pf, a, latestVersion.Version.ParseToSemVer());
            realVersion = latestVersion.Version;
        }
        else
        {
            var semVerParsed = version.TryParseToSemVer(out var semVer);
            if (semVerParsed is false)
            {
                _logger.LogWarning("传入 version 值 {Version} 解析失败", version);
                return NotFound();
            }

            pc = await _downloadService.GetFullPackage(component, pf, a, semVer);
            realVersion = version;
        }

        if (pc is null)
        {
            return NotFound();
        }
        var dUrl = $"{_configuration["MaaServer:Server:ApiFullUrl"]}/files/{pc.Id}.{pc.FileExtension}";
        var dto = new GetDownloadUrlDto(platform, arch, realVersion, dUrl, pc.Hash);
        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<GetDownloadUrlDto>> GetUpdatePackageDownloadUrl(
        string platform, string arch, [FromQuery] string from, [FromQuery] string to, [FromQuery] string component)
    {
        var pf = platform.ParseToPlatform();
        var a = arch.ParseToArchitecture();
        if (pf is Platform.UnSupported || a is Architecture.UnSupported)
        {
            _logger.LogWarning("传入 Platform 值 {Platform} 或 Arch 值 {Arch} 解析为不受支持", platform, arch);
            return NotFound();
        }

        string realTo;
        if (to == "latest")
        {
            var latestVersion = await GetLatestVersion(component, pf, a);
            if (latestVersion is null)
            {
                return null;
            }

            realTo = latestVersion.Version;
        }
        else
        {
            realTo = to;
        }

        var semVerParsed1 = from.TryParseToSemVer(out var fromSemVer);
        var semVerParsed2 = realTo.TryParseToSemVer(out var toSemVer);
        if (semVerParsed1 is false || semVerParsed2 is false)
        {
            _logger.LogWarning("传入 version 值 {From} 或 {To} 解析失败", from, to);
            return NotFound();
        }
        var pc = await _downloadService.GetUpdatePackage(component, pf, a, fromSemVer, toSemVer);
        if (pc is null)
        {
            return NotFound();
        }
        var dUrl = $"{_configuration["MaaServer:Server:ApiFullUrl"]}/files/{pc.Id}.{pc.FileExtension}";
        var dto = new GetDownloadUrlDto(platform, arch, $"{from} -> {realTo}", dUrl, pc.Hash);
        return Ok(dto);
    }

    private async Task<Package> GetLatestVersion(string component, Platform pf, Architecture a)
    {
        return await _versionService.GetLatestVersion(component, pf, a);
    }
}

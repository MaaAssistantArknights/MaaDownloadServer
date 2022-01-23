using Microsoft.AspNetCore.Mvc;
using Semver;

namespace MaaDownloadServer.Controller;

[ApiController]
[Route("download/{platform}/{arch}")]
public class DownloadController : ControllerBase
{
    private readonly ILogger<DownloadController> _logger;
    private readonly IDownloadService _downloadService;
    private readonly IConfiguration _configuration;

    public DownloadController(ILogger<DownloadController> logger, IDownloadService downloadService, IConfiguration configuration)
    {
        _logger = logger;
        _downloadService = downloadService;
        _configuration = configuration;
    }

    [HttpGet("{version}")]
    public async Task<ActionResult<GetDownloadUrlDto>> GetFullPackageDownloadUrl(string platform, string arch, string version)
    {
        var pf = platform.ParseToPlatform();
        var a = arch.ParseToArchitecture();
        if (pf is Platform.UnSupported || a is Architecture.UnSupported)
        {
            _logger.LogWarning("传入 Platform 值 {platform} 或 Arch 值 {arch} 解析为不受支持", platform, arch);
            return NotFound();
        }
        var semVerParsed = SemVersion.TryParse(version, out var semVer);
        if (semVerParsed is false)
        {
            _logger.LogWarning("传入 version 值 {version} 解析失败", version);
            return NotFound();
        }
        var pc = await _downloadService.GetFullPackage(pf, a, semVer);
        if (pc is null)
        {
            return NotFound();
        }
        var dUrl = $"{_configuration["MaaServer:Server:ApiFullUrl"]}/files/{pc.Id}.zip";
        var dto = new GetDownloadUrlDto(platform, arch, version, dUrl, pc.Hash);
        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<GetDownloadUrlDto>> GetUpdatePackageDownloadUrl(
        string platform, string arch, [FromQuery] string from, [FromQuery] string to)
    {
        var pf = platform.ParseToPlatform();
        var a = arch.ParseToArchitecture();
        if (pf is Platform.UnSupported || a is Architecture.UnSupported)
        {
            _logger.LogWarning("传入 Platform 值 {platform} 或 Arch 值 {arch} 解析为不受支持", platform, arch);
            return NotFound();
        }
        var semVerParsed1 = SemVersion.TryParse(from, out var fromSemVer);
        var semVerParsed2 = SemVersion.TryParse(to, out var toSemVer);
        if (semVerParsed1 is false || semVerParsed2 is false)
        {
            _logger.LogWarning("传入 version 值 {from} 或 {to} 解析失败", from, to);
            return NotFound();
        }
        var pc = await _downloadService.GetUpdatePackage(pf, a, fromSemVer, toSemVer);
        if (pc is null)
        {
            return NotFound();
        }
        var dUrl = $"{_configuration["MaaServer:Server:ApiFullUrl"]}/files/{pc.Id}.zip";
        var dto = new GetDownloadUrlDto(platform, arch, $"{from} -> {to}", dUrl, pc.Hash);
        return Ok(dto);
    }
}

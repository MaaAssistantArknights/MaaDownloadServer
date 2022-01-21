using MaaDownloadServer.Services;
using Microsoft.AspNetCore.Mvc;
using Semver;

namespace MaaDownloadServer.Controller;

[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    private readonly IVersionService _versionService;
    private readonly ILogger<VersionService> _logger;

    public VersionController(IVersionService versionService, ILogger<VersionService> logger)
    {
        _versionService = versionService;
        _logger = logger;
    }

    [HttpGet("getPlatform")]
    public async Task<ActionResult<GetSupportedPlatformDto>> GetSupportedPlatforms()
    {
        var platforms = await _versionService.GetSupportedPlatforms();
        if (platforms is null)
        {
            _logger.LogInformation("GetSupportedPlatforms() returned null");
            return NotFound();
        }
        var platformStr = platforms.Select(x => x.ToString()).ToList();
        return Ok(new GetSupportedPlatformDto(platformStr));
    }

    [HttpGet("{platform}/getArch")]
    public async Task<ActionResult<GetSupportedArchDto>> GetSupportedArchitectures(string platform)
    {
        var pf = platform.ParseToPlatform();
        if (pf is Platform.UnSupported)
        {
            _logger.LogWarning("传入 Platform 值 {platform} 解析为不受支持", platform);
            return NotFound();
        }
        var arch = await _versionService.GetSupportedArchitectures(pf);
        if (arch is null)
        {
            _logger.LogWarning("GetSupportedArchitectures() returned null");
            return NotFound();
        }
        var archStr = arch.Select(x => x.ToString()).ToList();
        return Ok(new GetSupportedArchDto(platform, archStr));
    }

    [HttpGet("{platform}/{arch}/getVersion")]
    public async Task<ActionResult<GetVersionsDto>> GetVersionList(string platform, string arch, [FromQuery] int page)
    {
        if (page < 1)
        {
            _logger.LogWarning("传入 page 值 {page} 不合法", page);
            return NotFound();
        }
        var pf = platform.ParseToPlatform();
        var a = arch.ParseToArchitecture();
        if (pf is Platform.UnSupported || a is Architecture.UnSupported)
        {
            _logger.LogWarning("传入 Platform 值 {platform} 或 Arch 值 {arch} 解析为不受支持", platform, arch);
            return NotFound();
        }
        var versions = await _versionService.GetVersions(pf, a, page);
        if (versions is null)
        {
            _logger.LogWarning("GetVersions() returned null");
            return NotFound();
        }
        var versionMetadataList = versions.Select(x => new VersionMetadata(x.Key, x.Value)).ToList();
        return Ok(new GetVersionsDto(platform, arch, versionMetadataList));
    }

    [HttpGet("{platform}/{arch}/{version}")]
    public async Task<ActionResult<GetVersionDto>> GetVersion(string platform, string arch, string version)
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
        var package = await _versionService.GetVersion(pf, a, semVer);
        if (package is not null)
        {
            return Ok(new GetVersionDto(platform, arch,
                new VersionDetail(package.Version, package.PublishTime, package.UpdateLog,
                    package.Resources.Select(x => new ResourceMetadata(x.FileName, x.Path, x.Hash)).ToList())));
        }

        _logger.LogWarning("GetVersion() returned null");
        return NotFound();
    }
}

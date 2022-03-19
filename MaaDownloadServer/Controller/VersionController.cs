using Microsoft.AspNetCore.Mvc;
using Semver;

namespace MaaDownloadServer.Controller;

[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    private readonly IVersionService _versionService;
    private readonly ILogger<VersionController> _logger;

    public VersionController(IVersionService versionService, ILogger<VersionController> logger)
    {
        _versionService = versionService;
        _logger = logger;
    }

    [HttpGet("{platform}/{arch}/{version}")]
    public async Task<ActionResult<GetVersionDto>> GetVersion(string platform, string arch, string version, [FromQuery] string component)
    {
        var pf = platform.ParseToPlatform();
        var a = arch.ParseToArchitecture();
        if (pf is Platform.UnSupported || a is Architecture.UnSupported)
        {
            _logger.LogWarning("传入 Platform 值 {Platform} 或 Arch 值 {Arch} 解析为不受支持", platform, arch);
            return NotFound();
        }

        _logger.LogInformation(version);

        Package package;
        if (version is "latest")
        {
            package = await _versionService.GetLatestVersion(component, pf, a);
        }
        else
        {
            var semVerParsed = SemVersion.TryParse(version, out var semVer);
            if (semVerParsed is false)
            {
                _logger.LogWarning("传入 version 值 {Version} 解析失败", version);
                return NotFound();
            }
            package = await _versionService.GetVersion(component, pf, a, semVer);
        }

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

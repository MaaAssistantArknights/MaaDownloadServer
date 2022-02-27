using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Controller;

[ApiController]
[Route("game-data/zone")]
public class ArkZoneController : ControllerBase
{
    private readonly IArkZoneService _arkZoneService;

    public ArkZoneController(IArkZoneService arkZoneService)
    {
        _arkZoneService = arkZoneService;
    }

    [HttpGet("{zone_id}")]
    // ReSharper disable once InconsistentNaming
    public async Task<ActionResult<GetStageDto>> GetArkZone(string zone_id)
    {
        var dto = await _arkZoneService.GetZone(zone_id);

        if (dto is null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<QueryStagesDto>> QueryArkZones(
        // ReSharper disable InconsistentNaming
        [FromQuery] string zone_id,
        [FromQuery] string zone_name,
        [FromQuery] string zone_type,
        [FromQuery] string available_cn,
        [FromQuery] string available_kr,
        [FromQuery] string available_us,
        [FromQuery] string available_jp,
        [FromQuery] string limit,
        [FromQuery] string page)
    // ReSharper restore InconsistentNaming
    {
        var allQueries = new Dictionary<string, string>
        {
            { "page", page },
            { "limit", limit },
            { "zone_id", zone_id },
            { "zone_name", zone_name },
            { "zone_type", zone_type },
            { "available_cn", available_cn },
            { "available_kr", available_kr },
            { "available_us", available_us },
            { "available_jp", available_jp }
        };

        var queries = allQueries.Where(x => x.Value is not "" && x.Value is not null).ToImmutableDictionary();

        var dto = await _arkZoneService.QueryZones(queries);

        return Ok(dto);
    }
}

using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Controller;

[ApiController]
[Route("game-data/stage")]
public class ArkStageController : ControllerBase
{
    private readonly IArkStageService _arkStageService;

    public ArkStageController(IArkStageService arkStageService)
    {
        _arkStageService = arkStageService;
    }

    [HttpGet("{stage_code}")]
    // ReSharper disable once InconsistentNaming
    public async Task<ActionResult<GetStageDto>> GetArkStage(string stage_code)
    {
        var dto = await _arkStageService.GetStage(stage_code);

        if (dto is null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<QueryStagesDto>> QueryArkStages(
        // ReSharper disable InconsistentNaming
        [FromQuery] string stage_id,
        [FromQuery] string stage_type,
        [FromQuery] string stage_code,
        [FromQuery] string ap_cost_lower_limit,
        [FromQuery] string ap_cost_up_limit,
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
            { "stage_id", stage_id },
            { "stage_type", stage_type },
            { "stage_code", stage_code },
            { "ap_cost_lower_limit", ap_cost_lower_limit },
            { "ap_cost_up_limit", ap_cost_up_limit },
            { "zone_id", zone_id },
            { "zone_name", zone_name },
            { "zone_type", zone_type },
            { "available_cn", available_cn },
            { "available_kr", available_kr },
            { "available_us", available_us },
            { "available_jp", available_jp }
        };

        var queries = allQueries.Where(x => x.Value is not "" && x.Value is not null).ToImmutableDictionary();

        var dto = await _arkStageService.QueryStages(queries);

        return Ok(dto);
    }
}

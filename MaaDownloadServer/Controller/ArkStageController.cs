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

    [HttpGet("{stage_code_or_id}")]
    // ReSharper disable once InconsistentNaming
    public async Task<ActionResult<GetStageDto>> GetArkStage(string stage_code_or_id)
    {
        var dto = await _arkStageService.GetStage(stage_code_or_id);

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

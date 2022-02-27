using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Controller;

[ApiController]
[Route("game-data/item")]
public class ArkItemController : ControllerBase
{
    private readonly IArkItemService _arkItemService;

    public ArkItemController(IArkItemService arkItemService)
    {
        _arkItemService = arkItemService;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<GetItemDto>> GetArkItem(string name)
    {
        var dto = await _arkItemService.GetItem(name);

        if (dto is null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<QueryItemsDto>> QueryArkItems(
        [FromQuery] string name,
        [FromQuery] string limit,
        [FromQuery] string page)
    {
        var realLimit = 50;
        var realPage = 1;

        if (int.TryParse(limit, out var parsedLimit))
        {
            if (parsedLimit >= 0)
            {
                realLimit = parsedLimit;
            }
        }

        if (int.TryParse(page, out var parsedPage))
        {
            if (parsedPage >= 0)
            {
                realPage = parsedPage;
            }
        }

        var dto = await _arkItemService.QueryItems(name, realLimit, realPage);
        return Ok(dto);
    }
}

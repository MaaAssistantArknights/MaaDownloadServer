using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Controller;

[ApiController]
[Route("component")]
public class ComponentController : ControllerBase
{
    private readonly IComponentService _componentService;

    public ComponentController(IComponentService componentService)
    {
        _componentService = componentService;
    }

    [HttpGet("getAll")]
    public async Task<ActionResult<List<ComponentDto>>> GetComponents()
    {
        var dtos = await _componentService.GetAllComponents();
        return Ok(dtos);
    }

    [HttpGet("getInfo")]
    public async Task<ActionResult<GetComponentDetailDto>> GetComponentDetail([FromQuery] string component,
        [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (limit < 1)
        {
            limit = 10;
        }

        var dto = await _componentService.GetComponentDetail(component, limit, page);
        if (dto is null)
        {
            return NotFound();
        }
        return Ok(dto);
    }
}

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
        return dtos;
    }
}

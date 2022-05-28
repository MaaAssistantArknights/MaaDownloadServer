// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.Module;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace MaaDownloadServer.Api.AspNetCore.Controller;

[ApiController]
[Route("modules")]
public class ModulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ModulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<OkObjectResult> GetModuleList([FromQuery] int? page)
    {
        var response = await _mediator.Send(new GetModuleListCommand(HttpContext, page ?? 1));
        return response;
    }

    [HttpGet("{module}")]
    public async Task<OkObjectResult> GetModuleInfo(string module)
    {
        var response = await _mediator.Send(new GetModuleInfoCommand(HttpContext, module));
        return response;
    }

    [HttpGet("{module}/versions")]
    public async Task<OkObjectResult> GetModuleVersionList(string module, [FromQuery] int? page)
    {
        var response = await _mediator.Send(new GetModuleVersionListCommand(HttpContext, module, page ?? 1));
        return response;
    }
}

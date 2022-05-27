// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.Module;
using Microsoft.AspNetCore.Mvc;
using MaaDownloadServer.Core.Domain.Dto.Module;
using MaaDownloadServer.Shared.Utils.Extensions;
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
    public async Task<ActionResult<GetModuleListDto>> GetModuleList([FromQuery] int? page)
    {
        var response = await _mediator.Send(new GetModuleListCommand(page ?? 1));
        return response.GetOkOrNotFound();
    }

    [HttpGet("{module}")]
    public async Task<ActionResult<GetModuleInfoDto>> GetModuleInfo(string module)
    {
        var response = await _mediator.Send(new GetModuleInfoCommand(module));
        return response.GetOkOrNotFound();
    }

    [HttpGet("{module}/versions")]
    public async Task<ActionResult<GetModuleVersionListDto>> GetModuleVersionList(string module, [FromQuery] int? page)
    {
        var response = await _mediator.Send(new GetModuleVersionListCommand(module, page ?? 1));
        return response.GetOkOrNotFound();
    }
}

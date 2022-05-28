// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.Resource;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Api.AspNetCore.Controller;

[ApiController]
[Route("assets")]
public class ResourcesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResourcesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{module}/{version}")]
    public async Task<OkObjectResult> GetModuleVersionInfo(string module, string version)
    {
        var response = await _mediator.Send(new GetModuleVersionInfoCommand(HttpContext, module, version));
        return response;
    }
}

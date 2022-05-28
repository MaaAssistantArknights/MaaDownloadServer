// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.External;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Api.AspNetCore.Controller;

[ApiController]
[Route("externals")]
public class ExternalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExternalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<OkObjectResult> GetThirdPartyResource([FromQuery] int? page)
    {
        var response = await _mediator.Send(new GetThirdPartyResourceListCommand(HttpContext, page ?? 1));
        return response;
    }
}

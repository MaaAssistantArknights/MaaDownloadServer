// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.Other;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Api.AspNetCore.Controller;

[ApiController]
[Route("")]
public class OtherController : ControllerBase
{
    private readonly IMediator _mediator;

    public OtherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("announce/{publisher}")]
    public async Task<OkObjectResult> GetAnnounce(string publisher)
    {
        var response = await _mediator.Send(new GetAnnounceCommand(HttpContext.TraceIdentifier, publisher));
        return response;
    }
}

// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.Other;
using MaaDownloadServer.Core.Domain.Dto.Other;
using MaaDownloadServer.Shared.Utils.Extensions;
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

    [HttpGet("announce/{id}")]
    public async Task<ActionResult<GetModuleSyncInfoDto>> GetModuleSyncInfo(string id)
    {
        var response = await _mediator.Send(new GetModuleSyncInfoCommand(id));
        return response.GetOkOrNotFound();
    }
}

// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core.Requests.External;
using MaaDownloadServer.Core.Domain.Dto.External;
using MaaDownloadServer.Shared.Utils.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Api.AspNetCore.Controller;

[ApiController]
[Route("externals")]
public class ExternalsController : ControllerBase
{
    private readonly IMediator _mediator;

    private ExternalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetThirdPartyResourceListDto>> GetThirdPartyResource([FromQuery] int? page)
    {
        var response = await _mediator.Send(new GetThirdPartyResourceListCommand(page ?? 1));
        return response.GetOkOrNotFound();
    }
}

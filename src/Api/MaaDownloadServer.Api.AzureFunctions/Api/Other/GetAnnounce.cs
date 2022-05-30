// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Threading.Tasks;
using MaaDownloadServer.App.Core.Requests.Other;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MaaDownloadServer.Api.AzureFunctions.Api;

public class GetAnnounce
{
    private readonly IMediator _mediator;

    public GetAnnounce(IMediator mediator)
    {
        _mediator = mediator;
    }

    [FunctionName("GetAnnounce")]
    public async Task<OkObjectResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "announce/{publisher}")]
        HttpRequest req, ILogger log, ExecutionContext exc, string publisher)
    {
        var traceId = $"{exc.FunctionName}-{exc.InvocationId}";
        var response = await _mediator.Send(new GetAnnounceCommand(traceId, publisher));
        return response;
    }
}

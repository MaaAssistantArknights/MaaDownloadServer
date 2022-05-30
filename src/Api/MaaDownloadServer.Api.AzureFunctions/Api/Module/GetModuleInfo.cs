// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Threading.Tasks;
using MaaDownloadServer.App.Core.Requests.Module;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MaaDownloadServer.Api.AzureFunctions.Api;

public class GetModuleInfo
{
    private readonly IMediator _mediator;

    public GetModuleInfo(IMediator mediator)
    {
        _mediator = mediator;
    }

    [FunctionName("GetModuleInfo")]
    public async Task<OkObjectResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "modules/{module}")]
        HttpRequest req, ILogger log, ExecutionContext exc, string module)
    {
        var traceId = $"{exc.FunctionName}-{exc.InvocationId}";
        var response = await _mediator.Send(new GetModuleInfoCommand(traceId, module));
        return response;
    }
}

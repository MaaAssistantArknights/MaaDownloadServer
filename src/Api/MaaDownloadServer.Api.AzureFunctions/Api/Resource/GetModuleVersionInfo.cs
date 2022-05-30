// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Threading.Tasks;
using MaaDownloadServer.App.Core.Requests.Resource;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MaaDownloadServer.Api.AzureFunctions.Api;

public class GetModuleVersionInfo
{
    private readonly IMediator _mediator;

    public GetModuleVersionInfo(IMediator mediator)
    {
        _mediator = mediator;
    }

    [FunctionName("GetModuleVersionInfo")]
    public async Task<OkObjectResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "assets/{module}/{version}")]
        HttpRequest req, ILogger log, ExecutionContext exc, string module, string version)
    {
        var traceId = $"{exc.FunctionName}-{exc.InvocationId}";
        var response = await _mediator.Send(new GetModuleVersionInfoCommand(traceId, module, version));
        return response;
    }
}

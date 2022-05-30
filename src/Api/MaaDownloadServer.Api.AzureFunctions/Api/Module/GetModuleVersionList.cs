// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Threading.Tasks;
using MaaDownloadServer.Api.AzureFunctions.Utils;
using MaaDownloadServer.App.Core.Requests.Module;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MaaDownloadServer.Api.AzureFunctions.Api;

public class GetModuleVersionList
{
    private readonly IMediator _mediator;

    public GetModuleVersionList(IMediator mediator)
    {
        _mediator = mediator;
    }

    [FunctionName("GetModuleVersionList")]
    public async Task<OkObjectResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "modules/{module}/version")]
        HttpRequest req, ILogger log, ExecutionContext exc, string module)
    {
        var traceId = $"{exc.FunctionName}-{exc.InvocationId}";
        var page = req.Query.GetValue("page", 1, int.MaxValue, 1);
        var response = await _mediator.Send(new GetModuleVersionListCommand(traceId, module, page));
        return response;
    }
}

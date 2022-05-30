// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core;
using MaaDownloadServer.Data;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(MaaDownloadServer.Api.AzureFunctions.Startup))]

namespace MaaDownloadServer.Api.AzureFunctions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddMaaDbContext();
        builder.Services.AddMediatR(typeof(AppCoreAssemblyMarker));
    }
}

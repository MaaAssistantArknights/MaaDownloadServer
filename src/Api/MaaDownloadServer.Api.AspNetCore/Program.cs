// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.App.Core;
using MaaDownloadServer.Data;
using MaaDownloadServer.Shared.Utils.Helper;
using MediatR;

var configuration = ConfigurationHelper.BuildConfiguration();

var builder = WebApplication.CreateBuilder();

builder.Configuration.AddConfiguration(configuration);

builder.Services.AddControllers();
builder.Services.AddMaaDbContext(configuration);
builder.Services.AddMediatR(typeof(AppCoreAssemblyMarker));

var app = builder.Build();

app.MapControllers();

app.Run();

using MaaDownloadServer.Jobs;
using MaaDownloadServer.Middleware;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Quartz;
using Serilog;

#region Build configuration and logger

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

Log.Logger.Information("启动中...");

#endregion

#region Data directories

if (Directory.Exists(configuration["PhysicalFileProvider:Path"]) is false)
{
    Directory.CreateDirectory(configuration["PhysicalFileProvider:Path"]);
}

var subDirectories = new[] { "download", "public", "resources", "database" };
foreach (var subDirectory in subDirectories)
{
    var dir = Path.Combine(configuration["PhysicalFileProvider:Path"], subDirectory);
    if (Directory.Exists(dir) is false)
    {
        Directory.CreateDirectory(dir);
    }
}

#endregion

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddQuartzFetchGithubReleaseJob(configuration);
builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

app.UseUpdateCheck();

#region File server middleware

var provider = new FileExtensionContentTypeProvider
{
    Mappings =
    {
        [".zip"] = "application/octet-stream"
    }
};

app.UseFileServer(new FileServerOptions
{
    StaticFileOptions =
    {
        ContentTypeProvider = provider,
        OnPrepareResponse = context =>
        {
            var fn = context.File.Name;
            context.Context.Response.Headers.Add("content-disposition", $"attachment; filename={fn}");
        }
    },
    FileProvider = new PhysicalFileProvider(Path.Combine(configuration["PhysicalFileProvider:Path"], "public")),
    RequestPath = "/files",
    EnableDirectoryBrowsing = false,
    EnableDefaultFiles = false,
    RedirectToAppendTrailingSlash = false,
});

#endregion

app.MapControllers();
app.Run();

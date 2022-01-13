using MaaDownloadServer.Jobs;
using MaaDownloadServer.Middleware;
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

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddQuartzFetchGithubReleaseJob(configuration);
builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

app.UseUpdateCheck();
app.MapControllers();
app.Run();

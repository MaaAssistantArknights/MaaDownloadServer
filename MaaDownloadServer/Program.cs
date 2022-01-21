using MaaDownloadServer.Jobs;
using MaaDownloadServer.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
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

if (Directory.Exists(configuration["MaaServer:DataDirectories:RootPath"]) is false)
{
    Directory.CreateDirectory(configuration["MaaServer:DataDirectories:RootPath"]);
}

var subDirectories = new[]
{
    configuration["MaaServer:DataDirectories:SubDirectories:Downloads"],
    configuration["MaaServer:DataDirectories:SubDirectories:Public"],
    configuration["MaaServer:DataDirectories:SubDirectories:Resources"],
    configuration["MaaServer:DataDirectories:SubDirectories:Database"],
    configuration["MaaServer:DataDirectories:SubDirectories:Temp"]
};

foreach (var subDirectory in subDirectories)
{
    var dir = Path.Combine(configuration["MaaServer:DataDirectories:RootPath"], subDirectory);
    if (Directory.Exists(dir) is false)
    {
        Directory.CreateDirectory(dir);
    }
}

#endregion

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddMaaDownloadServerDbContext();
builder.Services.AddControllers();
builder.Services.AddLazyCache();
builder.Services.AddMaaServices();
builder.Services.AddQuartzFetchGithubReleaseJob(configuration);
builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

#region Database check

using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetService<MaaDownloadServerDbContext>();

if (File.Exists(
        Path.Combine(configuration["MaaServer:DataDirectories:RootPath"],
            configuration["MaaServer:DataDirectories:SubDirectories:Database"],
            "data.db")) is false)
{
    Log.Logger.Information("数据库文件不存在，准备创建新的数据库文件");
    dbContext!.Database.Migrate();
    Log.Logger.Information("数据库创建完成");
}

#endregion

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
    FileProvider = new PhysicalFileProvider(Path.Combine(configuration["MaaServer:DataDirectories:RootPath"], "public")),
    RequestPath = "/files",
    EnableDirectoryBrowsing = false,
    EnableDefaultFiles = false,
    RedirectToAppendTrailingSlash = false,
});

#endregion

app.MapControllers();
app.Run();

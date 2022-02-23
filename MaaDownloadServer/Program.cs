using System.Text;
using System.Text.Json;
using System.Web;
using AspNetCoreRateLimit;
using MaaDownloadServer.External;
using MaaDownloadServer.Jobs;
using MaaDownloadServer.Model.External;
using MaaDownloadServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Quartz;
using Serilog;
using Serilog.Extensions.Logging;

#region Build configuration and logger

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

Log.Logger.Information("启动中...");

#endregion

#region Data directories

if (configuration.GetValue<bool>("no-data-directory-check") is false)
{
    if (Directory.Exists(configuration["MaaServer:DataDirectories:RootPath"]) is false)
    {
        Directory.CreateDirectory(configuration["MaaServer:DataDirectories:RootPath"]);
    }

    var subDirectories = new[]
    {
        (configuration["MaaServer:DataDirectories:SubDirectories:Downloads"], true),
        (configuration["MaaServer:DataDirectories:SubDirectories:Public"], false),
        (configuration["MaaServer:DataDirectories:SubDirectories:Resources"], false),
        (configuration["MaaServer:DataDirectories:SubDirectories:Database"], false),
        (configuration["MaaServer:DataDirectories:SubDirectories:Temp"], true),
        (configuration["MaaServer:DataDirectories:SubDirectories:Scripts"], false),
        (configuration["MaaServer:DataDirectories:SubDirectories:VirtualEnvironments"], false),
        (configuration["MaaServer:DataDirectories:SubDirectories:GameData"], false)
    };

    foreach (var (subDirectory, initRequired) in subDirectories)
    {
        var dir = Path.Combine(configuration["MaaServer:DataDirectories:RootPath"], subDirectory);
        var di = new DirectoryInfo(dir);
        if (initRequired && di.Exists)
        {
            di.Delete(true);
        }
        if (di.Exists is false)
        {
            di.Create();
        }
    }
}
else
{
    Log.Logger.Warning("跳过了数据目录检查");
}

#endregion

#region Python environment and script configuration

var noCheckPythonEnvironment = configuration.GetValue<bool>("no-python-environment-check");
if (noCheckPythonEnvironment)
{
    Log.Logger.Warning("跳过了 Python 环境检查");
}

var basePythonInterpreter = configuration["MaaServer:ScriptEngine:Python"];
var logger = new SerilogLoggerFactory(Log.Logger).CreateLogger<Python>();

if (noCheckPythonEnvironment is false)
{
    // Check Python Interpreter Exist
    var pythonInterpreterExist = Python.EnvironmentCheck(logger, basePythonInterpreter);
    if (pythonInterpreterExist is false)
    {
        Log.Logger.Fatal("Python 解释器不存在，请检查配置");
        Environment.Exit(-1);
    }
}

// Init Python environment
var scriptRootDirectory = new DirectoryInfo(Path.Combine(
    configuration["MaaServer:DataDirectories:RootPath"],
    configuration["MaaServer:DataDirectories:SubDirectories:Scripts"]));
var venvRootDirectory = new DirectoryInfo(Path.Combine(
    configuration["MaaServer:DataDirectories:RootPath"],
    configuration["MaaServer:DataDirectories:SubDirectories:VirtualEnvironments"]));

var scriptDirectories = scriptRootDirectory.GetDirectories();

var componentConfigurations = new List<ComponentConfiguration>();
foreach (var scriptDirectory in scriptDirectories)
{
    var configurationFile = Path.Combine(scriptDirectory.FullName, "component.json");
    if (File.Exists(configurationFile) is false)
    {
        Environment.Exit(-1);
    }

    try
    {
        using var configFileStream = File.OpenRead(configurationFile);
        var configObj = JsonSerializer.Deserialize<ComponentConfiguration>(configFileStream);
        componentConfigurations.Add(configObj);
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "解析组件配置文件失败");
        Environment.Exit(-1);
    }

    if (noCheckPythonEnvironment is false)
    {
        var venvDirectory = Path.Combine(
            venvRootDirectory.FullName,
            scriptDirectory.Name);
        var requirements = scriptDirectory.GetFiles().FirstOrDefault(x => x.Name == "requirements.txt");
        var pyVenvCreateStatus = Python.CreateVirtualEnvironment(logger, basePythonInterpreter, venvDirectory, requirements?.FullName);
        if (pyVenvCreateStatus is false)
        {
            logger.LogCritical("Python 虚拟环境创建失败，venvDirectory: {venvDirectory}", venvDirectory);
            Environment.Exit(-1);
        }
    }
}

#endregion

var builder = WebApplication.CreateBuilder(args);
var url = $"http://{configuration["MaaServer:Server:Host"]}:{configuration["MaaServer:Server:Port"]}";
builder.WebHost.UseUrls(url);

#region Services
builder.Host.UseSerilog();
builder.Services.AddOptions();
builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddMaaDownloadServerDbContext();
builder.Services.AddControllers();
builder.Services.AddLazyCache();
builder.Services.AddMaaServices();
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddQuartzFetchGithubReleaseJob(configuration, componentConfigurations);
builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

#endregion

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

app.UseIpRateLimiting();

#region File server middleware

app.UseFileServer(new FileServerOptions
{
    StaticFileOptions =
    {
        DefaultContentType = "application/octet-stream",
        OnPrepareResponse = context =>
        {
            var fn = context.File.Name;

            if (fn is null)
            {
                return;
            }

            var encodedName = HttpUtility.UrlEncode(fn, Encoding.UTF8);
            context.Context.Response.Headers.Add("content-disposition", $"attachment; filename={encodedName}");
        }
    },
    FileProvider = new PhysicalFileProvider(Path.Combine(configuration["MaaServer:DataDirectories:RootPath"],
            configuration["MaaServer:DataDirectories:SubDirectories:Public"])),
    RequestPath = "/files/maa",
    EnableDirectoryBrowsing = false,
    EnableDefaultFiles = false,
    RedirectToAppendTrailingSlash = false,
});

app.UseFileServer(new FileServerOptions
{
    StaticFileOptions = { DefaultContentType = "image/png", },
    FileProvider = new PhysicalFileProvider(Path.Combine(configuration["MaaServer:DataDirectories:RootPath"],
        configuration["MaaServer:DataDirectories:SubDirectories:GameData"])),
    RequestPath = "/files/game-data",
    EnableDirectoryBrowsing = false,
    EnableDefaultFiles = false,
    RedirectToAppendTrailingSlash = false,
});

#endregion

app.MapControllers();

GC.Collect();
GC.WaitForPendingFinalizers();

app.Run();

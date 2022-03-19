using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Web;
using AspNetCoreRateLimit;
using MaaDownloadServer.External;
using MaaDownloadServer.Jobs;
using MaaDownloadServer.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Quartz;
using Serilog;
using Serilog.Extensions.Logging;

#region First run

var dataDirectoryEnvironmentVariable = Environment.GetEnvironmentVariable("MAADS_DATA_DIRECTORY");
var assemblyPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName;

var dataDirectory = string.IsNullOrEmpty(dataDirectoryEnvironmentVariable) ?
    new DirectoryInfo(Path.Combine(assemblyPath, "data")) :
    new DirectoryInfo(dataDirectoryEnvironmentVariable);

if (dataDirectory.Exists is false)
{
    dataDirectory.Create();
}

var configurationFileExist = dataDirectory.GetFiles("appsettings.json").Length == 1;

if (configurationFileExist is false)
{
    var appSettingString = await File.ReadAllTextAsync(Path.Combine(assemblyPath, "appsettings.json"));
    appSettingString = appSettingString.Replace("{{DATA DIRECTORY}}", dataDirectory.FullName);
    await File.WriteAllTextAsync(Path.Combine(dataDirectory.FullName, "appsettings.json"), appSettingString);
    Console.WriteLine($"配置文件不存在, 已复制新的 appsettings.json 至 {dataDirectory.FullName} 路径, 请修改配置文件");
    Environment.Exit(0);
}

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    if (File.Exists(Path.Combine(assemblyPath, "appsettings.Development.json")) &&
        File.Exists(Path.Combine(dataDirectory.FullName, "appsettings.Development.json")) is false)
    {
        File.Copy(Path.Combine(assemblyPath, "appsettings.Development.json"),
            Path.Combine(dataDirectory.FullName, "appsettings.Development.json"));
    }
}

#endregion

#region Build configuration and logger

var configurationBuilder = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(dataDirectory.FullName, "appsettings.json"));

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    configurationBuilder.AddJsonFile(Path.Combine(dataDirectory.FullName, "appsettings.Development.json"), true);
}

var azureAppConfigurationConnectionString = Environment.GetEnvironmentVariable("MAA_DS_AZURE_APP_CONFIGURATION");

if (azureAppConfigurationConnectionString is not null or "")
{
    configurationBuilder.AddAzureAppConfiguration(azureAppConfigurationConnectionString);
}

configurationBuilder
    .AddEnvironmentVariables("MAA:")
    .AddCommandLine(args);

var configuration = configurationBuilder.Build();

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
        (configuration["MaaServer:DataDirectories:SubDirectories:Static"], false),
        (configuration["MaaServer:DataDirectories:SubDirectories:VirtualEnvironments"], false),
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
        await using var configFileStream = File.OpenRead(configurationFile);
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
            logger.LogCritical("Python 虚拟环境创建失败，venvDirectory: {VenvDirectory}", venvDirectory);
            Environment.Exit(-1);
        }
    }
}

#endregion

var builder = WebApplication.CreateBuilder(args);

if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") is not "true")
{
    var url = $"http://{configuration["MaaServer:Server:Host"]}:{configuration["MaaServer:Server:Port"]}";
    builder.WebHost.UseUrls(url);
}
else
{
    Log.Logger.Information("在 Docker Container 中运行，忽略 MaaServer:Server:Host 和 MaaServer:Server:Port 配置项");
}

#region Web application builder

builder.Host.UseSerilog();
builder.Configuration.AddConfiguration(configuration);

builder.Services.AddOptions();
builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

builder.Services.AddMaaDownloadServerDbContext();
builder.Services.AddControllers();
builder.Services.AddMaaServices();
builder.Services.AddHttpClients(configuration);
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();

builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddQuartzJobs(configuration, componentConfigurations);
builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
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

var dbCaches = await dbContext!.DatabaseCaches.ToListAsync();
dbContext!.DatabaseCaches.RemoveRange(dbCaches);

#endregion

#region Add component name and description

var componentInfosDbCache = componentConfigurations
    .Select(x => new ComponentDto(x.Name, x.Description))
    .Select(x => JsonSerializer.Serialize(x))
    .Select(x => new DatabaseCache { Id = Guid.NewGuid(), QueryId = "Component", Value = x })
    .ToList();

await dbContext!.DatabaseCaches.AddRangeAsync(componentInfosDbCache);
await dbContext!.SaveChangesAsync();

Log.Logger.Information("已添加 {C} 个 Component", componentInfosDbCache.Count);

await dbContext!.DisposeAsync();

#endregion

app.UseIpRateLimiting();

app.UseCors();

app.UseMiddleware<DownloadCountMiddleware>();

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
    RequestPath = "/files",
    EnableDirectoryBrowsing = false,
    EnableDefaultFiles = false,
    RedirectToAppendTrailingSlash = false,
});

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
        configuration["MaaServer:DataDirectories:SubDirectories:Static"])),
    RequestPath = "/static",
    EnableDirectoryBrowsing = false,
    EnableDefaultFiles = false,
    RedirectToAppendTrailingSlash = false,
});

#endregion

#region Response Caching

app.UseResponseCaching();
app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
        new CacheControlHeaderValue { Public = true, MaxAge = TimeSpan.FromMinutes(5) };
    context.Response.Headers[HeaderNames.Vary] =
        new[] { "Accept-Encoding" };

    await next(context);
});

#endregion

app.MapControllers();

GC.Collect();
GC.WaitForPendingFinalizers();

app.Run();

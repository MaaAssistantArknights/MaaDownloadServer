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

var maaConfigurationProvider = MaaConfigurationProvider.GetProvider();
if (maaConfigurationProvider is null)
{
    Environment.Exit(ProgramExitCode.ConfigurationProviderIsNull);
}

#region Build configuration and logger

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(maaConfigurationProvider.GetConfiguration())
    .CreateLogger();

Log.Logger.Information("启动中...");
Log.Logger.Information("程序集版本：{AssemblyVersion}",
    maaConfigurationProvider.GetConfiguration().GetValue<string>("AssemblyVersion"));

#endregion

#region Data directories

var dataDirectoriesOption = maaConfigurationProvider.GetOption<DataDirectoriesOption>().Value;

if (MaaConfigurationProvider.IsNoDataDirectoryCheck() is false)
{
    if (Directory.Exists(dataDirectoriesOption.RootPath) is false)
    {
        Directory.CreateDirectory(dataDirectoriesOption.RootPath);
    }

    var directoryCheck = (string path) =>
    {
        if (Directory.Exists(path) is false)
        {
            Directory.CreateDirectory(path!);
        }
    };

    directoryCheck.Invoke(dataDirectoriesOption.Downloads);
    directoryCheck.Invoke(dataDirectoriesOption.Public);
    directoryCheck.Invoke(dataDirectoriesOption.Resources);
    directoryCheck.Invoke(dataDirectoriesOption.Database);
    directoryCheck.Invoke(dataDirectoriesOption.Temp);
    directoryCheck.Invoke(dataDirectoriesOption.Scripts);
    directoryCheck.Invoke(dataDirectoriesOption.Static);
    directoryCheck.Invoke(dataDirectoriesOption.VirtualEnvironments);
}
else
{
    Log.Logger.Warning("跳过了数据目录检查");
}

#endregion

#region Python environment and script configuration

if (MaaConfigurationProvider.IsNoPythonCheck())
{
    Log.Logger.Warning("跳过了 Python 环境检查");
}

var scriptEngineOption = maaConfigurationProvider.GetOption<ScriptEngineOption>().Value;

var logger = new SerilogLoggerFactory(Log.Logger).CreateLogger<Python>();

if (MaaConfigurationProvider.IsNoPythonCheck())
{
    // Check Python Interpreter Exist
    var pythonInterpreterExist = Python.EnvironmentCheck(logger, scriptEngineOption.Python);
    if (pythonInterpreterExist is false)
    {
        Log.Logger.Fatal("Python 解释器不存在，请检查配置");
        Environment.Exit(ProgramExitCode.NoPythonInterpreter);
    }
}

// Init Python environment
var scriptDirectories = new DirectoryInfo(dataDirectoriesOption.Scripts).GetDirectories();

var componentConfigurations = new List<ComponentConfiguration>();
foreach (var scriptDirectory in scriptDirectories)
{
    var configurationFile = Path.Combine(scriptDirectory.FullName, "component.json");
    if (File.Exists(configurationFile) is false)
    {
        Environment.Exit(ProgramExitCode.ScriptDoNotHaveConfigFile);
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
        Environment.Exit(ProgramExitCode.FailedToParseScriptConfigFile);
    }

    if (MaaConfigurationProvider.IsNoPythonCheck() is false)
    {
        var venvDirectory = Path.Combine(
            dataDirectoriesOption.VirtualEnvironments,
            scriptDirectory.Name);
        var requirements = scriptDirectory.GetFiles().FirstOrDefault(x => x.Name == "requirements.txt");
        var pyVenvCreateStatus = Python.CreateVirtualEnvironment(logger, scriptEngineOption.Python, venvDirectory, requirements?.FullName);
        if (pyVenvCreateStatus is false)
        {
            logger.LogCritical("Python 虚拟环境创建失败，venvDirectory: {VenvDirectory}", venvDirectory);
            Environment.Exit(ProgramExitCode.FailedToCreatePythonVenv);
        }
    }
}

#endregion

var builder = WebApplication.CreateBuilder(args);

if (MaaConfigurationProvider.IsInsideDocker())
{
    var serverOption = maaConfigurationProvider.GetOption<ServerOption>().Value;
    var url = $"http://{serverOption.Host}:{serverOption.Port}";
    builder.WebHost.UseUrls(url);
}
else
{
    Log.Logger.Information("在 Docker Container 中运行，忽略 MaaServer:Server:Host 和 MaaServer:Server:Port 配置项");
}

#region Web application builder

builder.Host.UseSerilog();
builder.Configuration.AddConfiguration(MaaConfigurationProvider.GetProvider().GetConfiguration());

builder.Services.AddMaaOptions(MaaConfigurationProvider.GetProvider());

builder.Services.AddMaaDownloadServerDbContext();
builder.Services.AddControllers();
builder.Services.AddMaaServices();
builder.Services.AddHttpClients(MaaConfigurationProvider.GetProvider());
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();

builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddQuartzJobs(maaConfigurationProvider.GetOption<PublicContentOption>(), componentConfigurations);
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

if (File.Exists(Path.Combine(dataDirectoriesOption.Database, "data.db")) is false)
{
    Log.Logger.Information("数据库文件不存在，准备创建新的数据库文件");
    dbContext!.Database.Migrate();
    Log.Logger.Information("数据库创建完成");
}

var dbCaches = await dbContext!.DatabaseCaches
    .Where(x => x.QueryId.StartsWith("persist_") == false)
    .ToListAsync();
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

app.UseSerilogRequestLogging(config =>
{
    config.IncludeQueryInRequestPath = true;
});

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
    FileProvider = new PhysicalFileProvider(dataDirectoriesOption.Public),
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
    FileProvider = new PhysicalFileProvider(dataDirectoriesOption.Static),
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

app.Run();

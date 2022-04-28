using System.Reflection;
using Microsoft.Extensions.Options;

namespace MaaDownloadServer.Providers;

public class MaaConfigurationProvider
{
    private static MaaConfigurationProvider s_provider;
    private readonly IConfiguration _configuration;

    private MaaConfigurationProvider(string assemblyPath, string dataDirectory)
    {
        var configFile = Path.Combine(dataDirectory, "appsettings.json");

        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile(configFile, false, true);

        if (IsDevelopment())
        {
            configurationBuilder.AddJsonFile(Path.Combine(dataDirectory, "appsettings.Development.json"), true, true);
        }

        var azureAppConfigurationConnectionString = Environment.GetEnvironmentVariable("MAADS_AZURE_APP_CONFIGURATION");
        if (string.IsNullOrEmpty(azureAppConfigurationConnectionString) is false)
        {
            configurationBuilder.AddAzureAppConfiguration(azureAppConfigurationConnectionString);
        }

        configurationBuilder.AddEnvironmentVariables("MAADS_");
        configurationBuilder.AddCommandLine(Environment.GetCommandLineArgs());

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var versionString = "0.0.0";
        if (version is not null)
        {
            versionString = $"{version.Major}.{version.Minor}.{version.Revision}";
        }
        configurationBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new("AssemblyPath", assemblyPath),
            new("ConfigurationFile", configFile),
            new("DataDirectory", dataDirectory),
            new("AssemblyVersion", versionString)
        });

        if (IsDevelopment())
        {
            configurationBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
            {
                new("DevConfigurationFile", Path.Combine(dataDirectory, "appsettings.Development.json")),
            });
        }

        _configuration = configurationBuilder.Build();
    }

    public static MaaConfigurationProvider GetProvider()
    {
        if (s_provider is not null)
        {
            return s_provider;
        }

        CreateProvider();
        return s_provider;
    }

    private static void CreateProvider()
    {
        var dataDirectoryEnvironmentVariable = Environment.GetEnvironmentVariable("MAADS_DATA_DIRECTORY");
        var assemblyPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName;

        var dataDirectory = string.IsNullOrEmpty(dataDirectoryEnvironmentVariable)
            ? new DirectoryInfo(Path.Combine(assemblyPath, "data"))
            : new DirectoryInfo(dataDirectoryEnvironmentVariable);

        if (dataDirectory.Exists is false)
        {
            dataDirectory.Create();
        }

        var configurationFileExist = dataDirectory.GetFiles("appsettings.json").Length == 1;

        if (configurationFileExist is false)
        {
            var appSettingString = File.ReadAllTextAsync(Path.Combine(assemblyPath, "appsettings.json")).Result;
            appSettingString = appSettingString.Replace("{{DATA DIRECTORY}}", dataDirectory.FullName);
            File.WriteAllTextAsync(Path.Combine(dataDirectory.FullName, "appsettings.json"), appSettingString).Wait();
            Console.WriteLine($"配置文件不存在, 已复制新的 appsettings.json 至 {dataDirectory.FullName} 路径, 请修改配置文件");
            Environment.Exit(0);
        }

        if (IsDevelopment())
        {
            if (File.Exists(Path.Combine(assemblyPath, "appsettings.Development.json")) &&
                File.Exists(Path.Combine(dataDirectory.FullName, "appsettings.Development.json")) is false)
            {
                File.Copy(Path.Combine(assemblyPath, "appsettings.Development.json"),
                    Path.Combine(dataDirectory.FullName, "appsettings.Development.json"));
            }
        }

        s_provider = new MaaConfigurationProvider(assemblyPath, dataDirectory.FullName);
    }

    public static bool IsInsideDocker()
    {
        return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") is "true";
    }

    public static bool IsDevelopment()
    {
        return GetFromEnvAndArgs("ASPNETCORE_ENVIRONMENT", "Development");
    }

    public static bool IsNoDataDirectoryCheck()
    {
        return GetFromEnvAndArgs("NO_DATA_DIRECTORY_CHECK");
    }

    public static bool IsNoPythonCheck()
    {
        return GetFromEnvAndArgs("NO_PYTHON_CHECK");
    }

    private static bool GetFromEnvAndArgs(string name, string value = null)
    {
        if (value is not null)
        {
            if (Environment.GetEnvironmentVariable(name) == value)
            {
                return true;
            }
        }
        else
        {
            if (Environment.GetEnvironmentVariable(name)?.ToLower() == "true")
            {
                return true;
            }
        }

        var inArgs = Environment.GetCommandLineArgs().FirstOrDefault(x => x.StartsWith(name));
        if (inArgs is null)
        {
            return false;
        }

        var status = inArgs.Replace($"{name}=", "");
        return status == value;
    }

    public IConfiguration GetConfiguration()
    {
        return _configuration;
    }

    public IOptions<T> GetOption<T>() where T : class, IMaaOption, new()
    {
        var obj = new T();
        var sectionName = AttributeUtil.ReadAttributeValue<T, ConfigurationSectionAttribute>();
        _configuration.Bind(sectionName, obj);
        var option = Options.Create(obj);
        return option;
    }

    public IConfigurationSection GetConfigurationSection(string key)
    {
        return _configuration.GetSection(key);
    }

    public IConfigurationSection GetOptionConfigurationSection<T>() where T : class, IMaaOption, new()
    {
        var sectionName = AttributeUtil.ReadAttributeValue<T, ConfigurationSectionAttribute>();
        return _configuration.GetSection(sectionName);
    }
}

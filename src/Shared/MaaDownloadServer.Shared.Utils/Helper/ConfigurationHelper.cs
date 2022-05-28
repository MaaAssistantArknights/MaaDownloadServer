// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Reflection;
using MaaDownloadServer.Shared.Utils.Extensions;
using Microsoft.Extensions.Configuration;

namespace MaaDownloadServer.Shared.Utils.Helper;

public static class ConfigurationHelper
{
    /// <summary>
    /// 构建 <see cref="IConfiguration"/>
    /// </summary>
    /// <remarks>不适用于 Azure Functions 等云服务</remarks>
    /// <returns><see cref="IConfiguration"/> 实例 (<see cref="ConfigurationRoot"/> 对象)</returns>
    public static IConfiguration BuildConfiguration()
    {
        var dataDirectoryEnv = Environment.GetEnvironmentVariable("MAA_DATA_DIRECTORY");

        var assemblyDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.NotNull();
        var dataDirectory = string.IsNullOrEmpty(dataDirectoryEnv)
            ? new DirectoryInfo(assemblyDirectory.FullName.CombinePath("data")).EnsureCreated()
            : new DirectoryInfo(dataDirectoryEnv).EnsureCreated();

        var currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var appsettingsFile = new FileInfo(dataDirectory.FullName.CombinePath("appsettings.json"));
        var appsettingsEnvFile = new FileInfo(dataDirectory.FullName.CombinePath($"appsettings.{currentEnvironment}.json"));
        var originalAppsettingsFile = new FileInfo(assemblyDirectory.FullName.CombinePath("appsettings.json")).AssertExist();
        var originalAppsettingsEnvFile = new FileInfo(assemblyDirectory.FullName.CombinePath($"appsettings.{currentEnvironment}.json"));

        if (appsettingsFile.Exists is false || appsettingsFile.IsSameMd5With(originalAppsettingsFile) is false)
        {
            appsettingsFile.EnsureDeleted();

            var text = File.ReadAllText(originalAppsettingsFile.FullName);
            text = text.Replace("{{ DATA DIRECTORY }}", dataDirectory.FullName);
            File.WriteAllText(appsettingsFile.FullName, text);

            appsettingsFile.AssertExist();
        }
        if (originalAppsettingsEnvFile.Exists)
        {
            originalAppsettingsEnvFile.CopyTo(appsettingsEnvFile.FullName, true);
        }
        else
        {
            appsettingsEnvFile.EnsureDeleted();
        }

        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile(appsettingsFile.FullName, optional: false, reloadOnChange: true);
        configurationBuilder.AddJsonFile(appsettingsEnvFile.FullName, optional: true, reloadOnChange: true);

        configurationBuilder.AddEnvironmentVariables("MAA_");

        configurationBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new("Application:AssemblyPath", assemblyDirectory.FullName),
            new("Application:DataDirectory", dataDirectory.FullName)
        });

        return configurationBuilder.Build();
    }
}

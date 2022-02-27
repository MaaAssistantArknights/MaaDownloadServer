using MaaDownloadServer.Services.Base;
using MaaDownloadServer.Services.Controller;

namespace MaaDownloadServer.Services;

public static class ServiceExtension
{
    public static void AddMaaServices(this IServiceCollection serviceCollection)
    {
        // Singleton
        serviceCollection.AddSingleton<ICacheService, CacheService>();

        // Scoped
        serviceCollection.AddScoped<IConfigurationService, ConfigurationService>();
        serviceCollection.AddScoped<IFileSystemService, FileSystemService>();

        // Controller
        serviceCollection.AddScoped<IVersionService, VersionService>();
        serviceCollection.AddScoped<IDownloadService, DownloadService>();
        serviceCollection.AddScoped<IArkItemService, ArkItemService>();
        serviceCollection.AddScoped<IArkStageService, ArkStageService>();
        serviceCollection.AddScoped<IArkZoneService, ArkZoneService>();
    }
}

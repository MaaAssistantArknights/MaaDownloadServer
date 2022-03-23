using MaaDownloadServer.Services.Base;
using MaaDownloadServer.Services.Controller;

namespace MaaDownloadServer.Extensions;

public static class ServiceExtension
{
    public static void AddMaaServices(this IServiceCollection serviceCollection)
    {
        // Scoped
        serviceCollection.AddScoped<IConfigurationService, ConfigurationService>();
        serviceCollection.AddScoped<IFileSystemService, FileSystemService>();
        serviceCollection.AddScoped<IAnnounceService, AnnounceService>();

        // Controller
        serviceCollection.AddScoped<IVersionService, VersionService>();
        serviceCollection.AddScoped<IDownloadService, DownloadService>();
        serviceCollection.AddScoped<IComponentService, ComponentService>();
    }
}

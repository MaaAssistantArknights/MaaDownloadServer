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
        serviceCollection.AddScoped<IVersionService, VersionService>();
        serviceCollection.AddScoped<IDownloadService, DownloadService>();

        // Transient
        serviceCollection.AddTransient<IUpdateManagerService, UpdateManagerService>();
    }
}

namespace MaaDownloadServer.Services;

public static class ServiceExtension
{
    public static void AddMaaServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IConfigurationService, ConfigurationService>();
        serviceCollection.AddScoped<IFileSystemService, FileSystemService>();
        serviceCollection.AddScoped<IVersionService, VersionService>();
    }
}

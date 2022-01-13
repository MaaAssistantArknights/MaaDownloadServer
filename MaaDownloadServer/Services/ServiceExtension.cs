namespace MaaDownloadServer.Services;

public static class ServiceExtension
{
    public static void AddMaaServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IResourceManagerService, ResourceManagerService>();
    }
}

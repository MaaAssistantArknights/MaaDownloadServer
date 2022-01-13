namespace MaaDownloadServer.Database;

public static class DbContextExtension
{
    public static void AddMaaDownloadServerDbContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<MaaDownloadServerDbContext>();
    }
}

namespace MaaDownloadServer.Middleware;

public static class ApplicationBuilderExtensions
{
    public static void UseUpdateCheck(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseMiddleware<UpdateCheckMiddleware>();
    }
}

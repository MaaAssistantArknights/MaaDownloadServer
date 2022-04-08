using AspNetCoreRateLimit;

namespace MaaDownloadServer.Extensions;

public static class OptionExtension
{
    public static void AddMaaOptions(this IServiceCollection service, MaaConfigurationProvider provider)
    {
        service.AddOptions();

        service.Configure<IpRateLimitOptions>(provider.GetConfigurationSection("IpRateLimiting"));
        service.Configure<IpRateLimitPolicies>(provider.GetConfigurationSection("IpRateLimitPolicies"));

        service.AddConfigureOption<DataDirectoriesOption>(provider);
        service.AddConfigureOption<NetworkOption>(provider);
        service.AddConfigureOption<PublicContentOption>(provider);
        service.AddConfigureOption<ScriptEngineOption>(provider);
        service.AddConfigureOption<ServerOption>(provider);
    }

    private static void AddConfigureOption<T>(this IServiceCollection service, MaaConfigurationProvider provider)
        where T : class, IMaaOption, new()
    {
        service.Configure<T>(provider.GetOptionConfigurationSection<T>());
    }
}

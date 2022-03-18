using System.Net;
using Polly;

namespace MaaDownloadServer.Extensions;

public static class HttpClientFactoryExtension
{
    public static void AddHttpClients(this IServiceCollection service, IConfiguration configuration)
    {
        var userAgent = configuration.GetValue<string>("MaaServer:Network:UserAgent");
        var proxyUrl = configuration.GetValue<string>("MaaServer:Network:Proxy");
        var proxy = string.IsNullOrEmpty(proxyUrl) ? null : new WebProxy(proxyUrl);

        service.AddHttpClient("NoProxy", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { Proxy = null, UseProxy = false, AllowAutoRedirect = true })
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(2000)));

        service.AddHttpClient("Proxy", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { Proxy = proxy, UseProxy = proxy is not null, AllowAutoRedirect = true })
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(2000)));
    }
}

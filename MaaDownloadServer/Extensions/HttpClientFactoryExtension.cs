using System.Net;
using Polly;

namespace MaaDownloadServer.Extensions;

public static class HttpClientFactoryExtension
{
    public static void AddHttpClients(this IServiceCollection service, MaaConfigurationProvider provider)
    {
        var option = provider.GetOption<NetworkOption>();
        var userAgent = option.Value.UserAgent;
        var proxyUrl = option.Value.Proxy;
        var version = provider.GetConfiguration().GetValue<string>("AssemblyVersion");
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

        service.AddHttpClient("ServerChan", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", $"MaaDownloadServer/{version}");
                client.BaseAddress = new Uri("https://sctapi.ftqq.com/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AllowAutoRedirect = true })
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(1000)));
    }
}

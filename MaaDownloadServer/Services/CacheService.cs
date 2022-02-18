using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace MaaDownloadServer.Services;

public class CacheService : ICacheService
{
    private readonly IAppCache _appCache;
    private const string LatestVersion = "{c}-{p}-{a}-versions-latest";
    private const string Version = "{c}-{p}-{a}-versions-{v}";
    private const string Versions = "{c}-{p}-{a}-versions-all-{page}";
    private const string AllSupportedPlatforms = "{c}-all-supported-platforms";
    private const string PlatformSupportedArchitectures = "{c}-{p}-supported-architectures";
    private const string DownloadFullPackage = "{c}-{p}-{a}-pc-f-{v}";
    private const string DownloadUpdatePackage = "{c}-{p}-{a}-pc-u-{from}-{to}";

    private readonly int _cacheExpirationInMinutes;

    private CancellationTokenSource _cancellationTokenSource;
    private MemoryCacheEntryOptions _cacheEntryOptions;
    private readonly Dictionary<string, (CancellationTokenSource, MemoryCacheEntryOptions)> _group = new();

    public CacheService(IAppCache appCache, IConfiguration configuration)
    {
        _cacheExpirationInMinutes = configuration.GetValue<int>("MaaServer:MemoryCache:ExpireTimeInMinutes");

        _appCache = appCache;
        _cancellationTokenSource = new CancellationTokenSource();
        _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheExpirationInMinutes))
            .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token));
    }

    public void Add<T>(string key, T value)
    {
        _appCache.Add(key, value, _cacheEntryOptions);
    }

    public void Add<T>(string key, T value, string group)
    {
        if (_group.ContainsKey(group) is false)
        {
            var ts = new CancellationTokenSource();
            var option = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheExpirationInMinutes))
                .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token))
                .AddExpirationToken(new CancellationChangeToken(ts.Token));
            _group.Add(group, (ts, option));
        }
        _appCache.Add(key, value, _group[group].Item2);
    }

    public void Add(string key, PublicContent pc, PublicContentTagType type)
    {
        if (_group.ContainsKey(type.ToString()) is false)
        {
            var ts = new CancellationTokenSource();
            _group.Add(type.ToString(), (ts, null));
        }
        var option = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(pc.Duration)
            .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token))
            .AddExpirationToken(new CancellationChangeToken(_group[type.ToString()].Item1.Token));
        _appCache.Add(key, pc, option);
    }

    public void RemoveAll()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheExpirationInMinutes))
            .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token));
        _group.Clear();
    }

    public void RemoveAll(string group)
    {
        if (_group.ContainsKey(group) is false)
        {
            return;
        }
        _group[group].Item1.Cancel();
        _group.Remove(group);
    }

    public void RemoveAll(PublicContentTagType type)
    {
        if (_group.ContainsKey(type.ToString()) is false)
        {
            return;
        }
        _group[type.ToString()].Item1.Cancel();
        _group.Remove(type.ToString());
    }

    public void Remove(string key)
    {
        _appCache.Remove(key);
    }

    public bool Contains(string key)
    {
        return _appCache.TryGetValue<object>(key, out _);
    }

    public T Get<T>(string key)
    {
        return _appCache.Get<T>(key);
    }

    public string GetLatestVersionKey(string componentName, Platform p, Architecture a)
    {
        return LatestVersion.ReplaceComponentName(componentName).ReplacePlatform(p).ReplaceArchitecture(a);
    }

    public string GetVersionCacheKey(string componentName, Platform p, Architecture a, string version)
    {
        return Version.ReplaceComponentName(componentName).ReplacePlatform(p).ReplaceArchitecture(a).Replace("{v}", version);
    }

    public string GetVersionsCacheKey(string componentName, Platform p, Architecture a, int page)
    {
        return Versions.ReplaceComponentName(componentName).ReplacePlatform(p).ReplaceArchitecture(a).Replace("{page}", page.ToString());
    }

    public string GetAllSupportedPlatformsKey(string componentName)
    {
        return AllSupportedPlatforms.ReplaceComponentName(componentName);
    }

    public string GetPlatformSupportedArchitecturesKey(string componentName, Platform p)
    {
        return PlatformSupportedArchitectures.ReplaceComponentName(componentName).ReplacePlatform(p);
    }

    public string GetDownloadFullPackageKey(string componentName, Platform p, Architecture a, string version)
    {
        return DownloadFullPackage.ReplaceComponentName(componentName).ReplacePlatform(p).ReplaceArchitecture(a).Replace("{v}", version);
    }

    public string GetDownloadUpdatePackageKey(string componentName, Platform p, Architecture a, string from, string to)
    {
        return DownloadUpdatePackage.ReplaceComponentName(componentName).ReplacePlatform(p).ReplaceArchitecture(a)
            .Replace("{from}", from).Replace("{to}", to);
    }
}

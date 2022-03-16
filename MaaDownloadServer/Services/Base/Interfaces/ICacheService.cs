namespace MaaDownloadServer.Services.Base.Interfaces;

public interface ICacheService
{
    void Add<T>(string key, T value);
    void Add<T>(string key, T value, string group);
    void Add(string key, PublicContent pc, PublicContentTagType type);
    void RemoveAll();
    void RemoveAll(string group);
    void RemoveAll(PublicContentTagType type);
    void Remove(string key);
    bool Contains(string key);
    T Get<T>(string key);

    string GetLatestVersionKey(string componentName, Platform p, Architecture a);
    string GetVersionCacheKey(string componentName, Platform p, Architecture a, string version);
    string GetVersionsCacheKey(string componentName, Platform p, Architecture a, int page);
    string GetAllSupportedPlatformsKey(string componentName);
    string GetPlatformSupportedArchitecturesKey(string componentName, Platform p);
    string GetDownloadFullPackageKey(string componentName, Platform p, Architecture a, string version);
    string GetDownloadUpdatePackageKey(string componentName, Platform p, Architecture a, string from, string to);
    string GetAllComponentsKey();
}

namespace MaaDownloadServer.Services.IServices;

public interface ICacheService
{
    void Add<T>(string key, T value);
    void Add<T>(string key, T value, string group);
    void RemoveAll();
    void RemoveAll(string group);
    void Remove(string key);
    bool Contains(string key);
    T Get<T>(string key);

    string GetLatestVersionKey(Platform p, Architecture a);
    string GetVersionCacheKey(Platform p, Architecture a, string version);
    string GetVersionsCacheKey(Platform p, Architecture a, int page);
    string GetAllSupportedPlatformsKey();
    string GetPlatformSupportedArchitecturesKey(Platform p);
}

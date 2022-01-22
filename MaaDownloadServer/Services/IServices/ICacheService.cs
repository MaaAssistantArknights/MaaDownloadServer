namespace MaaDownloadServer.Services.IServices;

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

    string GetLatestVersionKey(Platform p, Architecture a);
    string GetVersionCacheKey(Platform p, Architecture a, string version);
    string GetVersionsCacheKey(Platform p, Architecture a, int page);
    string GetAllSupportedPlatformsKey();
    string GetPlatformSupportedArchitecturesKey(Platform p);
    string GetDownloadFullPackageKey(Platform p, Architecture a, string version);
    string GetDownloadUpdatePackageKey(Platform p, Architecture a, string from, string to);
}

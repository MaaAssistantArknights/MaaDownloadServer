namespace MaaDownloadServer.Utils;

public static class CacheKeyUtil
{
    private const string LatestVersion = "{p}-{a}-versions-latest";
    private const string Version = "{p}-{a}-versions-{v}";
    private const string Versions = "{p}-{a}-versions-all-{page}";
    private const string AllSupportedPlatforms = "all-supported-platforms";
    private const string PlatformSupportedArchitectures = "{p}-supported-architectures";

    public static string GetLatestVersionKey(Platform p, Architecture a)
    {
        return LatestVersion.ReplacePlatform(p).ReplaceArchitecture(a);
    }

    public static string GetVersionCacheKey(Platform p, Architecture a, string version)
    {
        return Version.ReplacePlatform(p).ReplaceArchitecture(a).Replace("{v}", version);
    }

    public static string GetVersionsCacheKey(Platform p, Architecture a, int page)
    {
        return Versions.ReplacePlatform(p).ReplaceArchitecture(a).Replace("{page}", page.ToString());
    }

    public static string GetAllSupportedPlatformsKey()
    {
        return AllSupportedPlatforms;
    }

    public static string GetPlatformSupportedArchitecturesKey(Platform p)
    {
        return PlatformSupportedArchitectures.ReplacePlatform(p);
    }

    private static string ReplacePlatform(this string o, Platform p)
    {
        return o.Replace("{p}", p.ToString());
    }
    private static string ReplaceArchitecture(this string o, Architecture a)
    {
        return o.Replace("{a}", a.ToString());
    }
}

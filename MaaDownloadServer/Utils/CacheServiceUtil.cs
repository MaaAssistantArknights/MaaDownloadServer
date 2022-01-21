namespace MaaDownloadServer.Utils;

public static class CacheServiceUtil
{
    internal static string ReplacePlatform(this string o, Platform p)
    {
        return o.Replace("{p}", p.ToString());
    }
    internal static string ReplaceArchitecture(this string o, Architecture a)
    {
        return o.Replace("{a}", a.ToString());
    }
}

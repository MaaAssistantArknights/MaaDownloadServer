namespace MaaDownloadServer.Utils;

public static class PlatformArchParser
{
    public static Platform ParseToPlatform(this string platformString)
    {
        var str = platformString.ToLower();
        return str switch
        {
            "windows" => Platform.windows,
            "win" => Platform.windows,
            "linux" => Platform.linux,
            "macos" => Platform.macos,
            "osx" => Platform.macos,
            "no_platform" => Platform.NoPlatform,
            _ => Platform.UnSupported
        };
    }

    public static Architecture ParseToArchitecture(this string architectureString)
    {
        var str = architectureString.ToLower();
        return str switch
        {
            "x64" => Architecture.x64,
            "arm64" => Architecture.arm64,
            "no_arch" => Architecture.NoArch,
            _ => Architecture.UnSupported
        };
    }
}

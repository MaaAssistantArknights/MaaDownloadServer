using MaaDownloadServer.Enums;

namespace MaaDownloadServer.Utils;

public static class PlatformArchParser
{
    public static Platform ParseToPlatform(this string platformString)
    {
        var str = platformString.ToLower();
        return str switch
        {
            "windows" => Platform.Windows,
            "win" => Platform.Windows,
            "linux" => Platform.Linux,
            "macos" => Platform.MacOs,
            "osx" => Platform.MacOs,
            _ => Platform.UnSupported
        };
    }

    public static Architecture ParseToArchitecture(this string architectureString)
    {
        var str = architectureString.ToLower();
        return str switch
        {
            "x64" => Architecture.X64,
            "arm64" => Architecture.Arm64,
            _ => Architecture.UnSupported
        };
    }
}

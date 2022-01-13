using System.Security.Cryptography;

namespace MaaDownloadServer.Utils;

public static class HashUtil
{
    public static string ComputeFileMd5Hash(string filePath)
    {
        if (File.Exists(filePath) is false)
        {
            throw new FileNotFoundException("文件不存在", filePath);
        }

        using var fs = File.Open(filePath, FileMode.Open);
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(fs);
        var hashStr = BitConverter.ToString(hashBytes).Replace("-", "");
        return hashStr;
    }
}

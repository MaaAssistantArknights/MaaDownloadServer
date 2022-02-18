using System.Security.Cryptography;

namespace MaaDownloadServer.Utils;

public static class HashUtil
{
    public static string ComputeFileHash(ChecksumType type, string filePath)
        => type switch
        {
            ChecksumType.Md5 => ComputeFileMd5Hash(filePath),
            ChecksumType.Sha1 => ComputeFileSha1Hash(filePath),
            ChecksumType.Sha256 => ComputeFileSha256Hash(filePath),
            ChecksumType.Sha384 => ComputeFileSha384Hash(filePath),
            ChecksumType.Sha512 => ComputeFileSha512Hash(filePath),
            _ => null
        };

    public static string ComputeFileMd5Hash(string filePath)
    {
        return ComputeFileHash<MD5>(filePath);
    }

    public static string ComputeFileSha1Hash(string filePath)
    {
        return ComputeFileHash<SHA1>(filePath);
    }

    public static string ComputeFileSha256Hash(string filePath)
    {
        return ComputeFileHash<SHA256>(filePath);
    }

    public static string ComputeFileSha384Hash(string filePath)
    {
        return ComputeFileHash<SHA384>(filePath);
    }

    public static string ComputeFileSha512Hash(string filePath)
    {
        return ComputeFileHash<SHA512>(filePath);
    }

    private static string ComputeFileHash<T>(string filePath) where T : HashAlgorithm
    {
        if (File.Exists(filePath) is false)
        {
            throw new FileNotFoundException("文件不存在", filePath);
        }

        using var fs = File.Open(filePath, FileMode.Open);
        using var hash = HashAlgorithm.Create(typeof(T).Name);

        if (hash is null)
        {
            throw new SystemException($"不支持的 Hash 算法: {typeof(T).Name}");
        }

        var hashBytes = hash.ComputeHash(fs);
        var hashStr = BitConverter.ToString(hashBytes).Replace("-", "");
        return hashStr;
    }
}

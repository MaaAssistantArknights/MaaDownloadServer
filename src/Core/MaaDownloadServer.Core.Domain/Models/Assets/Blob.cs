// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Core.Domain.Models.Assets;

/// <summary>
/// 「区块」为单个文件的信息，不包含打包的「资源」
/// </summary>
public class Blob
{
    public Blob(string name, string sha1, string md5, string path)
    {
        Name = name;
        Sha1 = sha1;
        Md5 = md5;
        Path = path;
    }

    /// <summary>
    /// 文件名
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }
    /// <summary>
    /// SHA1 校验
    /// </summary>
    [JsonPropertyName("sha1")]
    public string Sha1 { get; }
    /// <summary>
    /// MD5 校验
    /// </summary>
    [JsonPropertyName("md5")]
    public string Md5 { get; }
    /// <summary>
    /// 相对路径
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; }
}

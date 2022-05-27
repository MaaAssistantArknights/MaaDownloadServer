// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Core.Domain.Models.Assets;

/// <summary>
/// 「资源」为可下载的文件，「资源」可以是单独的文件，也可以是文件集合
/// </summary>
public class Asset
{
    public Asset(string name, string sha1, string md5, string url, int downloadCount, bool isBundle, ICollection<Blob> files)
    {
        Name = name;
        Sha1 = sha1;
        Md5 = md5;
        Url = url;
        DownloadCount = downloadCount;
        IsBundle = isBundle;
        Files = files;
    }

    /// <summary>
    /// 资源名称
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
    /// 下载链接
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; }
    /// <summary>
    /// 下载次数
    /// </summary>
    [JsonPropertyName("download_count")]
    public int DownloadCount { get; }
    /// <summary>
    /// 是否为文件集
    /// </summary>
    [JsonPropertyName("is_bundle")]
    public bool IsBundle { get; }
    /// <summary>
    /// 资源所包含文件
    /// </summary>
    [JsonPropertyName("files")]
    public ICollection<Blob> Files { get; }
}

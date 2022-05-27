// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;
using MaaDownloadServer.Core.Domain.Models.Assets;

namespace MaaDownloadServer.Core.Domain.Models.External;

/// <summary>
/// 第三方组件信息
/// </summary>
public class ExternalModuleInfo
{
    public ExternalModuleInfo(string id, string name, string description, string url, string lastSync, Asset asset)
    {
        Id = id;
        Name = name;
        Description = description;
        Url = url;
        LastSync = lastSync;
        Asset = asset;
    }

    /// <summary>
    /// 组件 ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    /// 组件名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }

    /// <summary>
    /// 组件简介
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; }

    /// <summary>
    /// 组件地址
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; }

    /// <summary>
    /// 上次同步时间
    /// </summary>
    [JsonPropertyName("last_sync")]
    public string LastSync { get; }

    /// <summary>
    /// 组件资源
    /// </summary>
    [JsonPropertyName("asset")]
    public Asset Asset { get; }
}

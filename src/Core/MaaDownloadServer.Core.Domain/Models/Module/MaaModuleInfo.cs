// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Core.Domain.Models.Module;

/// <summary>
/// Maa 组件信息
/// </summary>
public class MaaModuleInfo
{
    public MaaModuleInfo(string id, string name, string description, string url, string lastSync, MaaModuleVersion? latestVersion)
    {
        Id = id;
        Name = name;
        Description = description;
        Url = url;
        LastSync = lastSync;
        LatestVersion = latestVersion;
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
    /// 最后一次同步时间
    /// </summary>
    [JsonPropertyName("last_sync")]
    public string LastSync { get; }

    /// <summary>
    /// 最新版本
    /// </summary>
    [JsonPropertyName("latest_version")]
    public MaaModuleVersion? LatestVersion { get; }
}

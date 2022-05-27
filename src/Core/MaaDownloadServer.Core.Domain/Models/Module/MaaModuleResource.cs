// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;
using MaaDownloadServer.Core.Domain.Models.Assets;

namespace MaaDownloadServer.Core.Domain.Models.Module;

/// <summary>
/// Maa 组件版本资源
/// </summary>
public class MaaModuleResource
{
    public MaaModuleResource(string id, string name, string version, string updateTime, string changeLog, Asset package, ICollection<MaaModuleUpdateResource> updates)
    {
        Id = id;
        Name = name;
        Version = version;
        UpdateTime = updateTime;
        ChangeLog = changeLog;
        Package = package;
        Updates = updates;
    }

    /// <summary>
    /// 组件 ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    /// 组件名
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }

    /// <summary>
    /// 版本号
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [JsonPropertyName("update_time")]
    public string UpdateTime { get; }

    /// <summary>
    /// 更新日志
    /// </summary>
    [JsonPropertyName("change_log")]
    public string ChangeLog { get; }

    /// <summary>
    /// 资源包
    /// </summary>
    [JsonPropertyName("package")]
    public Asset Package { get; }

    /// <summary>
    /// 更新资源
    /// </summary>
    [JsonPropertyName("updates")]
    public ICollection<MaaModuleUpdateResource> Updates { get; }
}

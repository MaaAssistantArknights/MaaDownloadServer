// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;
using MaaDownloadServer.Core.Domain.Models.Assets;

namespace MaaDownloadServer.Core.Domain.Models.Module;

/// <summary>
/// Maa 组件增量更新资源
/// </summary>
public class MaaModuleUpdateResource
{
    public MaaModuleUpdateResource(string id, string name, string versionFrom, string versionTo, MaaModuleUpdateChanges changes, Asset asset)
    {
        Id = id;
        Name = name;
        VersionFrom = versionFrom;
        VersionTo = versionTo;
        Changes = changes;
        Asset = asset;
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
    /// 起始版本
    /// </summary>
    [JsonPropertyName("version_from")]
    public string VersionFrom { get; }

    /// <summary>
    /// 终点版本
    /// </summary>
    [JsonPropertyName("version_to")]
    public string VersionTo { get; }

    /// <summary>
    /// 文件变化
    /// </summary>
    [JsonPropertyName("changes")]
    public MaaModuleUpdateChanges Changes { get; }

    /// <summary>
    /// 资源信息
    /// </summary>
    [JsonPropertyName("asset")]
    public Asset Asset { get; }
}

// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Core.Domain.Models.Module;

/// <summary>
/// Maa 组件版本列表
/// </summary>
public class MaaModuleVersionList
{
    public MaaModuleVersionList(string id, string name, int total, ICollection<MaaModuleVersion> versions)
    {
        Id = id;
        Name = name;
        Total = total;
        Versions = versions;
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
    /// 版本总数
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; }

    /// <summary>
    /// 版本列表
    /// </summary>
    [JsonPropertyName("versions")]
    public ICollection<MaaModuleVersion> Versions { get; }
}

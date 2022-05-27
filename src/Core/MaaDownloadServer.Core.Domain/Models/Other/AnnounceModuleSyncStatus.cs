// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Core.Domain.Models.Other;

/// <summary>
/// 组件同步信息
/// </summary>
public class AnnounceModuleSyncStatus
{
    public AnnounceModuleSyncStatus(string id, string name, string lastSync, string announceTime, string latestVersion, string message)
    {
        Id = id;
        Name = name;
        LastSync = lastSync;
        AnnounceTime = announceTime;
        LatestVersion = latestVersion;
        Message = message;
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
    /// 最后一次同步时间
    /// </summary>
    [JsonPropertyName("last_sync")]
    public string LastSync { get; set; }

    /// <summary>
    /// 公告时间
    /// </summary>
    [JsonPropertyName("announce_time")]
    public string AnnounceTime { get; set; }

    /// <summary>
    /// 最新同步版本
    /// </summary>
    [JsonPropertyName("latest_version")]
    public string LatestVersion { get; set; }

    /// <summary>
    /// 公告消息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}

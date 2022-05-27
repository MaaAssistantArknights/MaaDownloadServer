// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Core.Domain.Models.Module;

/// <summary>
/// Maa 组件版本信息
/// </summary>
public class MaaModuleVersion
{
    public MaaModuleVersion(string version, string updateTime, string changeLog)
    {
        Version = version;
        UpdateTime = updateTime;
        ChangeLog = changeLog;
    }

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
}

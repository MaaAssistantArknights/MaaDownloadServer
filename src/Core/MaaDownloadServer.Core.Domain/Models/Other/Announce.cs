// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Core.Domain.Models.Other;

/// <summary>
/// 公告
/// </summary>
public class Announce
{
    public Announce(string publishTime, string publisher, string message)
    {
        PublishTime = publishTime;
        Publisher = publisher;
        Message = message;
    }

    /// <summary>
    /// 发布者
    /// </summary>
    [JsonPropertyName("publish_time")]
    public string PublishTime { get; }

    /// <summary>
    /// 发布者
    /// </summary>
    [JsonPropertyName("publisher")]
    public string Publisher { get; }

    /// <summary>
    /// 公告消息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; }
}

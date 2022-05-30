// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Text.Json.Serialization;
using MaaDownloadServer.Data.Base.Entities.Base;

namespace MaaDownloadServer.Data.Base.Entities.Other;

/// <summary>
/// 公告
/// </summary>
public class Announce : EditableEntity
{
    public Announce(string publisher, string message)
    {
        Publisher = publisher;
        Message = message;
    }

    public void UpdateMessage(string message)
    {
        Message = message;
        base.UpdateAt = DateTimeOffset.Now;
    }

#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private Announce() { }
#pragma warning restore CS8618

    /// <summary>
    /// 发布者
    /// </summary>
    [JsonPropertyName("publisher")]
    public string Publisher { get; private set; }

    /// <summary>
    /// 公告消息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; private set; }
}

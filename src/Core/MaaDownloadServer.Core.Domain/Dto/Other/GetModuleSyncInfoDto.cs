// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Models.Other;

namespace MaaDownloadServer.Core.Domain.Dto.Other;

/// <summary>
/// 获取组件同步信息
/// </summary>
public class GetModuleSyncInfoDto : AnnounceModuleSyncStatus
{
    public GetModuleSyncInfoDto(string id, string name, string lastSync, string announceTime, string latestVersion, string message)
        : base(id, name, lastSync, announceTime, latestVersion, message) { }
}

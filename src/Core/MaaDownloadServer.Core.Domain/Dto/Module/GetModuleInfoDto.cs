// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Models.Module;

namespace MaaDownloadServer.Core.Domain.Dto.Module;

/// <summary>
/// 获取组件信息
/// </summary>
public class GetModuleInfoDto : MaaModuleInfo
{
    public GetModuleInfoDto(string id, string name, string description, string url, string lastSync, MaaModuleVersion latestVersion)
        : base(id, name, description, url, lastSync, latestVersion) { }
}

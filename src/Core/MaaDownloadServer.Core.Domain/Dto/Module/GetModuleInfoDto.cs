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
    public GetModuleInfoDto(MaaModuleInfo info)
        : base(info.Id, info.Name, info.Description, info.Url, info.LastSync, info.LatestVersion) { }
}

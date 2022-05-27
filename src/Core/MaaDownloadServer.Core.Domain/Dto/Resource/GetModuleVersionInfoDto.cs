// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Models.Assets;
using MaaDownloadServer.Core.Domain.Models.Module;

namespace MaaDownloadServer.Core.Domain.Dto.Resource;

/// <summary>
/// 获取组件版本资源
/// </summary>
public class GetModuleVersionInfoDto : MaaModuleResource
{
    public GetModuleVersionInfoDto(string id, string name, string version, string updateTime, string changeLog, Asset package, ICollection<MaaModuleUpdateResource> updates)
        : base(id, name, version, updateTime, changeLog, package, updates) { }
}

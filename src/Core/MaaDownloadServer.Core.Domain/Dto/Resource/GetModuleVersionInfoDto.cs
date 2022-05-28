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
    public GetModuleVersionInfoDto(MaaModuleResource r)
        : base(r.Id, r.Name, r.Version, r.UpdateTime, r.ChangeLog, r.Package, r.Updates) { }
}

// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Models.Module;

namespace MaaDownloadServer.Core.Domain.Dto.Module;

/// <summary>
/// 获取组件版本列表
/// </summary>
public class GetModuleVersionListDto : MaaModuleVersionList
{
    public GetModuleVersionListDto(MaaModuleVersionList v)
        : base(v.Id, v.Name, v.Total, v.Versions) { }
}

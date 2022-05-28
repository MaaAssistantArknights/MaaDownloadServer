// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Collections.ObjectModel;
using MaaDownloadServer.Core.Domain.Models.External;

namespace MaaDownloadServer.Core.Domain.Dto.External;

/// <summary>
/// 获取第三方资源列表
/// </summary>
public class GetThirdPartyResourceListDto : List<ExternalModuleInfo>
{
    public GetThirdPartyResourceListDto(IEnumerable<ExternalModuleInfo> infos) : base(infos) { }
}

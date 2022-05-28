// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Models.Other;

namespace MaaDownloadServer.Core.Domain.Dto.Other;

/// <summary>
/// 获取组件同步信息
/// </summary>
public class GetAnnounceDto : Announce
{
    public GetAnnounceDto(string publishTime, string publisher, string message)
        : base(publishTime, publisher, message) { }
}

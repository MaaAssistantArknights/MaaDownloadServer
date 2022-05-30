// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using Microsoft.AspNetCore.Http;

namespace MaaDownloadServer.App.Core.Requests.Other;

/// <summary>
/// 获取组件同步信息
/// </summary>
public class GetAnnounceCommand : MaaDomainRequest<GetAnnounceDto>
{
    public GetAnnounceCommand(HttpContext context, string publisher) : base(context)
    {
        Publisher = publisher;
    }

    public string Publisher { get; }
}

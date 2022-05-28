// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Dto.Other;
using Microsoft.AspNetCore.Http;

namespace MaaDownloadServer.App.Core.Requests.Other;

/// <summary>
/// 获取组件同步信息
/// </summary>
public class GetModuleSyncInfoCommand : MaaDomainRequest<GetModuleSyncInfoDto>
{
    public GetModuleSyncInfoCommand(HttpContext context, string moduleId) : base(context)
    {
        ModuleId = moduleId;
    }

    public string ModuleId { get; }
}

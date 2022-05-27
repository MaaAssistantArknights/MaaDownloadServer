// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Dto.Other;
using MediatR;

namespace MaaDownloadServer.App.Core.Requests.Other;

/// <summary>
/// 获取组件同步信息
/// </summary>
public class GetModuleSyncInfoCommand : IRequest<GetModuleSyncInfoDto>
{
    public GetModuleSyncInfoCommand(string moduleId)
    {
        ModuleId = moduleId;
    }

    public string ModuleId { get; }
}

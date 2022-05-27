// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Dto.Module;
using MediatR;

namespace MaaDownloadServer.App.Core.Requests.Module;

/// <summary>
/// 获取组件信息 Mediator 指令
/// </summary>
public class GetModuleInfoCommand : IRequest<GetModuleInfoDto?>
{
    public GetModuleInfoCommand(string moduleId)
    {
        ModuleId = moduleId;
    }

    public string ModuleId { get; }
}

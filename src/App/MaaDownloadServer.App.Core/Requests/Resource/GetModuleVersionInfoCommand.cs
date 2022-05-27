// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Dto.Resource;
using MediatR;

namespace MaaDownloadServer.App.Core.Requests.Resource;

/// <summary>
/// 获取组件版本资源 Mediator 指令
/// </summary>
public class GetModuleVersionInfoCommand : IRequest<GetModuleVersionInfoDto>
{
    public GetModuleVersionInfoCommand(string moduleId, string version)
    {
        ModuleId = moduleId;
        Version = version;
    }

    public string ModuleId { get; }
    public string Version { get; }
}

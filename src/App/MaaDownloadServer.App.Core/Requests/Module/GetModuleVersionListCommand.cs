// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Dto.Module;
using MediatR;

namespace MaaDownloadServer.App.Core.Requests.Module;

/// <summary>
/// 获取组件版本列表 Mediator 指令
/// </summary>
public class GetModuleVersionListCommand : IRequest<GetModuleVersionListDto>
{
    public GetModuleVersionListCommand(string moduleId, int page)
    {
        ModuleId = moduleId;
        Page = page;
    }

    public string ModuleId { get; }

    public int Page { get; }
}

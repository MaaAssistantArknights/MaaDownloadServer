// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using Microsoft.AspNetCore.Http;

namespace MaaDownloadServer.App.Core.Requests.Module;

/// <summary>
/// 获取组件信息 Mediator 指令
/// </summary>
public class GetModuleInfoCommand : MaaDomainRequest<GetModuleInfoDto>
{
    public GetModuleInfoCommand(HttpContext context, string moduleId) : base(context)
    {
        ModuleId = moduleId;
    }

    public string ModuleId { get; }
}

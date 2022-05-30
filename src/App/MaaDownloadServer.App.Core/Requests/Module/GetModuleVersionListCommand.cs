// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using Microsoft.AspNetCore.Http;

namespace MaaDownloadServer.App.Core.Requests.Module;

/// <summary>
/// 获取组件版本列表 Mediator 指令
/// </summary>
public class GetModuleVersionListCommand : MaaDomainRequest<GetModuleVersionListDto>
{
    public GetModuleVersionListCommand(string traceId, string moduleId, int page) : base(traceId)
    {
        ModuleId = moduleId;
        Page = page;
    }

    public string ModuleId { get; }

    public int Page { get; }
}

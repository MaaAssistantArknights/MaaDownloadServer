// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using Microsoft.AspNetCore.Http;

namespace MaaDownloadServer.App.Core.Requests.Module;

/// <summary>
/// 获取组件列表 Mediator 指令
/// </summary>
public class GetModuleListCommand : MaaDomainRequest<GetModuleListDto>
{
    public GetModuleListCommand(string traceId, int page) : base(traceId)
    {
        Page = page;
    }

    public int Page { get; }
}

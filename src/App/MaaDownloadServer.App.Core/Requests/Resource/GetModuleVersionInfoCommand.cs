// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using Microsoft.AspNetCore.Http;

namespace MaaDownloadServer.App.Core.Requests.Resource;

/// <summary>
/// 获取组件版本资源 Mediator 指令
/// </summary>
public class GetModuleVersionInfoCommand : MaaDomainRequest<GetModuleVersionInfoDto>
{
    public GetModuleVersionInfoCommand(string traceId, string moduleId, string version) : base(traceId)
    {
        ModuleId = moduleId;
        Version = version;
    }

    public string ModuleId { get; }
    public string Version { get; }
}

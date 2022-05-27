// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Dto.External;
using MediatR;

namespace MaaDownloadServer.App.Core.Requests.External;

/// <summary>
/// 获取第三方资源列表 Mediator 指令
/// </summary>
public class GetThirdPartyResourceListCommand : IRequest<GetThirdPartyResourceListDto>
{
    public GetThirdPartyResourceListCommand(int page)
    {
        Page = page;
    }

    public int Page { get;}
}

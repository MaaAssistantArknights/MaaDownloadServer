// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using Microsoft.AspNetCore.Http;

namespace MaaDownloadServer.App.Core.Requests;

public abstract class MaaDomainRequest<T> : IRequest<MaaActionResult<T>>
{
    public HttpContext HttpContext { get; }

    protected MaaDomainRequest(HttpContext httpContext)
    {
        HttpContext = httpContext;
    }
}

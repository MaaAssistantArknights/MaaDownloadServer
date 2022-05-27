// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Shared.Utils.Extensions;

public static class ActionResultNullCheckExtension
{
    public static ActionResult<TResponse> GetOkOrNotFound<TResponse>(this TResponse? obj) where TResponse : class
    {
        return obj is null ? new NotFoundResult() : new OkObjectResult(obj);
    }
}

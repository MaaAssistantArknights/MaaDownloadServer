// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Shared.Utils.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaaDownloadServer.Shared.Utils.Api;

public class MaaActionResult<T>
{
    private readonly MaaApiResponse _maaApiResponse;

    private MaaActionResult(MaaApiResponse maaApiResponse)
    {
        _maaApiResponse = maaApiResponse;
    }

    public static implicit operator OkObjectResult(MaaActionResult<T> result)
    {
        return new OkObjectResult(result._maaApiResponse);
    }

    public static implicit operator MaaActionResult<T>(MaaApiResponse response)
    {
        return new MaaActionResult<T>(response);
    }
}

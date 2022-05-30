// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using Microsoft.AspNetCore.Http;

namespace MaaDownloadServer.Api.AzureFunctions.Utils;

public static class QueryParameterUtil
{
    public static string GetValue(this IQueryCollection queryCollection, string paramName, string defaultValue)
    {
        var hasValue = queryCollection.TryGetValue(paramName, out var valueString);
        if (hasValue is false)
        {
            return defaultValue;
        }

        return valueString;
    }

    public static int GetValue(this IQueryCollection queryCollection, string paramName, int defaultValue, int? max = null, int? min = null)
    {
        var hasValue = queryCollection.TryGetValue(paramName, out var valueString);
        if (hasValue is false)
        {
            return defaultValue;
        }

        var isInt = int.TryParse(valueString, out var valueInt);
        if (isInt is false)
        {
            return defaultValue;
        }

        if (valueInt > max)
        {
            return max.Value;
        }

        if (valueInt < min)
        {
            return min.Value;
        }

        return valueInt;
    }
}

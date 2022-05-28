// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Globalization;

namespace MaaDownloadServer.Shared.Utils.Extensions;

public static class DateTimeOffsetExtension
{
    public static string? ToStringZhHans(this DateTimeOffset? dateTimeOffset)
    {
        if (dateTimeOffset is null) { return null;}
        var culture = new CultureInfo("zh-Hans");
        return dateTimeOffset.Value.ToString("o", culture);
    }

    public static string ToStringZhHans(this DateTimeOffset dateTimeOffset)
    {
        var culture = new CultureInfo("zh-Hans");
        return dateTimeOffset.ToString("o", culture);
    }
}

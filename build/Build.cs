// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using System.Diagnostics.CodeAnalysis;
using Nuke.Common;
using Nuke.Common.Execution;
using Serilog;

namespace MaaDownloadServer.Tools.Builder;

[CheckBuildProjectConfigurations]
public partial class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Default);

    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    Target Default => _ => _
        .Executes(() =>
        {
            Log.Warning("请指定一个 Target");
        });
}

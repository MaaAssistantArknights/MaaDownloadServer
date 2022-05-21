using Nuke.Common;
using Nuke.Common.Execution;
using Serilog;

[CheckBuildProjectConfigurations]
public partial class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Default);

    Target Default => _ => _
        .Executes(() =>
        {
            Log.Warning("请指定一个 Target");
        });
}

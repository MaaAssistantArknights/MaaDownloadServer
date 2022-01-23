using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Frosting;

namespace MaaDownloadServer.Build.Tasks;

[TaskName("Publish")]
[IsDependentOn(typeof(BuildTask))]
public sealed class PublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetPublish("../MaaDownloadServer/MaaDownloadServer.csproj", new DotNetPublishSettings
        {
            Configuration = context.MsBuildConfiguration,
            SelfContained = false,
            OutputDirectory = "../publish",
            Framework = context.Framework,
            Runtime = context.PublishRid is "any" ? null : context.PublishRid,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
        });
    }
}

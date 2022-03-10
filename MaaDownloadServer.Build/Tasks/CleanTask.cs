using Cake.Common.IO;
using Cake.Frosting;

namespace MaaDownloadServer.Build.Tasks;

[TaskName("Clean")]
[IsDependentOn(typeof(LoggingTask))]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectory($"../MaaDownloadServer/bin/{context.MsBuildConfiguration}");
        context.CleanDirectory("../publish");
    }
}

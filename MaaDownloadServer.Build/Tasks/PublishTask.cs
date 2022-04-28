using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace MaaDownloadServer.Build.Tasks;

[TaskName("Publish")]
[IsDependentOn(typeof(BuildTask))]
public sealed class PublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.Docker is "false")
        {
            context.DotNetPublish("../MaaDownloadServer/MaaDownloadServer.csproj", new DotNetPublishSettings
            {
                Configuration = context.MsBuildConfiguration,
                SelfContained = false,
                OutputDirectory = $"../publish/{context.Framework}-{context.PublishRid}-{context.MsBuildConfiguration}",
                Framework = context.Framework,
                Runtime = context.PublishRid is "portable" ? null : context.PublishRid,
                MSBuildSettings = context.BuildSettings
            });
        }
        else
        {
            var arches = context.DockerArches.Split(",");
            foreach (var arch in arches)
            {
                var clrArch = arch switch
                {
                    "amd64" => "x64",
                    "arm64" => "arm64",
                    "arm/v7" => "arm",
                    _ => "?"
                };
                if (clrArch is "?")
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Error, $"Unsupported arch: {arch}");
                    continue;
                }
                context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Publish app for Docker with arch {clrArch}");
                context.DotNetPublish("../MaaDownloadServer/MaaDownloadServer.csproj", new DotNetPublishSettings
                {
                    Configuration = context.MsBuildConfiguration,
                    SelfContained = false,
                    OutputDirectory = $"../publish/net6.0-docker-{arch}-{context.MsBuildConfiguration}",
                    Framework = "net6.0",
                    Runtime = $"linux-{clrArch}",
                    MSBuildSettings = context.BuildSettings
                });
            }
        }
    }
}

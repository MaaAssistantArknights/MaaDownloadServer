using Cake.Frosting;
using LogLevel = Cake.Core.Diagnostics.LogLevel;
using Verbosity = Cake.Core.Diagnostics.Verbosity;

namespace MaaDownloadServer.Build.Tasks;

[TaskName("Logging")]
public class LoggingTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.Docker is "false")
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Build MaaDownloadServer for bare-metal.");
            context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Configuration: {context.MsBuildConfiguration}");
            context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Platform: {context.PublishRid}");
            context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Framework: {context.Framework}");
        }
        else
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Build MaaDownloadServer for Docker.");
            context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Configuration: {context.MsBuildConfiguration}");
            context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Docker Arches: {context.DockerArches}");
            context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Framework: {context.Framework}");
        }
    }
}

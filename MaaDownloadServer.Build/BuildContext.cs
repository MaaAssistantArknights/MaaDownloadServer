using Cake.Common;
using Cake.Core;
using Cake.Frosting;
using LogLevel = Cake.Core.Diagnostics.LogLevel;
using Verbosity = Cake.Core.Diagnostics.Verbosity;

namespace MaaDownloadServer.Build;

public class BuildContext : FrostingContext
{
    public string MsBuildConfiguration { get; set; }
    public string PublishRid { get; set; }
    public string Framework { get; set; }
    public string Docker { get; set; }
    public string DockerArches { get; set; }

    public BuildContext(ICakeContext context) : base(context)
    {
        context.Log.Write(Verbosity.Normal, LogLevel.Information, "");
        MsBuildConfiguration = context.Argument("configuration", "Release");
        PublishRid = context.Argument("rid", "portable");
        Framework = context.Argument("framework", "net6.0");
        Docker = context.Argument("docker", "false");
        DockerArches = context.Argument("docker-arches", "amd64,arm64,arm/v7");
    }
}

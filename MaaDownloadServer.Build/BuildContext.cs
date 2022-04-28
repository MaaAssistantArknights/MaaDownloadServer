using Cake.Common;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Core;
using Cake.Frosting;
using LogLevel = Cake.Core.Diagnostics.LogLevel;
using Verbosity = Cake.Core.Diagnostics.Verbosity;

namespace MaaDownloadServer.Build;

public class BuildContext : FrostingContext
{
    private string Version { get; set; }

    public string MsBuildConfiguration { get; set; }
    public string PublishRid { get; set; }
    public string Framework { get; set; }
    public string Docker { get; set; }
    public string DockerArches { get; set; }
    public DotNetMSBuildSettings BuildSettings { get; set; }

    public BuildContext(ICakeContext context) : base(context)
    {
        context.Log.Write(Verbosity.Normal, LogLevel.Information, "");
        MsBuildConfiguration = context.Argument("configuration", "Release");
        Version = context.Argument("maads-version", "0.0.0");
        PublishRid = context.Argument("rid", "portable");
        Framework = context.Argument("framework", "net6.0");
        Docker = context.Argument("docker", "false");
        DockerArches = context.Argument("docker-arches", "amd64,arm64,arm/v7");

        var versionOk = SemVersion.TryParse(this.Version, out var version);
        if (versionOk is false)
        {
            throw new ArgumentException("Version string is not valid.");
        }

        BuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
            .SetVersion(version.AssemblyVersion.ToString())
            .SetFileVersion(version.AssemblyVersion.ToString())
            .SetInformationalVersion(version.VersionString)
            .SetAssemblyVersion(version.AssemblyVersion.ToString());
        if (version.IsPreRelease)
        {
            BuildSettings.SetVersionSuffix(version.PreRelease);
        }
    }
}

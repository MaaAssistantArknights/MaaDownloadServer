using Cake.Common;
using Cake.Core;
using Cake.Frosting;

namespace MaaDownloadServer.Build;

public class BuildContext : FrostingContext
{
    public string MsBuildConfiguration { get; set; }
    public string PublishRid { get; set; }
    public string Framework { get; set; }

    public BuildContext(ICakeContext context) : base(context)
    {
        MsBuildConfiguration = context.Argument("configuration", "Release");
        PublishRid = context.Argument("rid", "portable");
        Framework = context.Argument("framework", "net6.0");
    }
}

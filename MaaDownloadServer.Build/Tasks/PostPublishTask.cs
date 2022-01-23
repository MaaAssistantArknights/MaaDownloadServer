using System.IO.Compression;
using Cake.Frosting;

namespace MaaDownloadServer.Build.Tasks;

[TaskName("PostPublish")]
[IsDependentOn(typeof(PublishTask))]
public class PostPublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (File.Exists("../publish/appsettings.Development.json"))
        {
            File.Delete("../publish/appsettings.Development.json");
        }
        ZipFile.CreateFromDirectory("../publish",
            $"../MaaDownloadServer-{context.MsBuildConfiguration}-{context.Framework}-{context.PublishRid}.zip");
    }
}

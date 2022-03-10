using System.IO.Compression;
using Cake.Frosting;

namespace MaaDownloadServer.Build.Tasks;

[TaskName("PostPublish")]
[IsDependentOn(typeof(PublishTask))]
public class PostPublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.Docker is "false")
        {
            if (File.Exists(
                    $"../publish/{context.Framework}-{context.PublishRid}-{context.MsBuildConfiguration}/appsettings.Development.json"))
            {
                File.Delete(
                    $"../publish/{context.Framework}-{context.PublishRid}-{context.MsBuildConfiguration}/appsettings.Development.json");
            }

            ZipFile.CreateFromDirectory(
                $"../publish/{context.Framework}-{context.PublishRid}-{context.MsBuildConfiguration}",
                $"../publish/MaaDownloadServer-{context.MsBuildConfiguration}-{context.Framework}-{context.PublishRid}.zip");
        }
        else
        {
            var arches = context.DockerArches.Split(",");
            foreach (var arch in arches)
            {
                if (File.Exists(
                        $"../publish/net6.0-docker-{arch}-{context.MsBuildConfiguration}/appsettings.Development.json"))
                {
                    File.Delete(
                        $"../publish/net6.0-docker-{arch}-{context.MsBuildConfiguration}/appsettings.Development.json");
                }

                if (File.Exists(
                        $"../publish/net6.0-docker-{arch}-{context.MsBuildConfiguration}/appsettings.json"))
                {
                    File.Delete(
                        $"../publish/net6.0-docker-{arch}-{context.MsBuildConfiguration}/appsettings.json");
                }

                if (File.Exists(
                        $"../publish/net6.0-docker-{arch}-{context.MsBuildConfiguration}/appsettings.Docker.json"))
                {
                    File.Move($"../publish/net6.0-docker-{arch}-{context.MsBuildConfiguration}/appsettings.Docker.json",
                        $"../publish/net6.0-docker-{arch}-{context.MsBuildConfiguration}/appsettings.json");
                }
                else
                {
                    File.Copy($"../MaaDownloadServer/appsettings.Docker.json",
                        $"../publish/net6.0-docker-{arch}-{context.MsBuildConfiguration}/appsettings.json");
                }
            }
        }
    }
}

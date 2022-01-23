using Cake.Frosting;

namespace MaaDownloadServer.Build.Tasks;

[TaskName("Default")]
[IsDependentOn(typeof(PublishTask))]
public sealed class DefaultTask : FrostingTask { }

using Cake.Frosting;

namespace MaaDownloadServer.Build.Tasks;

[TaskName("Default")]
[IsDependentOn(typeof(PostPublishTask))]
public sealed class DefaultTask : FrostingTask { }

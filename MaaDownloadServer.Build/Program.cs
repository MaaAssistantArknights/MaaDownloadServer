using Cake.Frosting;
using MaaDownloadServer.Build;

var sArgs = new List<string>();
foreach (var arg in args)
{
    var sa = arg.Split(" ")
        .ToList();
    sa.RemoveAll(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));
    sArgs.AddRange(sa);
}

return new CakeHost()
    .UseContext<BuildContext>()
    .Run(sArgs);

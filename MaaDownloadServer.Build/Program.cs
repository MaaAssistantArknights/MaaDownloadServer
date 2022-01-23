using Cake.Frosting;
using MaaDownloadServer.Build;

return new CakeHost()
    .UseContext<BuildContext>()
    .Run(args);

# 运行配置

## 必要条件

* [.NET Runtime 6.0.*](https://dotnet.microsoft.com/en-us/download/dotnet) (如果有安装 .NET SDK，则已经包含)

## 运行

> 运行前请先根据文档下面的内容修改 `appsettings.json` 文件

### 直接运行

* 所有版本：命令行运行 `dotnet MaaDownloadServer.dll`
* `Windows` 版本：直接运行 `MaaDownloadServer.exe`
* `Linux/MacOs` 版本：直接运行 `MaaDownloadServer` 二进制文件

### Linux systemd

参考以下 service 文件：

``` text
[Unit]
Description=Maa Download Server

[Service]
WorkingDirectory=【DLL 所在路径】
ExecStart=【运行命令，可以使用 dotnet 启动 DLL，也可以直接写二进制文件路径】
Restart=always
RestartSec=10
SyslogIdentifier=MaaDownloadServer
User=【Linux 用户名】
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

* 将上面的内容复制到 `/etc/systemd/system/MaaDownloadServer.service` 文件中
* 运行 `sudo systemctl daemon-reload`
* 启动服务：`sudo systemctl start MaaDownloadServer`

## 配置文件 appsettings.json

> 仅说明可配置的配置项

> 所有路径相关的配置推荐写绝对路径

### Serilog

#### Serilog 日志组件的配置

* `MinimumLevel:Default`：日志级别，可选 `Trace`、`Debug`、`Information`、`Warning`、`Error`、`Critical`，默认为 `Information`，不建议选择 `Warning` 及以上的等级，`Debug` 和 `Trace` 仅供调试使用
* `WriteTo:[1]:Args:path`：日志文件保存路径，日志文件的文件名会为设定的文件名后缀前加上日期，推荐文件名设置为 `log-.log`

#### MaaServer 服务器配置项

* `Server:Host`：服务器监听地址，可以为 `*`
* `Server:Port`：服务器监听端口
* `Server:ApiFullUrl`：API 的外网可访问的完整路径，本地测试时可以是 `http://localhost:5000` 之类的，若开放至公网，则需要设置为 `https://www.maa.moe` 之类的
* `GithubQuery:ApiEndpoint`：Github Release API 终结点，只需要填入 `{USER}` 和 `{REPO}`
* `GithubQuery:Proxy`：Github 访问请求网络代理，留空为不使用代理
* `GithubQuery:Interval`：Github Release API 请求间隔，单位为分钟
* `GithubQuery:PackageName`：Github Release 中发布的包名，命名规则为 `{PackageName}-{Platform}-{Arch}-{Version}.zip`
* `DataDirectories:RootPath`：数据文件保存的根目录，请确保程序有读写权限
* `PublicContent:DefaultDuration`：默认公开内容的有效期，单位为天，默认为 30 天
* `PublicContent:AutoBundledDuration`：自动打包的内容的有效期，即新版本发布后自动打包的近期 3 个版本到最新版本的增量更新包，单位为天，默认为 60 天
* `PublicContent:OutdatedCheckInterval`：Public Content 过期检查时间间隔，单位为分钟，默认为 30 分钟
* `MemoryCache:ExpireTimeInMinutes`：内存缓存的过期时间，单位为分钟，默认为 10080 分钟
* `ZipRequiredFolder`：Release 的包中，需要打成一个压缩包视为一个资源的文件夹名称

#### IpRateLimiting 限流组件的配置

更加详细的内容请参考 [AspNetCoreRateLimit 文档](https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware#setup)

* `RealIpHeader`：使用反向代理时，获取真实 IP 的请求头，默认为 `X-Real-IP`
* `GeneralRules`：限流规则

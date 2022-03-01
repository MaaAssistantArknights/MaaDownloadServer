# 运行配置

## 必要条件

* [.NET Runtime 6.0.*](https://dotnet.microsoft.com/en-us/download/dotnet) (如果有安装 .NET SDK，则已经包含)

## 运行

### 直接运行

* 所有版本：命令行运行 `dotnet MaaDownloadServer.dll`
* `Windows` 版本：直接运行 `MaaDownloadServer.exe`
* `Linux/MacOs` 版本：直接运行 `MaaDownloadServer` 二进制文件

### Linux systemd

参考以下 service 文件：

```text
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

## 支持的环境变量

* `MAADS_DATA_DIRECTORY`: 数据文件目录，第一次运行时，会将配置文件拷贝到这个目录，并设置数据目录为这个环境变量指定的目录
* `MAA_DS_AZURE_APP_CONFIGURATION`: 使用 Azure App Configuration，环境变量值为 Azure App Configuration 的 Connection String，推荐使用只读权限的连接字符串

## 配置文件 appsettings.json

> 仅说明可配置的配置项

> 所有路径相关的配置推荐写绝对路径

### Serilog

#### Serilog 日志组件的配置

* `MinimumLevel:Default`：日志级别，可选 `Trace`、`Debug`、`Information`、`Warning`、`Error`、`Critical`，默认为 `Information`，不建议选择 `Warning` 及以上的等级，`Debug` 和 `Trace` 仅供调试使用
* `WriteTo:[1]:Args:path`：日志文件保存路径，日志文件的文件名会为设定的文件名后缀前加上日期，推荐文件名设置为 `log-.log`

#### MaaServer 服务器配置项

##### Server

* `Host`：服务器监听地址，可以为 `*`
* `Port`：服务器监听端口
* `ApiFullUrl`：API 的外网可访问的完整路径，本地测试时可以是 `http://localhost:5000` 之类的，若开放至公网，则需要设置为 `https://www.maa.moe` 之类的

##### Network

* `Proxy`: 网络代理地址，留空为不使用代理

##### GameData

* `UpdateJobInterval`: 游戏数据更新任务时间间隔，单位为分钟，默认为 120 分钟

##### DataDirectories

* `RootPath`：数据文件保存的根目录，请确保程序有读写权限，默认为 `MAADS_DATA_DIRECTORY` 环境变量所定义的路径

###### SubDirectories

* `Download`: 下载保存临时目录
* `Public`: 公共可访问下载的文件保存目录
* `Resources`: 资源文件目录
* `Database`: 数据库文件目录
* `Temp`: 临时文件目录
* `GameData`: 游戏数据目录
* `Scripts`: Component Python 脚本目录
* `VirtualEnvironments`: Python 虚拟环境目录

##### PublicContent

* `OutdatedCheckInterval`: 公共资源过期确认时间，单位为分钟，默认为 60 分钟
* `DefaultDuration`: 公共资源默认过期时间，单位为天，默认为 30 天
* `AutoBundledDuration`: 自动打包的公共资源过期时间，单位为天，默认为 60 天

##### MemoryCache

* `ExpireTimeInMinutes`：内存缓存的过期时间，单位为分钟，默认为 10080 分钟

##### ScriptEngine

* `Python`: Python 解释器路径，请确保安装了 pip 和 virtualenv

#### IpRateLimiting 限流组件的配置

更加详细的内容请参考 [AspNetCoreRateLimit 文档](https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware#setup)

* `RealIpHeader`：使用反向代理时，获取真实 IP 的请求头，默认为 `X-Real-IP`
* `GeneralRules`：限流规则

## 启动流程

### 第一次启动

1. 查找 `MAADS_DATA_DIRECTORY` 环境变量的值
2. 若 `MAADS_DATA_DIRECTORY` 值存在，则该环境变量值所指的目录作为 `数据目录`，若不存在，则程序根目录的 `data` 目录作为 `数据目录`
3. 复制 `appsettings.json` 到 `数据目录`
4. `appsettings.json` 中所有的 `{{DATA DIRECTORY}}` 会被替换为 `数据目录`
5. 若 `ASPNETCORE_ENVIRONMENT` 环境变量的值为 `appsettings.Development.json` 复制到 `数据目录`
6. 程序退出

### 后续启动

> 第一次启动后，你应该先配置 `appsettings.json` 文件

当 `appsettings.json` 文件在指定的 `数据目录` 中存在时，将会正常启动程序

程序配置文件的加载顺序 (后面的会覆盖前面的):

1. appsettings.json 文件
2. appsettings.Development.json 文件，可选
3. Azure App Configuration 配置提供器，由 `MAA_DS_AZURE_APP_CONFIGURATION` 指定，变量为空即为不使用
4. 所有以 `MAA:` 开头的环境变量
5. 命令行参数

例如:

1. 在 `appsettings.json` 文件中定义了 `MaaServer:Server:Port` 为 10010
2. 在环境变量中有 `MAA:MaaServer:Server:Port` 为 10011
3. 在命令行参数有 `--MaaServer:Server:Port=10012`

最终，该配置项值为 10012

# MaaDownloadServer

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/MaaAssistantArknights/MaaDownloadServer/build-test?label=CI%3Abuild-test&logo=github)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/MaaAssistantArknights/MaaDownloadServer/publish-docker?label=CI%3Apublish-docker&logo=github)
![C Sharp](https://img.shields.io/badge/C%23-10-239120?logo=csharp)
![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?logo=.net)
![GitHub](https://img.shields.io/github/license/MaaAssistantArknights/MaaDownloadServer)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/MaaAssistantArknights/MaaDownloadServer)

![Docker Image Version (latest by date)](https://img.shields.io/docker/v/alisaqaq/maa-download-server?arch=amd64&label=Docker%20Image%20%28amd64%29&logo=docker)
![Docker Image Version (latest by date)](https://img.shields.io/docker/v/alisaqaq/maa-download-server?arch=arm64&label=Docker%20Image%20%28arm64%29&logo=docker)
![Docker Image Version (latest by date)](https://img.shields.io/docker/v/alisaqaq/maa-download-server?arch=arm&label=Docker%20Image%20%28arm%2Fv7%29&logo=docker)

## 关于

MAA 更新和下载服务器

用于 MAA 本体在国内的下载加速，以及提供增量更新包

## 功能

* 提供 API 用于检查不同平台和架构所支持的 MAA 版本
* 提供 MAA 本体完整包的下载
* 提供 MAA 增量更新包的下载


## 文档

* [API 文档 (ApiFox)](https://www.apifox.cn/apidoc/shared-e9acdf71-e5e6-4198-aaa7-5417e1304335)
* [编译和打包](./docs/Compile.md)
* [本地运行配置](./docs/RunNative.md)
* [Docker 运行配置](./docs/RunDocker.md)
* [Component 和 Python 脚本](./docs/ComponentAndPythonScript.md)

# 致谢

感谢本项目使用到的开源项目/开源库：

- 框架：[ASP.NET Core](https://github.com/dotnet/aspnetcore) ![GitHub](https://img.shields.io/github/license/dotnet/aspnetcore)
- JSON 字符串处理：[EscapeRoute](https://github.com/JackWFinlay/EscapeRoute) ![GitHub](https://img.shields.io/github/license/JackWFinlay/EscapeRoute)
- 限流：[AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit) ![GitHub](https://img.shields.io/github/license/stefanprodan/AspNetCoreRateLimit)
- 计划任务：[Quartz.NET](https://github.com/quartznet/quartznet) ![GitHub](https://img.shields.io/github/license/quartznet/quartznet)
- 版本号解析：[Semver](https://github.com/maxhauser/semver) ![GitHub](https://img.shields.io/github/license/maxhauser/semver)
- 日志记录：[Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore) ![GitHub](https://img.shields.io/github/license/serilog/serilog-aspnetcore)
- ORM：[Entity Framework Core](https://github.com/dotnet/efcore) ![GitHub](https://img.shields.io/github/license/dotnet/efcore)
- 自动构建：[Cake.Frosting](https://github.com/cake-build/cake) ![GitHub](https://img.shields.io/github/license/cake-build/cake)

# 许可证

本项目使用 [GNU AFFERO GENERAL PUBLIC LICENSE Version 3](./LICENSE) 授权

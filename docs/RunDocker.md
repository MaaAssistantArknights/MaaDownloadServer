# Docker 运行配置

## 必要条件

* [Docker Desktop (Windows/macOS)](https://docs.docker.com/desktop/)
* [Docker Engine (Linux)](https://docs.docker.com/engine/)

## Docker 镜像

MaaDS 提供 `amd64` `arm64` `arm/v7` 架构的 Linux 容器镜像。

[Docker Hub 链接](https://hub.docker.com/r/alisaqaq/maa-download-server)

### 镜像 Tag

镜像 Tag 中的版本号与 GitHub Release 版本号一致，当前不同架构的最新版本如下。

![Docker Image Version (latest by date)](https://img.shields.io/docker/v/alisaqaq/maa-download-server?arch=amd64&label=Docker%20Image%20%28amd64%29&logo=docker)
![Docker Image Version (latest by date)](https://img.shields.io/docker/v/alisaqaq/maa-download-server?arch=arm64&label=Docker%20Image%20%28arm64%29&logo=docker)
![Docker Image Version (latest by date)](https://img.shields.io/docker/v/alisaqaq/maa-download-server?arch=arm&label=Docker%20Image%20%28arm%2Fv7%29&logo=docker)

单独架构的镜像 Tag 命名规则遵循 `v{Semantic Version}-{Arch}` 的格式。

某个版本的最新版所有架构的集合 Manifest Tag 命名规则遵循 `v{Semantic Version}` 的格式。

最新版所有架构的集合 Manifest Tag 命名规则遵循 `latest` 的格式。

Tag 列表请看 [Docker Hub]()

### 镜像源

MaaDS Docker 镜像保存在 Docker Hub 和腾讯云容器镜像服务 TCR 上。

从 Docker Hub 拉取镜像：

```shell
docker pull alisaqaq/maa-download-server
```

从腾讯云 TCR 上拉取镜像：

```shell
docker pull ccr.ccs.tencentyun.com/alisaqaq/maa-download-server
```

## 运行

### 运行流程

1. 创建并运行容器
2. 容器第一次运行会复制配置文件到数据目录，然后停止
3. 修改配置文件
4. 使用 `docker start [Container]` 命令在其启动容器

### Docker 直接运行

> 如果使用来自腾讯云 TCR 上的镜像，请将 `alisaqaq/maa-download-server` 替换为 `ccr.ccs.tencentyun.com/alisaqaq/maa-download-server`

```shell
docker run -p 外部端口:80 -v 数据保存路径:/app/data --name maads -d alisaqaq/maa-download-server
```

### Docker Compose 运行

```yaml
version: "3"

services:
  maads:
   image: alisaqaq/maa-download-server
   volumes:
     - 数据保存路径:/app/data
   ports:
     - 外部端口:80
```

## 配置

使用 Docker 运行时，***请勿修改***以下选项：

- `MaaServer:ScriptEngine:Python` 将会使用容器内部的 Python 3.9.2 解释器
- `MaaServer:Server:Url` 该值不生效
- `MaaServer:Server:Port` 该值不生效
- `MaaServer:DataDirectories:RootPath` 仅能为容器内指定路径
- `Serilog:WriteTo[1].path` 仅能为容器内指定路径

使用 Docker 运行时，***请勿配置***以下环境变量：

- `DOTNET_RUNNING_IN_CONTAINER` 该值为 `True`，用于程序判断是否在容器中运行
- `ASPNETCORE_URLS` 该值为 `http://+:80`，为容器中运行时绑定的端口

关于其他配置项目，请参考 [本地运行配置中的详细说明](./RunNative.md)

# 编译和打包

## 发布到本机

### 必要条件

* [.NET SDK 6.0.*](https://dotnet.microsoft.com/en-us/download/dotnet)

### 步骤

* 安装 .NET SDK 6.0
* 用任何方法把这个仓库里的代码弄到本地
* 打开命令行 (CMD/PowerShell/Bash...)
* CD 到项目目录
* 执行 `publish.sh` 或者 `publish.ps1` 文件，参数见下面
* Publish 文件在 `./publish` 目录下，同时会打包为 `./MaaDownloadServer-{Configuration}-{Framework}-{RID}.zip` 文件

### Publish 参数

运行格式：`./publish.sh [options]` 或者 `./publish.ps1 [options]`

Options 格式：`--<option>=<value>`

#### 可用参数

* `--configuration` 选项指定编译配置，可选 `Release` 或 `Debug`，默认为 `Release`
* `--rid` 选项指定编译平台，可选项请参考 [RID 列表](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) ，默认编译为 `Portable`
* `--framework` 选定 .NET 版本，目前仅在 `net6.0` 下测试过，默认为 `net6.0`，变更此选项需要更改 `csproj` 文件
* `--maads-version` MaaDS 版本号，SemVer 格式，将作为 `AssemblyVersion` `AssemblyInformationVersion` `FileVersion` 写入程序集，默认为 `0.0.0`

### 示例

1. 发布 Linux x64 Release 版本
    ```shell
    ./publish.sh --rid=linux-x64
    ```
2. 发布 MacOS Monterey arm64 Debug 版本
    ```shell
   ./publish.sh --rid=osx.12-arm64 --configuration=Debug
   ```
3. 发布 Windows 10/11 x64 Release 版本
    ```shell
   ./publish.sh --rid=win10-x64
   ```

## 发布到 Docker 镜像

### 必要条件

* [.NET SDK 6.0.*](https://dotnet.microsoft.com/en-us/download/dotnet)
* [Docker Desktop (Windows/macOS)](https://docs.docker.com/desktop/)
* [Docker Engine (Linux)](https://docs.docker.com/engine/)

> 在 Linux 下，`amd64` 架构的系统只能制作 `linux/amd64` 和 `linux/386` 架构的镜像，若要跨平台制作镜像，你可能需要安装 QEMU，具体参考 [这篇 Docker 官方的博客](https://www.docker.com/blog/multi-platform-docker-builds/)

> 在 Windows 和 macOS 下，Docker Desktop 已具备跨平台制作镜像的功能

### 步骤

* 安装 .NET SDK 6.0
* 安装并启动 Docker Desktop 或 Docker Engine
* 用任何方法把这个仓库里的代码弄到本地
* 打开命令行 (CMD/PowerShell/Bash...)
* CD 到项目目录
* 执行 `publish-docker.sh` 或者 `publish-docker.ps1` 文件，参数见下面

### Publish 参数

运行格式：`./publish-docker.sh <Tag> [Arches]` 或者 `./publish-docker.ps1 <Tag> [Arches]`

* Tag 即 Docker Image 的版本 Tag，如果制作 `linux/amd64` 的镜像，会生成名为 `maa-download-server:<Tag>-amd64` 的镜像
* Arches 为打包镜像的架构，使用 `,` 分隔，默认为 `amd64,arm64,arm/v7`，只支持这三种架构

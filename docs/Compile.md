# 编译和打包

## 必要条件

* [.NET SDK 6.0.*](https://dotnet.microsoft.com/en-us/download/dotnet)

## 步骤

* 安装 .NET SDK 6.0
* 用任何方法把这个仓库里的代码弄到本地
* 打开命令行 (CMD/PowerShell/Bash...)
* CD 到项目目录
* 执行 `publish.sh` 或者 `publish.sh1` 文件，参数见下面
* Publish 文件在 `./publish` 目录下，同时会打包为 `./MaaDownloadServer-{Configuration}-{Framework}-{RID}.zip` 文件

## Publish 参数

运行格式：`./publish.sh [options]`

Options 格式：`--<option> <value>`

### 可用参数

* `--configuration` 选项指定编译配置，可选 `Release` 或 `Debug`，默认为 `Release`
* `--rid` 选项指定编译平台，可选项请参考 [RID 列表](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) ，默认编译为 `Portable`
* `--framework` 选定 .NET 版本，目前仅在 `net6.0` 下测试过，默认为 `net6.0`，变更此选项需要更改 `csproj` 文件

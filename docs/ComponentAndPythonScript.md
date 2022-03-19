# Component 和 Python 脚本

## 关于 Component

Maa 存在多个组件，多个组件以不同的方式进行更新，为了同时支持多个组件包，MaaDownloadServer 引入了 Python 脚本用于处理一些 Component 相关的信息

每一个组件的配置均需要在程序运行前存放在 `scripts` 目录中，目录结构如下

```text
scripts
|
|-- MaaComponent1
|        |
|        |-- component.json
|        |-- requirements.txt
|        |-- some_script.py
|        |-- another_script.py
|
|-- MaaComponent2
|        |
|        |-- component.json
|        |-- component_2_script.py
|        |-- yet_another_script.py
```

> Component 目录名必须和 `component.json` 中的 `name` 相同

当程序执行时，会对每一个 Component 在 `venvs` 目录中建立用于运行脚本的虚拟环境

更新程序对于 Component 的更新方式存在一些限制:

- 每个 `平台-架构` 组合所对应的包必须为一个文件

## Demo

[配置参考](../demo/scripts/MeoAssistantArkNight)

## Component Package 配置文件 `component.json`

```json
{
    "name": "MeoAssistantArkNight",
    "description": "Maa 核心 (MaaDS 测试用)",
    "metadata_urls": [
        "https://api.github.com/repos/LiamSho/MAA_Download_Test/releases/{version}"
    ],
    "default_url_placeholder": {
        "version": "latest"
    },
    "after_download_process": {
        "operation": "Unzip",
        "args": {}
    },
    "before_add_process": {
        "operation": "Zip",
        "args": {
            "zip": [
                {
                    "name": "resource",
                    "files": [],
                    "folders": [
                        "resource"
                    ]
                }
            ]
        }
    },
    "scripts": {
        "get_download_info": "get_download_info.py",
        "after_download_process": "",
        "before_add_process": "",
        "relative_path_calculation": ""
    },
    "use_proxy": true,
    "pack_update_package": true,
    "interval": 120
}
```

## 更新流程

> `interval` 为更新任务执行间隔，单位为分钟

> 若 `use_proxy` 为 `true`，则所有的 API 请求都会使用 `MaaServer:Network:Proxy` 所指定的代理

> 若 Python 脚本在标准错误输出流 `sys.stderr.write('Error Info')` 中有输出信息，则视为执行失败，若在标准输出流 `sys.stdout.write(output)` 中输出了信息，视为脚本返回值

### STEP 1: 请求 Metadata API

按照顺序请求 `metadata_urls`，其中 `{}` 内的内容会使用 `default_url_placeholder` 中指定的内容替代，请求得到内容应为一个 Json 字符串

> url_placeholder 会在预定的功能更新 (手动触发组件更新) 中使用到

### STEP 2: 运行 Python 脚本获取下载信息列表

运行 `scripts:get_download_info` 所指定的 Python 脚本，传入参数第一个值为脚本文件名，第二个参数为 `component.json` 字符串，后续参数为步骤 1 所获的的 Json 字符串

Python 脚本应在标准输出流中输出处理结果，示例 Json 字符串为:

```json
[
    {
        "version":"2.0.2",
        "update_time":"2022-01-22T16:40:18Z",
        "update_log":"2.0.2 更新日志",
        "platform":"Windows",
        "arch":"arm64",
        "download_url":"https://github.com/LiamSho/MAA_Download_Test/releases/download/v2.0.2/MeoAssistantArkNight-Windows-arm64-2.0.2.zip",
        "file_extension":"zip",
        "checksum":"",
        "checksum_type":"none"
    },
    {
        "version":"2.0.2",
        "update_time":"2022-01-22T16:40:18Z",
        "update_log":"2.0.2 更新日志",
        "platform":"Windows",
        "arch":"x64",
        "download_url":"https://github.com/LiamSho/MAA_Download_Test/releases/download/v2.0.2/MeoAssistantArkNight-Windows-x64-2.0.2.zip",
        "file_extension":"zip",
        "checksum":"",
        "checksum_type":"none"
    }
]
```

| 字段              | 类型     | 可选项                                | 说明                      |
|-----------------|--------|------------------------------------|-------------------------|
| version         | string |                                    | Semantic Version 版本号字符串 |
| update_time     | string |                                    | ISO-8601 标准日期和时间字符串     |
| update_log      | string |                                    | 更新日志                    |
| platform        | string | Windows/MacOS/Linux/NoPlatform     | 平台                      |
| arch            | string | x64/arm64/NoArch                   | 架构                      |
| download_url    | string |                                    | 下载链接                    |
| file_extension  | string |                                    | 文件后缀，不带 `.` 号           |
| checksum        | string |                                    | 校验码                     |
| checksum_type   | string | none/md5/sha1/sha256/sha384/sha512 | 校验码类型，为 none 即不校验       |

### STEP 3: 检查版本号，检查数据库

检查数据库，若存在组件名、版本号、平台、架构一致的版本，则表示该包已存在于数据库中，跳过更新

### STEP 4, 5: 下载文件 & 校验文件，校验失败则返回重试，最多尝试下载3次

根据 `download_url` 下载文件，文件将会保存到 `downloads/{jobId}/{fileId}.{file_extension}`，若有校验码，下载后会检查校验码，下载失败将会删除下载文件重新下载，最多重试三次，三次失败将会退出更新

### STEP 6: 执行 AfterDownloadProcess

读取值 `after_download_process:operation`，该值可以为 `Unzip` `None` `Custom`

- `Unzip`: 解压缩文件，下载下来的文件必须是 `zip` 格式，压缩包将会解压至 `temps/{jobId}/{fileId}` 目录
- `None`: 将文件直接移动至 `temps/{jobId}/{fileId}` 目录
- `Custom`: 执行自定义的 Python 脚本 `scripts:after_download_process`，传入参数第一个为脚本文件名，第二个为序列化后的 `after_download_process:args` Json 字符串，第三个为源文件路径，第四个文件为 `temps/{jobId}/{fileId}` 目录路径

### STEP 7: 将完整包加入PublicContent

存在于 `downloads/{jobId}` 目录中的版本完整包加入 `Public Content`

### STEP 8: 执行 BeforeAddProcess

读取值 `before_add_process:operation`，该值可以为 `Zip` `None` `Custom`

- `Zip`: 压缩文件，需要参数，参考示例 Json 文件
- `None`: 不执行任何操作
- `Custom`: 执行自定义的 Python 脚本 `scripts:before_add_process`，传入第一个参数为脚本文件名，第二个为 `temps/{jobId}/{fileId}` 目录路径，第三个为序列化后的 `before_add_process:args` Json 字符串

### STEP 9: 遍历资源，计算 MD5 Hash 建立 ResourceInfo，可能需要运行Python脚本计算相对路径，第一次去除重复

遍历所有的资源，默认以 `temps/{jobId}/{fileId}` 目录为 Root，计算各个资源文件的路径

若 Python 脚本 `scripts:relative_path_calculation` 不为空，则将执行该脚本计算相对路径，传入第一个参数为脚本文件名，第二个为资源文件完整路径，第三个为 `temps/{jobId}/{fileId}` 目录路径，计算结果从标准输出流输出

### STEP 10: Resource 与数据库比对，去除重复

将遍历得到的 Resource 与数据库比对，去除重复的资源

### STEP 11: 添加 Resource 进入数据库

将新增的 Resource 加入数据库

### STEP 12: 根据 RelativePath，FileName 和 Hash 从数据库检索包文件并建立包对象添加进入数据库

查询数据库，建立 Package 加入数据库

### STEP 13: 获取近期 3 个版本的包对象，比对 Resource 并打包更新包

若 `pack_update_package` 为 `false`，则不执行此步骤

获取近期 3 个版本的包对象，比对 Resource 打包成增量更新包，加入 `Public Content`

### STEP 14: 完成

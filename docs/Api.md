# API 文档

## 全局 API 返回值

所有 API 返回均存在以下状态码：

* `200 OK`：请求成功，返回数据
* `404 NotFound`：请求的资源不存在
* `429 TooManyRequests`：触发了访问频率限制
* `500 InternalServerError`：服务器内部错误，若出现这个状态码，请进行 Bug 反馈

所有 API 返回均存在以下 Header：

* `X-Rate-Limit-Limit:`：访问频率限制时间范围
* `X-Rate-Limit-Remaining`：在 `X-Rate-Limit-Limit:` 指定的时间范围内剩余请求次数
* `X-Rate-Limit-Reset`：请求次数刷新时间，ISO-8601 格式，例如 `2022-01-23T15:32:13.1991650Z`

## API 列表

所有 API 均为 GET 请求

### Version API

#### 获取支持的所有平台

```http request
GET /version/getPlatform
```

返回值示例：

```json
{
  "support_platform": [
    "windows",
    "macos",
    "linux"
  ]
}
```

#### 获取某平台支持的所有架构

```http request
GET /version/{{platform}}/getArch
```

| 参数       | 位置  | 说明                                     |
|----------|-----|----------------------------------------|
| platform | URL | 平台，不区分大小写，可选 `Windows`、`Linux`、`MacOs` |

返回值示例：

```json
{
  "platform": "windows",
  "support_arch": [
    "arm64",
    "x64"
  ]
}
```

#### 获取某平台、某架构支持的版本

```http request
GET /version/{{platform}}/{{arch}}/getVersion?page={{page}}
```

| 参数       | 位置    | 说明                                     |
|----------|-------|----------------------------------------|
| platform | URL   | 平台，不区分大小写，可选 `Windows`、`Linux`、`MacOs` |
| arch     | URL   | 架构，不区分大小写，可选 `arm64`、`x64`             |
| page     | Query | 分页参数，大于等于 1 的整数                        |

返回值示例：

```json
{
  "platform": "windows",
  "arch": "x64",
  "versions": [
    {
      "version": "2.0.2",
      "publish_time": "2022-01-23T00:40:18"
    },
    {
      "version": "2.0.1",
      "publish_time": "2022-01-22T16:03:30"
    }
  ]
}
```

#### 获取某平台、某架构、某版本的详细信息

```http request
GET /version/{{platform}}/{{arch}}/{{version}}
```

| 参数        | 位置  | 说明                                     |
|-----------|-----|----------------------------------------|
| platform  | URL | 平台，不区分大小写，可选 `Windows`、`Linux`、`MacOs` |
| arch      | URL | 架构，不区分大小写，可选 `arm64`、`x64`             |
| version   | URL | 版本，Semantic Version 标准版本号字符串           |

返回值示例：

```json
{
  "platform": "windows",
  "arch": "x64",
  "details": {
    "version": "2.0.1",
    "publish_time": "2022-01-22T16:03:30",
    "update_log": "测试内容\r\n\r\n* test\r\n",
    "resources": [
      {
        "file_name": "resource.zip",
        "path": "/",
        "hash": "BD8509A6AEAFCFA4CB8ABD067EB528D3"
      },
      {
        "file_name": "MeoAssistant.dll",
        "path": "/",
        "hash": "1E21CDA9E32F96746244DD7952429E83"
      }
    ]
  }
}
```

### Download API

### 获取某平台、某架构、某版本的完整包

```http request
GET /download/{{platform}}/{{arch}}/{{version}}
```


| 参数        | 位置  | 说明                                     |
|-----------|-----|----------------------------------------|
| platform  | URL | 平台，不区分大小写，可选 `Windows`、`Linux`、`MacOs` |
| arch      | URL | 架构，不区分大小写，可选 `arm64`、`x64`             |
| version   | URL | 版本，Semantic Version 标准版本号字符串           |

返回值示例：

```json
{
  "platform": "windows",
  "arch": "x64",
  "version": "2.0.1",
  "url": "http://localhost:5089/files/990d398d-f0c6-4fa5-85d6-b5d34fcc0ff6.zip",
  "hash": "711B8E1CDDE2CB85903F19C8384BEBF6"
}
```

### 获取某平台、某架构、从某个版本至另外一个版本的增量包

```http request
GET /download/{{platform}}/{{arch}}?from={{from}}&to={{to}}
```


| 参数       | 位置    | 说明                                     |
|----------|-------|----------------------------------------|
| platform | URL   | 平台，不区分大小写，可选 `Windows`、`Linux`、`MacOs` |
| arch     | URL   | 架构，不区分大小写，可选 `arm64`、`x64`             |
| from     | Query | 起始版本，Semantic Version 标准版本号字符串         |
| to       | Query | 目标版本，Semantic Version 标准版本号字符串         |

返回值示例：

```json
{
  "platform": "windows",
  "arch": "x64",
  "version": "2.0.1 -> 2.0.2",
  "url": "http://localhost:5089/files/bc8af461-69ef-4be1-ab91-1d33e04be705.zip",
  "hash": "73EFC155319D1763E84C461D9CB094CC"
}
```

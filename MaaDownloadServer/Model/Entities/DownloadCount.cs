using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

/// <summary>
/// 下载文件 API 访问次数统计
/// </summary>
[Table("download_count")]
public record DownloadCount
{
    /// <summary>
    /// ID
    /// </summary>
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// 组件名
    /// </summary>
    [Column("component_name")]
    public string ComponentName { get; set; }

    /// <summary>
    /// 源版本
    /// </summary>
    [Column("from_version")]
    public string FromVersion { get; set; }

    /// <summary>
    /// 目标版本，为空表示版本完整包
    /// </summary>
    [Column("to_version")]
    public string ToVersion { get; set; }

    /// <summary>
    /// 总计下载次数
    /// </summary>
    [Column("count")]
    public int Count { get; set; }
};

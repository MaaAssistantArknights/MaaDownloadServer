using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

/// <summary>
/// 可下载的包
/// </summary>
/// <param name="Id">ID</param>
/// <param name="Tag">标签</param>
/// <param name="AddTime">添加时间</param>
/// <param name="Hash">MD5 校验码</param>
/// <param name="Duration">过期时间</param>
[Table("public_content")]
public record PublicContent(Guid Id, string Tag, DateTime AddTime, string Hash, DateTime Duration)
{
    /// <summary>
    /// 公共资源 ID
    /// </summary>
    [Column("id")]
    public Guid Id { get; set; } = Id;

    /// <summary>
    /// 标签
    /// </summary>
    [Column("tag")]
    public string Tag { get; set; } = Tag;

    /// <summary>
    /// 过期时间
    /// </summary>
    [Column("duration")]
    public DateTime Duration { get; set; } = Duration;

    /// <summary>
    /// 文件 MD5 校验
    /// </summary>
    [Column("hash")]
    public string Hash { get; set; } = Hash;

    /// <summary>
    /// 添加时间
    /// </summary>
    [Column("add_time")]
    public DateTime AddTime { get; set; } = AddTime;
}

using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Entities;

public record PublicContent(Guid Id, string Tag, DateTime AddTime, string Hash, DateTime Duration)
{
    /// <summary>
    /// 公共资源 ID
    /// </summary>
    [JsonIgnore]
    public Guid Id { get; set; } = Id;

    /// <summary>
    /// 标签
    /// </summary>
    [JsonPropertyName("tag")]
    public string Tag { get; set; } = Tag;

    /// <summary>
    /// 过期时间
    /// </summary>
    [JsonPropertyName("duration")]
    public DateTime Duration { get; set; } = Duration;

    /// <summary>
    /// 文件 MD5 校验
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = Hash;

    /// <summary>
    /// 添加时间
    /// </summary>
    [JsonPropertyName("add_time")]
    public DateTime AddTime { get; set; } = AddTime;
}

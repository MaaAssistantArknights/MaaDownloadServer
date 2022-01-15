using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Entities;

public record PublicContent
{
    /// <summary>
    /// 公共资源 ID
    /// </summary>
    [JsonIgnore]
    public Guid Id { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime? Duration { get; set; }

    /// <summary>
    /// 文件 MD5 校验
    /// </summary>
    public string Hash { get; set; }
}

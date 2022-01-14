using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Entities;

/// <summary>
/// 包（一个平台的所有文件）
/// </summary>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="Platform"></param>
/// <param name="Architecture"></param>
public record Package(Guid Id, string Version, Platform Platform, Architecture Architecture)
{
    /// <summary>
    /// 包 Id
    /// </summary>
    [JsonIgnore]
    public Guid Id { get; set; } = Id;

    /// <summary>
    /// 版本
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = Version;

    /// <summary>
    /// 平台
    /// </summary>
    [JsonPropertyName("platform")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Platform Platform { get; set; } = Platform;

    /// <summary>
    /// 架构
    /// </summary>
    [JsonPropertyName("arch")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Architecture Architecture { get; set; } = Architecture;

    /// <summary>
    /// 资源
    /// </summary>
    [JsonPropertyName("resources")]
    public List<Resource> Resources { get; set; }

    /// <summary>
    /// 发包时间
    /// </summary>
    [JsonPropertyName("publish_time")]
    public DateTime PublishTime { get; set; }

    /// <summary>
    /// 版本下载次数
    /// </summary>
    [JsonIgnore]
    public uint DownloadTimes  { get; set; }
}

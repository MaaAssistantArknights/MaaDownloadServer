using System.Text.Json.Serialization;

namespace MaaDownloadServer.Model.Entities;

/// <summary>
/// 资源（单指一个文件）
/// </summary>
/// <param name="Id"></param>
/// <param name="FileName"></param>
/// <param name="Path"></param>
/// <param name="Hash"></param>
public record Resource(Guid Id, string FileName, string Path, string Hash)
{
    /// <summary>
    /// 包 Id
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Id;

    /// <summary>
    /// 文件名
    /// </summary>
    [JsonPropertyName("file_name")]
    public string FileName { get; set; } = FileName;

    /// <summary>
    /// 文件保存路径
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = Path;

    /// <summary>
    /// 文件 MD5 哈希值
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = Hash;

    /// <summary>
    /// 对应包
    /// </summary>
    [JsonIgnore]
    public List<Package> Packages { get; set; }

    /// <summary>
    /// 资源下载次数统计
    /// </summary>
    [JsonIgnore]
    public int DownloadTimes { get; set; }
}

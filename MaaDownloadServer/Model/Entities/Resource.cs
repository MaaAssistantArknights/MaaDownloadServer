using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

/// <summary>
/// 资源（单指一个文件）
/// </summary>
/// <param name="Id">ID</param>
/// <param name="FileName">文件名</param>
/// <param name="Path">保存路径</param>
/// <param name="Hash">MD5 校验码</param>
[Table("resource")]
public record Resource(Guid Id, string FileName, string Path, string Hash)
{
    /// <summary>
    /// 包 Id
    /// </summary>
    [Column("id")]
    public Guid Id { get; set; } = Id;

    /// <summary>
    /// 文件名
    /// </summary>
    [Column("file_name")]
    public string FileName { get; set; } = FileName;

    /// <summary>
    /// 文件保存路径
    /// </summary>
    [Column("path")]
    public string Path { get; set; } = Path;

    /// <summary>
    /// 文件 MD5 哈希值
    /// </summary>
    [Column("hash")]
    public string Hash { get; set; } = Hash;

    /// <summary>
    /// 对应包
    /// </summary>
    [Column("packages")]
    public List<Package> Packages { get; set; }
}

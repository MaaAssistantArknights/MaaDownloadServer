using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

/// <summary>
/// 包（一个平台的所有文件）
/// </summary>
/// <param name="Id">ID</param>
/// <param name="Component">组件名称</param>
/// <param name="Version">版本</param>
/// <param name="Platform">平台</param>
/// <param name="Architecture">架构</param>
/// <param name="PublishTime">发布时间</param>
/// <param name="UpdateLog">更新日志</param>
[Table("package")]
public record Package(Guid Id, string Component, string Version,
    Platform Platform, Architecture Architecture,
    DateTime PublishTime, string UpdateLog)
{
    /// <summary>
    /// 包 Id
    /// </summary>
    [Column("id")]
    public Guid Id { get; set; } = Id;

    /// <summary>
    /// 版本
    /// </summary>
    [Column("version")]
    public string Version { get; set; } = Version;

    /// <summary>
    /// 平台
    /// </summary>
    [Column("platform")]
    public Platform Platform { get; set; } = Platform;

    /// <summary>
    /// 架构
    /// </summary>
    [Column("architecture")]
    public Architecture Architecture { get; set; } = Architecture;

    /// <summary>
    /// 资源
    /// </summary>
    [Column("resources")]
    public List<Resource> Resources { get; set; } = new();

    /// <summary>
    /// 发包时间
    /// </summary>
    [Column("publish_time")]
    public DateTime PublishTime { get; set; } = PublishTime;

    /// <summary>
    /// 更新日志
    /// </summary>
    [Column("update_log")]
    public string UpdateLog { get; set; } = UpdateLog;

    /// <summary>
    /// 组件名称
    /// </summary>
    [Column("component")]
    public string Component { get; set; } = Component;
}

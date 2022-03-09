using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

/// <summary>
/// 临时数据表
/// </summary>
[Table("database_cache")]
public record DatabaseCache
{
    /// <summary>
    /// ID
    /// </summary>
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// 检索 ID
    /// </summary>
    [Column("query_id")]
    public string QueryId { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [Column("value")]
    public string Value { get; set; }
}

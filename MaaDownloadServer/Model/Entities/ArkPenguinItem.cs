using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

/// <summary>
/// 🐧物流数据站物品数据
/// </summary>
[Table("ark_penguin_item")]
public sealed record ArkPenguinItem
{
    /// <summary>
    /// 物品 Id (🐧物流)
    /// </summary>
    [Key]
    [Column("item_id")]
    public string ItemId { get; set; }

    /// <summary>
    /// 物品名称
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    /// 排序 Id
    /// </summary>
    [Column("sort_id")]
    public int SortId { get; set; }

    /// <summary>
    /// 稀有度
    /// </summary>
    [Column("rarity")]
    public int Rarity { get; set; }

    /// <summary>
    /// 物品分类
    /// </summary>
    [Column("item_type")]
    public string ItemType { get; set; }

    /// <summary>
    /// 美服是否存在
    /// </summary>
    [Column("us_exist")]
    public bool UsExist { get; set; }

    /// <summary>
    /// 日服是否存在
    /// </summary>
    [Column("jp_exist")]
    public bool JpExist { get; set; }

    /// <summary>
    /// 韩服是否存在
    /// </summary>
    [Column("kr_exist")]
    public bool KrExist { get; set; }

    /// <summary>
    /// 国服是否存在
    /// </summary>
    [Column("cn_exist")]
    public bool CnExist { get; set; }

    /// <summary>
    /// 汉语名称
    /// </summary>
    [Column("zh_name")]
    public string ZhNameI18N { get; set; }

    /// <summary>
    /// 英语名称
    /// </summary>
    [Column("en_name")]
    public string EnNameI18N { get; set; }

    /// <summary>
    /// 日语名称
    /// </summary>
    [Column("jp_name")]
    public string JpNameI18N { get; set; }

    /// <summary>
    /// 韩语名称
    /// </summary>
    [Column("ko_name")]
    public string KoNameI18N { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

/// <summary>
/// 从 PRTS Wiki 获取物品信息
/// </summary>
[Table("ark_prts_item")]
public record ArkPrtsItem
{
    /// <summary>
    /// PRTS Wiki 物品 ID 并不唯一, 因此生成一个 Guid 用作主键
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// 物品 ID
    /// </summary>
    [Column("item_id")]
    public string ItemId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    /// 介绍
    /// </summary>
    [Column("description")]
    public string Description { get; set; }

    /// <summary>
    /// 用途
    /// </summary>
    [Column("usage")]
    public string Usage { get; set; }

    /// <summary>
    /// 获取方法
    /// </summary>
    [Column("obtain")]
    public string ObtainMethod { get; set; }

    /// <summary>
    /// 稀有度, 值为 -1 时表示从 PRTS Wiki 获取和解析该值失败
    /// </summary>
    [Column("rarity")]
    public int Rarity { get; set; }

    /// <summary>
    /// 图片
    /// </summary>
    [Column("image")]
    public string Image { get; set; }

    /// <summary>
    /// 图片下载链接
    /// </summary>
    [Column("image_download_url")]
    public string ImageDownloadUrl { get; set; }

    /// <summary>
    /// 分类, 数据库中分类名使用 ;; 分隔
    /// </summary>
    [Column("category")]
    public List<string> Category { get; set; }

    public bool Equal(ArkPrtsItem other)
    {
        if (other is null)
        {
            return false;
        }

        if (Category.EqualWith(other.Category) is false)
        {
            return false;
        }

        var cId = Guid.NewGuid();

        var otherWithoutList = other with { Category = null, Id = cId };
        var thisWithoutList = this with { Category = null, Id = cId };
        return otherWithoutList == thisWithoutList;
    }
}

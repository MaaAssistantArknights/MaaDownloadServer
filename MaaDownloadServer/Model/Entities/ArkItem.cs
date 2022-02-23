using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

[Table("ark_item")]
public record ArkItem(int ItemId, string Name, string Description, string Usage, string ObtainApproach, int Rarity, string Image, string Category)
{
    [Column("id")]
    public int Id { get; set; }

    [Column("item_id")]
    public int ItemId { get; set; } = ItemId;

    [Column("name")]
    public string Name { get; set; } = Name;

    [Column("description")]
    public string Description { get; set; } = Description;

    [Column("usage")]
    public string Usage { get; set; } = Usage;

    [Column("obtain_approach")]
    public string ObtainApproach { get; set; } = ObtainApproach;

    [Column("rarity")]
    public int Rarity { get; set; } = Rarity;

    [Column("image")]
    public string Image { get; set; } = Image;

    [Column("category")]
    public string Category { get; set; } = Category;
}

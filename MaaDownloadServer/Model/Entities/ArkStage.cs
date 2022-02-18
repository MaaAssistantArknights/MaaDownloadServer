using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

[Table("ark_stage")]
public record ArkStage(
    string Type,
    string ZoneId,
    string StageId,
    string ZoneName,
    string StageName,
    int SanityCost,
    string? OpenCN,
    string? CloseCN,
    bool ExistCN,
    string? OpenGlobal,
    string? CloseGlobal,
    bool ExistGlobal
)
{
    [Column("id")]
    public int Id { get; set; }

    [Column("type")]
    public string Type { get; set; } = Type;

    [Column("zone_id")]
    public string ZoneId { get; set; } = ZoneId;

    [Column("stage_id")]
    public string StageId { get; set; } = StageId;

    [Column("zone_name")]
    public string ZoneName { get; set; } = ZoneName;

    [Column("stage_name")]
    public string StageName { get; set; } = StageName;

    [Column("sanity_cost")]
    public int SanityCost { get; set; } = SanityCost;

    [Column("open_cn")]
    public string? OpenCN { get; set; } = OpenCN;

    [Column("close_cn")]
    public string? CloseCN { get; set; } = CloseCN;

    [Column("exist_cn")]
    public bool ExistCN { get; set; } = ExistCN;

    [Column("open_global")]
    public string? OpenGlobal { get; set; } = OpenGlobal;

    [Column("close_global")]
    public string? CloseGlobal { get; set; } = CloseGlobal;
    
    [Column("exist_global")]
    public bool ExistGlobal { get; set; } = ExistGlobal;
}

using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

[Table("ark_stage")]
public record ArkStage(string ZoneId, string StageId, string ZoneName, string StageName, int SanityCost)
{
    [Column("id")]
    public int Id { get; set; }

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
}

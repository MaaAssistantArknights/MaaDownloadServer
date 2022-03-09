using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

[Table("ark_penguin_zone")]
public sealed record ArkPenguinZone
{
    /// <summary>
    /// 区域 ID
    /// </summary>
    [Key]
    [Column("zone_id")]
    public string ZoneId { get; set; }

    /// <summary>
    /// 区域名
    /// </summary>
    [Column("zone_name")]
    public string ZoneName { get; set; }

    /// <summary>
    /// 区域类型
    /// </summary>
    [Column("zone_type")]
    public string ZoneType { get; set; }

    /// <summary>
    /// 从🐧物流获取到的 Background 字段
    /// </summary>
    [Column("background")]
    public string Background { get; set; }

    /// <summary>
    /// 缓存中的 Background 文件名
    /// </summary>
    [Column("background_file_name")]
    public string BackgroundFileName { get; set; }

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
    /// 韩语区域名
    /// </summary>
    [Column("ko_zone_name")]
    public string KoZoneNameI18N { get; set; }

    /// <summary>
    /// 日语区域名
    /// </summary>
    [Column("ja_zone_name")]
    public string JaZoneNameI18N { get; set; }

    /// <summary>
    /// 英语区域名
    /// </summary>
    [Column("en_zone_name")]
    public string EnZoneNameI18N { get; set; }

    /// <summary>
    /// 汉语区域名
    /// </summary>
    [Column("zh_zone_name")]
    public string ZhZoneNameI18N { get; set; }

    /// <summary>
    /// 包含关卡
    /// </summary>
    public List<ArkPenguinStage> Stages { get; set; }

    public bool EqualTo(ArkPenguinZone other)
    {
        if (other is null)
        {
            return false;
        }

        var thisWithNoStages = this with { Stages = null };
        var otherWithNoStages = other with { Stages = null };

        if (thisWithNoStages != otherWithNoStages)
        {
            return false;
        }

        if (Stages.Count != other.Stages.Count)
        {
            return false;
        }

        var equalCount = Stages
            .Count(stage => other.Stages.Any(stage.EqualTo));

        return equalCount == Stages.Count;
    }

    public override string ToString()
    {
        var original = base.ToString();
        if (Stages is null)
        {
            return original + " with Null";
        }

        var strList = Stages.Select(x => x.ToString()).ToList();
        var str = string.Join("; ", strList);

        return original + " with " + str;
    }
}

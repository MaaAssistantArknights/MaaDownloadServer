using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaaDownloadServer.Model.Entities;

/// <summary>
/// 从企鹅物流数据站获取的关卡信息
/// </summary>
[Table("ark_penguin_stage")]
public sealed record ArkPenguinStage
{
    /// <summary>
    /// 关卡 ID
    /// </summary>
    [Key]
    [Column("stage_id")]
    public string StageId { get; set; }

    /// <summary>
    /// 关卡类型
    /// </summary>
    [Column("stage_type")]
    public string StageType { get; set; }

    /// <summary>
    /// 关卡代码
    /// </summary>
    [Column("stage_code")]
    public string StageCode { get; set; }

    /// <summary>
    /// 关卡理智消耗
    /// </summary>
    [Column("stage_ap_cost")]
    public int StageApCost { get; set; }

    /// <summary>
    /// 最短通关时间
    /// </summary>
    [Column("min_clear_time")]
    public long MinClearTime { get; set; }

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
    /// 韩语关卡代码
    /// </summary>
    [Column("ko_stage_code")]
    public string KoStageCodeI18N { get; set; }

    /// <summary>
    /// 日语关卡代码
    /// </summary>
    [Column("ja_stage_code")]
    public string JaStageCodeI18N { get; set; }

    /// <summary>
    /// 英语关卡代码
    /// </summary>
    [Column("en_stage_code")]
    public string EnStageCodeI18N { get; set; }

    /// <summary>
    /// 汉语关卡代码
    /// </summary>
    [Column("zh_stage_code")]
    public string ZhStageCodeI18N { get; set; }

    /// <summary>
    /// 美服开启时间
    /// </summary>
    [Column("us_open_time")]
    public DateTime? UsOpenTime { get; set; }

    /// <summary>
    /// 美服关闭时间
    /// </summary>
    [Column("us_close_time")]
    public DateTime? UsCloseTime { get; set; }

    /// <summary>
    /// 日服开启时间
    /// </summary>
    [Column("jp_open_time")]
    public DateTime? JpOpenTime { get; set; }

    /// <summary>
    /// 日服关闭时间
    /// </summary>
    [Column("jp_close_time")]
    public DateTime? JpCloseTime { get; set; }

    /// <summary>
    /// 韩服开启时间
    /// </summary>
    [Column("kr_open_time")]
    public DateTime? KrOpenTime { get; set; }

    /// <summary>
    /// 韩服关闭时间
    /// </summary>
    [Column("kr_close_time")]
    public DateTime? KrCloseTime { get; set; }

    /// <summary>
    /// 国服开启时间
    /// </summary>
    [Column("cn_open_time")]
    public DateTime? CnOpenTime { get; set; }

    /// <summary>
    /// 国服关闭时间
    /// </summary>
    [Column("cn_close_time")]
    public DateTime? CnCloseTime { get; set; }

    [Column("drop_items")]
    public List<ArkPenguinItem> DropItems { get; set; }

    public bool EqualTo(ArkPenguinStage other)
    {
        if (other is null)
        {
            return false;
        }

        var thisWithNoDropItems = this with { DropItems = null };
        var otherWithNoDropItems = other with { DropItems = null };

        if (thisWithNoDropItems != otherWithNoDropItems)
        {
            return false;
        }

        if (DropItems.Count != other.DropItems.Count)
        {
            return false;
        }

        var equalCount = DropItems
            .Count(item => other.DropItems.Any(item.EqualTo));

        return equalCount == DropItems.Count;
    }
}

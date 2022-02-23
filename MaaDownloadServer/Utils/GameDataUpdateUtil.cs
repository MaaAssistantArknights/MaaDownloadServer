using MaaDownloadServer.Jobs;

namespace MaaDownloadServer.Utils;

internal static class GameDataUpdateUtil
{
    internal static ArkPenguinStage ToArkPenguinStage(this GameDataUpdateJob.ApiPenguinStage apiStage, GameDataUpdateJob.ApiPenguinZone apiZone)
    {
        return new ArkPenguinStage
        {
            StageType = apiStage.StageType,
            StageId = apiStage.StageId,
            StageCode = apiStage.StageCode,
            StageApCost = apiStage.StageApCost,
            ZoneId = apiZone.ZoneId,
            ZoneName = apiZone.ZoneName,
            ZoneType = apiZone.ZoneType,
            UsExist = apiStage.Existence.Us.Exist,
            JpExist = apiStage.Existence.Jp.Exist,
            KrExist = apiStage.Existence.Kr.Exist,
            CnExist = apiStage.Existence.Cn.Exist,
            KoStageCodeI18N = apiStage.CodeI18N.Korean,
            JaStageCodeI18N = apiStage.CodeI18N.Japanese,
            EnStageCodeI18N = apiStage.CodeI18N.English,
            ZhStageCodeI18N = apiStage.CodeI18N.Chinese,
            KoZoneNameI18N = apiZone.ZoneNameI18N.Korean,
            JaZoneNameI18N = apiZone.ZoneNameI18N.Japanese,
            EnZoneNameI18N = apiZone.ZoneNameI18N.English,
            ZhZoneNameI18N = apiZone.ZoneNameI18N.Chinese,
            UsOpenTime = apiStage.Existence.Us.OpenTime.ToDateTime(GameRegion.America),
            UsCloseTime = apiStage.Existence.Us.CloseTime.ToDateTime(GameRegion.America),
            JpOpenTime = apiStage.Existence.Jp.OpenTime.ToDateTime(GameRegion.Japan),
            JpCloseTime = apiStage.Existence.Jp.CloseTime.ToDateTime(GameRegion.Japan),
            KrOpenTime = apiStage.Existence.Kr.OpenTime.ToDateTime(GameRegion.Korea),
            KrCloseTime = apiStage.Existence.Kr.CloseTime.ToDateTime(GameRegion.Korea),
            CnOpenTime = apiStage.Existence.Cn.OpenTime.ToDateTime(GameRegion.China),
            CnCloseTime = apiStage.Existence.Cn.CloseTime.ToDateTime(GameRegion.China)
        };
    }

    internal static DateTime? ToDateTime(this long? timestamp, GameRegion region)
    {
        if (timestamp is null)
        {
            return null;
        }

        var utcOffset = region switch
        {
            GameRegion.China => 8,
            GameRegion.Korea => 9,
            GameRegion.Japan => 9,
            GameRegion.America => -7,
            _ => throw new ArgumentException("Invalid region")
        };

        var utc = DateTimeOffset.FromUnixTimeMilliseconds((long)timestamp).DateTime;
        var tz = utc + TimeSpan.FromHours(utcOffset);
        return tz;
    }

    public static bool EqualWith<T>(this IEnumerable<T> original, IEnumerable<T> other)
    {
        var originalList = original.ToList();
        var otherList = other.ToList();
        var areEqual = false;
        if (originalList.Count != otherList.Count)
        {
            return false;
        }

        var filteredSequence = otherList.Where(x => originalList.Contains(x));
        if (filteredSequence.Count() == otherList.Count)
        {
            areEqual = true;
        }

        return areEqual;
    }
}

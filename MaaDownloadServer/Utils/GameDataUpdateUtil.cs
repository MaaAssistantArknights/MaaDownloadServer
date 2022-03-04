namespace MaaDownloadServer.Utils;

public static class GameDataUpdateUtil
{
    public static List<ArkPenguinZone> BuildZones(
        IEnumerable<ApiPenguinZone> apiZones,
        IEnumerable<ApiPenguinStage> apiStages)
    {
        var zones = apiZones.Select(x => x.ToArkPenguinZone()).ToList();
        var stages = apiStages.Select(x => x.ToArkPenguinStage()).ToList();

        // Include zone Stages
        for (var i = 0; i < zones.Count; i++)
        {
            var containedStageIds = zones[i].Item2;
            var containedStages = stages
                .Where(x => containedStageIds.Contains(x.StageId))
                .ToList();
            zones[i].Item1.Stages.AddRange(containedStages);
        }

        return zones.Select(x => x.Item1).ToList();
    }

    public static ArkPenguinItem ToArkPenguinItem(this ApiPenguinItem apiItem)
    {
        return new ArkPenguinItem
        {
            ItemId = apiItem.ItemId,
            Name = apiItem.Name,
            SortId = apiItem.SortId,
            Rarity = apiItem.Rarity,
            ItemType = apiItem.ItemType,
            UsExist = apiItem.Existence.Us.Exist,
            JpExist = apiItem.Existence.Jp.Exist,
            KrExist = apiItem.Existence.Kr.Exist,
            CnExist = apiItem.Existence.Cn.Exist,
            ZhNameI18N = apiItem.NameI18N.Chinese,
            EnNameI18N = apiItem.NameI18N.English,
            JpNameI18N = apiItem.NameI18N.Japanese,
            KoNameI18N = apiItem.NameI18N.Korean
        };
    }

    private static ArkPenguinStage ToArkPenguinStage(this ApiPenguinStage apiStage)
    {
        var items = new List<string>();
        if (apiStage.DropInfos is not null)
        {
            items = apiStage.DropInfos
                .Where(x => string.IsNullOrEmpty(x.ItemId) is false)
                .Select(x => x.ItemId)
                .ToList();
        }

        var stage = new ArkPenguinStage
        {
            StageId = apiStage.StageId,
            StageType = apiStage.StageType,
            StageCode = apiStage.StageCode,
            StageApCost = apiStage.StageApCost,
            MinClearTime = apiStage.MinClearTime ?? -1,
            UsExist = apiStage.Existence.Us.Exist,
            JpExist = apiStage.Existence.Jp.Exist,
            KrExist = apiStage.Existence.Kr.Exist,
            CnExist = apiStage.Existence.Cn.Exist,
            KoStageCodeI18N = apiStage.CodeI18N.Korean,
            JaStageCodeI18N = apiStage.CodeI18N.Japanese,
            EnStageCodeI18N = apiStage.CodeI18N.English,
            ZhStageCodeI18N = apiStage.CodeI18N.Chinese,
            UsOpenTime = apiStage.Existence.Us.OpenTime.ToDateTime(GameRegion.America),
            UsCloseTime = apiStage.Existence.Us.CloseTime.ToDateTime(GameRegion.America),
            JpOpenTime = apiStage.Existence.Jp.OpenTime.ToDateTime(GameRegion.Japan),
            JpCloseTime = apiStage.Existence.Jp.CloseTime.ToDateTime(GameRegion.Japan),
            KrOpenTime = apiStage.Existence.Kr.OpenTime.ToDateTime(GameRegion.Korea),
            KrCloseTime = apiStage.Existence.Kr.CloseTime.ToDateTime(GameRegion.Korea),
            CnOpenTime = apiStage.Existence.Cn.OpenTime.ToDateTime(GameRegion.China),
            CnCloseTime = apiStage.Existence.Cn.CloseTime.ToDateTime(GameRegion.China),
            DropItemIds = items
        };
        return stage;
    }

    private static (ArkPenguinZone, List<string>) ToArkPenguinZone(this ApiPenguinZone apiZone)
    {
        return (new ArkPenguinZone
        {
            ZoneId = apiZone.ZoneId,
            ZoneName = apiZone.ZoneName,
            ZoneType = apiZone.ZoneType,
            Background = apiZone.Background,
            BackgroundFileName = apiZone.ZoneId + ".jpg",
            UsExist = apiZone.Existence.Us.Exist,
            JpExist = apiZone.Existence.Jp.Exist,
            KrExist = apiZone.Existence.Kr.Exist,
            CnExist = apiZone.Existence.Cn.Exist,
            KoZoneNameI18N = apiZone.ZoneNameI18N.Korean,
            JaZoneNameI18N = apiZone.ZoneNameI18N.Japanese,
            EnZoneNameI18N = apiZone.ZoneNameI18N.English,
            ZhZoneNameI18N = apiZone.ZoneNameI18N.Chinese,
            Stages = new List<ArkPenguinStage>()
        }, apiZone.Stages);
    }

    private static DateTime? ToDateTime(this long? timestamp, GameRegion region)
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

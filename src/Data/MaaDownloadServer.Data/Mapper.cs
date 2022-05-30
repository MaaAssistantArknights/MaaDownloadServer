// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Models.Assets;

namespace MaaDownloadServer.Data;

public static class Mapper
{
    public static Blob MapBlob(this Base.Entities.Assets.Blob source)
    {
        return new Blob(source.Name, source.Sha1, source.Md5, source.Path);
    }

    public static ICollection<Blob> MapBlobs(this IEnumerable<Base.Entities.Assets.Blob> source)
    {
        return source.Select(MapBlob).ToList();
    }

    public static Asset MapAsset(this Base.Entities.Assets.Asset source)
    {
        return new Asset(
            source.Blob.Name,
            source.Blob.Sha1,
            source.Blob.Md5,
            source.Url,
            source.DownloadCount,
            source.IsBundle,
            source.Files.Select(MapBlob).ToList());
    }
}

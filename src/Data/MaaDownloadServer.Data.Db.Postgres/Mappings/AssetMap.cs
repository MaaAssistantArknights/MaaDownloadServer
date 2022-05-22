// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Entities.Assets;
using MaaDownloadServer.Data.Base.Entities.Map;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaaDownloadServer.Data.Db.Postgres.Mappings;

public class AssetMap : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasMany(p => p.Files)
            .WithMany(p => p.Assets)
            .UsingEntity<BlobToAsset>(j =>
            {
                j.HasOne(e => e.Asset)
                    .WithMany()
                    .HasForeignKey(e => e.AssetId);
                j.HasOne(e => e.Blob)
                    .WithMany()
                    .HasForeignKey(e => e.BlobId);
                j.HasQueryFilter(e => !e.IsDeleted);
                j.ToTable("Map_File_Asset");
            });

        builder.HasOne(p => p.Blob);
    }
}

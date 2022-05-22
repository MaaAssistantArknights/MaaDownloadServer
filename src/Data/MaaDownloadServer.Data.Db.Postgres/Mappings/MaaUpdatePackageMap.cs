// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Entities.Map;
using MaaDownloadServer.Data.Base.Entities.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaaDownloadServer.Data.Db.Postgres.Mappings;

public class MaaUpdatePackageMap : IEntityTypeConfiguration<MaaUpdatePackage>
{
    public void Configure(EntityTypeBuilder<MaaUpdatePackage> builder)
    {
        builder.HasMany(p => p.Add)
            .WithMany(p => p.MaaUpAdd)
            .UsingEntity<BlobToMaaUpAdd>(j =>
            {
                j.HasOne(e => e.MaaUpdatePackage)
                    .WithMany()
                    .HasForeignKey(e => e.MaaUpdateId);
                j.HasOne(e => e.Blob)
                    .WithMany()
                    .HasForeignKey(e => e.BlobId);
                j.HasQueryFilter(e => !e.IsDeleted);
                j.ToTable("Map_Blob_MaaUpAdd");
            });

        builder.HasMany(p => p.Remove)
            .WithMany(p => p.MaaUpRemove)
            .UsingEntity<BlobToMaaUpRemove>(j =>
            {
                j.HasOne(e => e.MaaUpdatePackage)
                    .WithMany()
                    .HasForeignKey(e => e.MaaUpdateId);
                j.HasOne(e => e.Blob)
                    .WithMany()
                    .HasForeignKey(e => e.BlobId);
                j.HasQueryFilter(e => !e.IsDeleted);
                j.ToTable("Map_Blob_MaaUpRemove");
            });

        builder.HasMany(p => p.Update)
            .WithMany(p => p.MaaUpUpdate)
            .UsingEntity<BlobToMaaUpUpdate>(j =>
            {
                j.HasOne(e => e.MaaUpdatePackage)
                    .WithMany()
                    .HasForeignKey(e => e.MaaUpdateId);
                j.HasOne(e => e.Blob)
                    .WithMany()
                    .HasForeignKey(e => e.BlobId);
                j.HasQueryFilter(e => !e.IsDeleted);
                j.ToTable("Map_Blob_MaaUpUpdate");
            });
    }
}

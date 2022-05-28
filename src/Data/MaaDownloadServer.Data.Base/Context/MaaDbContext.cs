// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Entities.Assets;
using MaaDownloadServer.Data.Base.Entities.Base;
using MaaDownloadServer.Data.Base.Entities.External;
using MaaDownloadServer.Data.Base.Entities.Modules;
using MaaDownloadServer.Data.Base.Entities.Other;
using MaaDownloadServer.Data.Base.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Data.Base.Context;

public abstract class MaaDbContext : DbContext
{
    public DbSet<Asset> Assets { get; set; } = null!;
    public DbSet<Blob> Blobs { get; set; } = null!;

    public DbSet<ExternalModule> ExternalModules { get; set; } = null!;
    public DbSet<ExternalSyncStatus> ExternalSyncStatus { get; set; } = null!;

    public DbSet<MaaModule> MaaModules { get; set; } = null!;
    public DbSet<MaaPackage> MaaPackages { get; set; } = null!;
    public DbSet<MaaSyncStatus> MaaSyncStatus { get; set; } = null!;
    public DbSet<MaaUpdatePackage> MaaUpdatePackages { get; set; } = null!;
    public DbSet<MaaVersion> MaaVersions { get; set; } = null!;

    public DbSet<Announce> Announces { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyCommonConfigurations();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void OnBeforeSaving()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Deleted)
            .ToList();
        foreach (var entry in entities)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (entry.State)
            {
                case EntityState.Added:
                    ((BaseEntity)entry.Entity).IsDeleted = false;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    ((BaseEntity)entry.Entity).IsDeleted = true;
                    break;
            }
        }
    }
}

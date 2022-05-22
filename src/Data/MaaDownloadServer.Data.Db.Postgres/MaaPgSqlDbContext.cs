// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Context;
using MaaDownloadServer.Data.Db.Postgres.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Data.Db.Postgres;

public class MaaPgSqlDbContext : MaaDbContext
{
    public MaaPgSqlDbContext(DbContextOptions<MaaDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AssetMap());
        modelBuilder.ApplyConfiguration(new MaaUpdatePackageMap());

        base.OnModelCreating(modelBuilder);
    }
}

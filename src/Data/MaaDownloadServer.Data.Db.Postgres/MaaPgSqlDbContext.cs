﻿// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Context;
using MaaDownloadServer.Data.Db.Postgres.Mappings;
using MaaDownloadServer.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MaaDownloadServer.Data.Db.Postgres;

public class MaaPgSqlDbContext : MaaDbContext
{
    private string? _connectionString;

    public MaaPgSqlDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    public MaaPgSqlDbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetValue<string>("Database:Postgres:ConnectionString");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _connectionString ??= Environment.GetEnvironmentVariable("Database_Postgres_ConnectionString");
        var connectionString = _connectionString.NotNull();
        optionsBuilder.UseNpgsql(connectionString);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AssetMap());
        modelBuilder.ApplyConfiguration(new MaaUpdatePackageMap());

        base.OnModelCreating(modelBuilder);
    }
}
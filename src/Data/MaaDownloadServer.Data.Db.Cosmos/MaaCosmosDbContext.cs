// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Context;
using MaaDownloadServer.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MaaDownloadServer.Data.Db.Cosmos;

public class MaaCosmosDbContext : MaaDbContext
{
    private string? _connectionString;
    private string? _databaseName;

    public MaaCosmosDbContext(string connectionString, string databaseName)
    {
        _connectionString = connectionString;
        _databaseName = databaseName;
    }

    public MaaCosmosDbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetValue<string>("Database:Cosmos:ConnectionString");
        _databaseName = configuration.GetValue<string>("Database:Cosmos:DatabaseName");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _connectionString ??= Environment.GetEnvironmentVariable("Database_Cosmos_ConnectionString");
        _databaseName ??= Environment.GetEnvironmentVariable("Database_Cosmos_DatabaseName");
        var connectionString = _connectionString.NotNull();
        var databaseName = _databaseName.NotNull();
        optionsBuilder.UseCosmos(connectionString, databaseName);
        base.OnConfiguring(optionsBuilder);
    }
}

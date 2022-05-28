// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MaaDownloadServer.Data.Db.Cosmos;

public class MaaCosmosDbContext : MaaDbContext
{
    private readonly string _connectionString;
    private readonly string _databaseName;

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
        optionsBuilder.UseCosmos(_connectionString, _databaseName);
        base.OnConfiguring(optionsBuilder);
    }
}

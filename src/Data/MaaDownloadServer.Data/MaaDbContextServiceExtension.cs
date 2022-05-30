// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Core.Domain.Exceptions;
using MaaDownloadServer.Data.Base.Context;
using MaaDownloadServer.Data.Db.Postgres;
using MaaDownloadServer.Shared.Utils.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MaaDownloadServer.Data;

public static class MaaDbContextServiceExtension
{
    public static IServiceCollection AddMaaDbContext(this IServiceCollection serviceCollection, IConfiguration? configuration = null)
    {
        var dbType = (configuration?.GetValue<string>("Database:Type") ??
                      Environment.GetEnvironmentVariable("Database_Type")).NotNull();
        switch (dbType)
        {
            case "Postgres":
                serviceCollection.AddDbContext<MaaDbContext, MaaPgSqlDbContext>();
                break;
            default:
                throw new UnknownDatabaseException(dbType);
        }

        return serviceCollection;
    }
}

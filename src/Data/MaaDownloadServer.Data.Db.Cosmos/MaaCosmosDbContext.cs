// This file is a part of MaaDownloadServer project.
// MaaDownloadServer belongs to the MAA organization.
// Licensed under the AGPL-3.0 license.

using MaaDownloadServer.Data.Base.Context;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Data.Db.Cosmos;

public class MaaCosmosDbContext : MaaDbContext
{
    public MaaCosmosDbContext(DbContextOptions<MaaDbContext> options) : base(options) { }
}

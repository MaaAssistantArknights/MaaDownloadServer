using MaaDownloadServer.Data.Base.Context;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Data.Db.Cosmos;

public class MaaCosmosDbContext : MaaDbContext
{
    public MaaCosmosDbContext(DbContextOptions<MaaDbContext> options) : base(options) { }
}

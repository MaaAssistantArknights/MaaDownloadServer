using MaaDownloadServer.Data.Base.Context;
using MaaDownloadServer.Data.Db.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MaaDownloadServer.Data.Migrations;

public class MaaPgSqlDbContextDesignTimeFactory : IDesignTimeDbContextFactory<MaaPgSqlDbContext>
{
    public MaaPgSqlDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MaaDbContext>();
        optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=maa;User Id=maa_admin;Password=m@@_@dmin_p@ss;");
        return new MaaPgSqlDbContext(optionsBuilder.Options);
    }
}

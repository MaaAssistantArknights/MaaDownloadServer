using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MaaDownloadServer.Database;

public class MaaDownloadServerDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public MaaDownloadServerDbContext(
        DbContextOptions<MaaDownloadServerDbContext> options,
        IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<Package> Packages { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<PublicContent> PublicContents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = $"Data Source={Path.Combine(_configuration["MaaServer:DataDirectories:RootPath"], _configuration["MaaServer:DataDirectories:SubDirectories:Database"], "data.db")};";
        optionsBuilder.UseSqlite(connectionString, builder =>
            builder.MigrationsAssembly("MaaDownloadServer"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region 转换器

        modelBuilder
            .Entity<Package>()
            .Property(x => x.Architecture)
            .HasConversion<EnumToStringConverter<Architecture>>();

        modelBuilder
            .Entity<Package>()
            .Property(x => x.Platform)
            .HasConversion<EnumToStringConverter<Platform>>();

        #endregion

        #region 多对多

        modelBuilder
            .Entity<Package>()
            .HasMany(x => x.Resources);

        modelBuilder
            .Entity<Resource>()
            .HasMany(x => x.Packages);

        #endregion
    }
}

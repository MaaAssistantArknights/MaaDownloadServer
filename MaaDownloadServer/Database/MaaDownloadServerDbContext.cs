using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public DbSet<ArkPenguinZone> ArkPenguinZones { get; set; }
    public DbSet<ArkPenguinStage> ArkPenguinStages { get; set; }
    public DbSet<ArkPenguinItem> ArkPenguinItems { get; set; }
    public DbSet<ArkPrtsItem> ArkPrtsItems { get; set; }
    public DbSet<DatabaseCache> DatabaseCaches { get; set; }

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

        var arkItemCategoryStringListConverter = new ValueConverter<List<string>, string>(
            v => string.Join(";;", v),
            v => v.Split(";;", StringSplitOptions.RemoveEmptyEntries).ToList());
        var arkItemCategoryValueCompare = new ValueComparer<List<string>>(
            (v, k) => v.EqualWith(k),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())));

        modelBuilder.Entity<ArkPrtsItem>()
            .Property(x => x.Category)
            .HasConversion(arkItemCategoryStringListConverter)
            .Metadata
            .SetValueComparer(arkItemCategoryValueCompare);

        #endregion

        #region 多对多

        modelBuilder
            .Entity<Package>()
            .HasMany(x => x.Resources);

        modelBuilder
            .Entity<Resource>()
            .HasMany(x => x.Packages);

        modelBuilder
            .Entity<ArkPenguinZone>()
            .HasMany(x => x.Stages);

        modelBuilder
            .Entity<ArkPenguinStage>()
            .HasMany(x => x.DropItems);

        #endregion
    }
}

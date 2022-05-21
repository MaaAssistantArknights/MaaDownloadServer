using System.Reflection;
using MaaDownloadServer.Data.Base.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace MaaDownloadServer.Data.Base.Mappings;

public static class CommonMap
{
    public static void ApplyCommonConfigurations(this ModelBuilder modelBuilder)
    {
        var method = typeof(CommonMap).GetTypeInfo().DeclaredMethods
            .Single(m => m.Name == nameof(Configure));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.IsSubclassOf(typeof(BaseEntity)))
            {
                method.MakeGenericMethod(entityType.ClrType).Invoke(null, new object?[] { modelBuilder });
            }
        }
    }

    private static void Configure<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>(builder =>
        {
            builder.HasQueryFilter(p => !p.IsDeleted);
            builder.Property(p => p.EntityId).ValueGeneratedNever();
            builder.Property(p => p.CreateAt).ValueGeneratedNever();
        });
    }
}

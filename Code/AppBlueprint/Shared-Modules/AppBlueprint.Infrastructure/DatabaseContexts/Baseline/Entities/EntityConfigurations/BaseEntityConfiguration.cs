using AppBlueprint.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Base entity configuration that applies named query filters for soft delete functionality.
/// This replaces the reflection-based approach with strongly-typed, named filters.
/// </summary>
/// <typeparam name="TEntity">The entity type that implements IEntity</typeparam>
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Apply named soft delete query filter
        // This filter ensures soft-deleted entities are automatically excluded from queries
        builder.HasQueryFilter(e => !e.IsSoftDeleted);

        // Configure common IEntity properties
        ConfigureBaseProperties(builder);
    }

    /// <summary>
    /// Configures the standard properties defined in IEntity interface.
    /// Derived classes can override this to customize base property configuration.
    /// </summary>
    protected virtual void ConfigureBaseProperties(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(40); // Standard length for prefixed ULID (prefix + underscore + 26 char ULID)

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt);

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Add index for soft delete filtering to improve query performance
        builder.HasIndex(e => e.IsSoftDeleted)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_IsSoftDeleted");
    }
}

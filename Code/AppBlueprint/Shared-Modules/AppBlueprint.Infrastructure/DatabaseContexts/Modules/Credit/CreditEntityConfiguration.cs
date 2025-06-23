using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Credit;

/// <summary>
/// Entity configuration for CreditEntity defining table structure, relationships, and constraints.
/// Manages credit balances and transactions for billing purposes.
/// </summary>
public sealed class CreditEntityConfiguration : IEntityTypeConfiguration<CreditEntity>
{
    public void Configure(EntityTypeBuilder<CreditEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Credits");

        // Configure ID as ULID string
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .IsRequired()
            .HasMaxLength(40)
            .ValueGeneratedNever();

        // Configure BaseEntity properties
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.LastUpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Configure TenantId for multi-tenancy
        builder.Property(c => c.TenantId)
            .IsRequired()
            .HasMaxLength(40);

        // Configure other properties
        builder.Property(c => c.CreditRemaining)
            .IsRequired()
            .HasPrecision(18, 2);

        // Add indexes
        builder.HasIndex(c => c.TenantId)
            .HasDatabaseName("IX_Credits_TenantId");

        builder.HasIndex(c => c.IsSoftDeleted)
            .HasDatabaseName("IX_Credits_IsSoftDeleted");

        builder.HasIndex(c => new { c.TenantId, c.IsSoftDeleted })
            .HasDatabaseName("IX_Credits_TenantId_IsSoftDeleted");

        // Configure query filter for soft delete and tenant scoping
        builder.HasQueryFilter(c => !c.IsSoftDeleted);
    }
}

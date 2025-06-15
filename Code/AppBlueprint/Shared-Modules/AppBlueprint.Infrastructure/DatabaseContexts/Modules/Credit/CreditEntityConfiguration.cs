using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Credit.EntityConfigurations;

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
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreditRemaining)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasIndex(e => e.Id)
            .IsUnique();
    }
}

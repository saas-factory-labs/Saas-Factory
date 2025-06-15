using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for AccountEntity defining table structure, relationships, and constraints.
/// Provides proper indexing, constraints, and standardized table naming conventions.
/// </summary>
public sealed class AccountEntityConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Define table name with standardized naming convention
        builder.ToTable("Accounts");

        // Define primary key
        builder.HasKey(e => e.AccountId);

        // Properties with proper constraints and defaults
        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.AccountId)
            .IsUnique()
            .HasDatabaseName("IX_Accounts_AccountId");
        
        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Accounts_IsActive");
            
        // Additional indexes can be added based on AccountEntity properties
        // Foreign key relationships will be configured as AccountEntity structure evolves
    }
}

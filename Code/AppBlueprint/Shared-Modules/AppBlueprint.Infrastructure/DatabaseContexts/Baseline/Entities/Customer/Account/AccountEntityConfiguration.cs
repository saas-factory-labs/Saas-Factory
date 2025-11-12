using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.Account;

/// <summary>
/// Entity configuration for AccountEntity defining table structure, relationships, and constraints.
/// Manages customer account information and billing details.
/// </summary>
public sealed class AccountEntityConfiguration : BaseEntityConfiguration<AccountEntity>
{
    public override void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
// Table Mapping
        builder.ToTable("Accounts");        // Primary Key        
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Role)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.LastUpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(e => e.Email)
            .IsUnique();

        // Handle SensitiveData attribute
        builder.Property(e => e.Email)
            .HasAnnotation("SensitiveData", true);

        // builder.Property(e => e.AdditionalMetadata)
        //     .HasColumnType("jsonb") // Assuming PostgreSQL
        //     .HasDefaultValue("{}");

        // Relationships (Optional)
        // Uncomment if Tenant relationship is used:
        // builder.HasOne(e => e.Tenant)
        //     .WithMany()
        //     .HasForeignKey(e => e.TenantId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}

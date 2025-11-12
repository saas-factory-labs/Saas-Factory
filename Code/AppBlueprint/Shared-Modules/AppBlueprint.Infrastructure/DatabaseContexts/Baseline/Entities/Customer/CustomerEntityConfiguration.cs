using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;

/// <summary>
/// Entity configuration for CustomerEntity defining table structure, relationships, and constraints.
/// </summary>
public sealed class CustomerEntityConfiguration : BaseEntityConfiguration<CustomerEntity>
{
    public override void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
// Table mapping with standardized naming
        builder.ToTable("Customers");

        // Primary key
        builder.HasKey(e => e.Id);

        // Configure ULID ID with proper length for prefixed ULID (prefix + underscore + 26 char ULID)
        builder.Property(e => e.Id)
            .HasMaxLength(40)
            .IsRequired();

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt);

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Properties with validation
        builder.Property(e => e.Type)
            .HasMaxLength(50);

        builder.Property(e => e.VatNumber)
            .HasMaxLength(50);

        builder.Property(e => e.VatId)
            .HasMaxLength(50);

        builder.Property(e => e.Country)
            .HasMaxLength(100);

        builder.Property(e => e.StripeCustomerId)
            .HasMaxLength(100);

        builder.Property(e => e.StripeSubscriptionId)
            .HasMaxLength(100);

        builder.Property(e => e.CurrentlyAtOnboardingFlowStep)
            .IsRequired();

        builder.Property(e => e.OrganizationId)
            .HasMaxLength(40);

        // Enum configuration
        builder.Property(e => e.CustomerType)
            .HasConversion<int>()
            .IsRequired();

        // Relationships
        builder.HasMany(c => c.Tenants)
            .WithOne(t => t.Customer)
            .HasForeignKey("CustomerId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Tenants_Customers_CustomerId");

        builder.HasMany(c => c.ContactPersons)
            .WithOne()
            .HasForeignKey("CustomerId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ContactPersons_Customers_CustomerId");        // Performance indexes with standardized naming
        builder.HasIndex(e => e.Id)
            .IsUnique()
            .HasDatabaseName("IX_Customers_Id");

        builder.HasIndex(e => e.CustomerType)
            .HasDatabaseName("IX_Customers_CustomerType");

        builder.HasIndex(e => e.Type)
            .HasDatabaseName("IX_Customers_Type");

        builder.HasIndex(e => e.StripeCustomerId)
            .IsUnique()
            .HasDatabaseName("IX_Customers_StripeCustomerId");

        builder.HasIndex(e => e.VatNumber)
            .HasDatabaseName("IX_Customers_VatNumber");

        builder.HasIndex(e => e.IsSoftDeleted)
            .HasDatabaseName("IX_Customers_IsSoftDeleted");

        builder.HasIndex(e => e.OrganizationId)
            .HasDatabaseName("IX_Customers_OrganizationId");
    }
}

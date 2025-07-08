using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.PaymentProvider;

/// <summary>
/// Entity configuration for PaymentProviderEntity managing payment service providers.
/// Configures payment providers like Stripe, PayPal, etc. with proper validation and indexing.
/// </summary>
public sealed class PaymentProviderEntityConfiguration : IEntityTypeConfiguration<PaymentProviderEntity>
{
    /// <summary>
    /// Configures the PaymentProviderEntity with table mapping, properties, and indexes.
    /// </summary>
    /// <param name="builder">The entity type builder for PaymentProviderEntity</param>
    /// <exception cref="ArgumentNullException">Thrown when builder is null</exception>
    public void Configure(EntityTypeBuilder<PaymentProviderEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table configuration
        builder.ToTable("PaymentProviders");
        builder.HasKey(e => e.Id);

        // Primary key configuration
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Unique identifier for the payment provider");

        // Payment provider name (required, unique)
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Name of the payment provider (e.g., Stripe, PayPal, Square)");

        // Description (optional)
        builder.Property(e => e.Description)
            .IsRequired(false)
            .HasMaxLength(500)
            .HasComment("Optional description of the payment provider and its capabilities");

        // Active status flag
        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Indicates if this payment provider is currently active and available for use");

        // Audit fields
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Timestamp when the payment provider was created");

        builder.Property(e => e.LastUpdatedAt)
            .IsRequired(false)
            .HasComment("Timestamp when the payment provider was last updated");

        // Indexes for performance
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_PaymentProviders_Name");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_PaymentProviders_IsActive");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_PaymentProviders_CreatedAt");

        // Note: PaymentProviderEntity typically doesn't have direct foreign key relationships 
        // as it serves as a reference/lookup table for other entities that may reference it.
    }
}

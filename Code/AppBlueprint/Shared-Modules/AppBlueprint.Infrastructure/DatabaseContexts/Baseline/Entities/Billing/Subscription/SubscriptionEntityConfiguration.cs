using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;

/// <summary>
/// Entity configuration for SubscriptionEntity managing subscription plans and offerings.
/// Configures subscription data with proper validation, indexing, and audit tracking.
/// </summary>
public sealed class SubscriptionEntityConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
{
    /// <summary>
    /// Configures the SubscriptionEntity with table mapping, properties, relationships, and indexes.
    /// </summary>
    /// <param name="builder">The entity type builder for SubscriptionEntity</param>
    /// <exception cref="ArgumentNullException">Thrown when builder is null</exception>
    public void Configure(EntityTypeBuilder<SubscriptionEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table configuration
        builder.ToTable("Subscriptions");
        builder.HasKey(e => e.Id);

        // Primary key configuration - ULID as string
        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(40)
            .HasComment("Unique identifier for the subscription plan");

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt)
            .IsRequired();

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // ITenantScoped property
        builder.Property(e => e.TenantId)
            .HasMaxLength(40);

        // Properties with string ID validation
        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.UpdatedBy)
            .IsRequired()
            .HasMaxLength(40);

        // Subscription name (required)
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Name of the subscription plan (e.g., Basic, Pro, Enterprise)");

        // Description (optional)
        builder.Property(e => e.Description)
            .IsRequired(false)
            .HasMaxLength(1000)
            .HasComment("Detailed description of the subscription plan features and benefits");

        // Subscription code (required, unique identifier)
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Unique code identifier for the subscription plan");

        // Status (required)
        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Current status of the subscription (Active, Inactive, Discontinued, etc.)");

        // Audit fields
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Timestamp when the subscription was created");



        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .HasComment("User ID who created this subscription");

        builder.Property(e => e.UpdatedBy)
            .IsRequired()
            .HasComment("User ID who last updated this subscription");

        // Unique constraints
        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("IX_Subscriptions_Code");

        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_Subscriptions_Name");

        // Performance indexes
        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Subscriptions_Status");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_Subscriptions_CreatedAt");

        builder.HasIndex(e => e.CreatedBy)
            .HasDatabaseName("IX_Subscriptions_CreatedBy");

        // Note: Relationships to users and customer subscriptions would be configured 
        // in their respective entity configurations to avoid circular dependencies.
    }
}

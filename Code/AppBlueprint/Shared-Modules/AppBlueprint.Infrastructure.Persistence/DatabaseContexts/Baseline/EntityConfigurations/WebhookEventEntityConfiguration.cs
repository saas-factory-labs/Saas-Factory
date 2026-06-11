using AppBlueprint.Domain.Entities.Webhooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.EntityConfigurations;

/// <summary>
/// Entity configuration for WebhookEventEntity.
/// </summary>
public sealed class WebhookEventEntityConfiguration : IEntityTypeConfiguration<WebhookEventEntity>
{
    public void Configure(EntityTypeBuilder<WebhookEventEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table and Primary Key
        builder.ToTable("WebhookEvents");
        builder.HasKey(e => e.Id);

        // Configure ULID ID
        builder.Property(e => e.Id)
            .HasMaxLength(40)
            .IsRequired()
            .HasComment("Unique webhook event identifier with prefix (e.g., whevt_01ABCD...)");

        // Indexes for efficient querying
        builder.HasIndex(e => e.Id)
            .IsUnique()
            .HasDatabaseName("IX_WebhookEvents_Id");

        builder.HasIndex(e => new { e.EventId, e.Source })
            .IsUnique()
            .HasDatabaseName("IX_WebhookEvents_EventId_Source");

        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_WebhookEvents_TenantId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_WebhookEvents_Status");

        builder.HasIndex(e => e.ReceivedAt)
            .HasDatabaseName("IX_WebhookEvents_ReceivedAt");

        // Properties
        builder.Property(e => e.EventId)
            .HasMaxLength(255)
            .IsRequired()
            .HasComment("External event ID from webhook provider (e.g., Stripe event ID)");

        builder.Property(e => e.EventType)
            .HasMaxLength(255)
            .IsRequired()
            .HasComment("Type of webhook event (e.g., payment_intent.succeeded)");

        builder.Property(e => e.Source)
            .HasMaxLength(50)
            .IsRequired()
            .HasComment("Source of webhook (e.g., stripe, paypal)");

        builder.Property(e => e.Payload)
            .HasColumnType("text")
            .IsRequired()
            .HasComment("Raw JSON payload of webhook event");

        builder.Property(e => e.TenantId)
            .HasMaxLength(40)
            .IsRequired()
            .HasDefaultValue(string.Empty)
            .HasComment("Tenant ID if webhook is tenant-scoped");

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired()
            .HasComment("Processing status (Pending, Processed, Failed, etc.)");

        builder.Property(e => e.RetryCount)
            .HasDefaultValue(0)
            .HasComment("Number of retry attempts");

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000)
            .IsRequired(false)
            .HasComment("Error message if processing failed");

        builder.Property(e => e.ReceivedAt)
            .IsRequired()
            .HasComment("Timestamp when event was received");

        builder.Property(e => e.ProcessedAt)
            .IsRequired(false)
            .HasComment("Timestamp when event was successfully processed");
    }
}

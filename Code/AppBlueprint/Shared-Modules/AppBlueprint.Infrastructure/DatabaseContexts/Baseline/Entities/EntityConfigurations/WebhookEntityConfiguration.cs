using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity Framework configuration for WebhookEntity.
/// Configures Uri to string value conversion for database storage.
/// </summary>
public class WebhookEntityConfiguration : IEntityTypeConfiguration<WebhookEntity>
{
    public void Configure(EntityTypeBuilder<WebhookEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Webhooks");

        builder.HasKey(e => e.Id);

        // Configure Uri property to be stored as string in database
        builder.Property(e => e.Url)
            .HasConversion(
                uri => uri.ToString(),
                str => new Uri(str))
            .HasMaxLength(2048)
            .HasComment("Customer-registered outbound endpoint URL");

        builder.Property(e => e.Secret)
            .HasMaxLength(256)
            .HasComment("HMAC-SHA256 signing secret for outbound payloads");

        builder.Property(e => e.Description)
            .HasMaxLength(1024);

        builder.Property(e => e.EventTypes)
            .HasMaxLength(1024)
            .HasDefaultValue(string.Empty)
            .HasComment("Comma-separated event type filter; empty means all events");

        builder.Property(e => e.TenantId)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Tenant this webhook endpoint belongs to");

        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_Webhooks_TenantId");
    }
}

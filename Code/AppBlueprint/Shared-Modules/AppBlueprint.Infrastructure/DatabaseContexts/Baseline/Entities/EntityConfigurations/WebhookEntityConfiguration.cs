using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
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

        // Configure Uri property to be stored as string in database
        builder.Property(e => e.Url)
            .HasConversion(
                uri => uri.ToString(),
                str => new Uri(str))
            .HasMaxLength(2048); // Reasonable URL length limit

        // Configure other string properties with appropriate lengths
        builder.Property(e => e.Secret)
            .HasMaxLength(256);

        builder.Property(e => e.Description)
            .HasMaxLength(1024);
    }
}

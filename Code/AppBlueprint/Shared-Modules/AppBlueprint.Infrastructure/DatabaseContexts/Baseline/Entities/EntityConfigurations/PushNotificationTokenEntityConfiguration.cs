using AppBlueprint.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public sealed class PushNotificationTokenEntityConfiguration : IEntityTypeConfiguration<PushNotificationTokenEntity>
{
    public void Configure(EntityTypeBuilder<PushNotificationTokenEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("PushNotificationTokens");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(e => e.TenantId)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(e => e.Token)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.DeviceType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.DeviceInfo)
            .HasMaxLength(200);

        builder.Property(e => e.IsActive)
            .IsRequired();

        builder.Property(e => e.LastUsedAt);

        // Indexes
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_PushNotificationTokens_TenantId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_PushNotificationTokens_UserId");

        builder.HasIndex(e => e.Token)
            .IsUnique()
            .HasDatabaseName("IX_PushNotificationTokens_Token");

        builder.HasIndex(e => new { e.UserId, e.IsActive })
            .HasDatabaseName("IX_PushNotificationTokens_UserId_IsActive");
    }
}

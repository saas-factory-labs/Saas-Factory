using AppBlueprint.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public sealed class NotificationPreferencesEntityConfiguration : IEntityTypeConfiguration<NotificationPreferencesEntity>
{
    public void Configure(EntityTypeBuilder<NotificationPreferencesEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("NotificationPreferences");
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

        builder.Property(e => e.EmailEnabled)
            .IsRequired();

        builder.Property(e => e.InAppEnabled)
            .IsRequired();

        builder.Property(e => e.PushEnabled)
            .IsRequired();

        builder.Property(e => e.SmsEnabled)
            .IsRequired();

        builder.Property(e => e.QuietHoursStart);
        builder.Property(e => e.QuietHoursEnd);

        // Indexes
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_NotificationPreferences_TenantId");

        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("IX_NotificationPreferences_UserId");
    }
}

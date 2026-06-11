using AppBlueprint.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public sealed class UserNotificationEntityConfiguration : IEntityTypeConfiguration<UserNotificationEntity>
{
    public void Configure(EntityTypeBuilder<UserNotificationEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("UserNotifications");
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

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Message)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.ActionUrl)
            .HasMaxLength(500)
            .HasConversion(
                v => v != null ? v.ToString() : null,
                v => !string.IsNullOrWhiteSpace(v) ? new Uri(v) : null);

        builder.Property(e => e.IsRead)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.ReadAt);

        // Indexes
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_UserNotifications_TenantId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserNotifications_UserId");

        builder.HasIndex(e => new { e.UserId, e.IsRead })
            .HasDatabaseName("IX_UserNotifications_UserId_IsRead");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_UserNotifications_CreatedAt");
    }
}

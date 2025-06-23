using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for NotificationEntity defining table structure, relationships, and constraints.
/// </summary>
public sealed class NotificationEntityConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("Notifications");

        // Primary key - ULID as string
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(40);

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt)
            .IsRequired();

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Properties with validation
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.OwnerId)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.OwnerId)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        // Relationships
        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Notifications_Users_UserId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_Notifications_UserId");

        builder.HasIndex(e => e.OwnerId)
            .HasDatabaseName("IX_Notifications_OwnerId");

        builder.HasIndex(e => e.IsRead)
            .HasDatabaseName("IX_Notifications_IsRead");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_Notifications_CreatedAt");
    }
}

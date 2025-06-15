using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.PasswordReset;

/// <summary>
/// Entity configuration for PasswordResetEntity defining table structure, relationships, and constraints.
/// Configures the User authentication flow for password reset functionality.
/// </summary>
public sealed class PasswordResetEntityConfiguration : IEntityTypeConfiguration<PasswordResetEntity>
{
    public void Configure(EntityTypeBuilder<PasswordResetEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("PasswordResets");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with proper validation
        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(500); // Increase for secure tokens

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.ExpireAt)
            .IsRequired();

        builder.Property(e => e.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        // User relationship for authentication flow
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PasswordResets_Users_UserId");

        // Performance indexes
        builder.HasIndex(e => e.Token)
            .IsUnique()
            .HasDatabaseName("IX_PasswordResets_Token");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_PasswordResets_UserId");

        builder.HasIndex(e => e.ExpireAt)
            .HasDatabaseName("IX_PasswordResets_ExpireAt");
    }
}

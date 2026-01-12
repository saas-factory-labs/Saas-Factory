using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.EmailVerification;

/// <summary>
/// Entity configuration for EmailVerificationEntity defining table structure, relationships, and constraints.
/// Configures the User authentication flow for email verification functionality.
/// </summary>
public sealed class EmailVerificationEntityConfiguration : IEntityTypeConfiguration<EmailVerificationEntity>
{
    public void Configure(EntityTypeBuilder<EmailVerificationEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("EmailVerifications");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with proper validation
        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(500)
            .HasAnnotation("SensitiveData", true);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.ExpireAt)
            .IsRequired();

        builder.Property(e => e.HasBeenOpened)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.HasBeenVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.UserId)
            .HasMaxLength(450)
            .IsRequired(false);

        // User relationship configuration
        builder.HasOne(e => e.User)
            .WithMany(u => u.EmailVerifications)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_EmailVerifications_Users_UserId");

        // Performance indexes
        builder.HasIndex(e => e.Token)
            .IsUnique()
            .HasDatabaseName("IX_EmailVerifications_Token");

        builder.HasIndex(e => e.ExpireAt)
            .HasDatabaseName("IX_EmailVerifications_ExpireAt");

        builder.HasIndex(e => e.HasBeenVerified)
            .HasDatabaseName("IX_EmailVerifications_HasBeenVerified");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_EmailVerifications_UserId");
    }
}




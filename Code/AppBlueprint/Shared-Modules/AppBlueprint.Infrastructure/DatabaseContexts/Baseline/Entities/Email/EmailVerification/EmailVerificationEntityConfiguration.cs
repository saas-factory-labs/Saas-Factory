using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailVerification;

/// <summary>
/// Entity configuration for EmailVerificationEntity defining table structure, relationships, and constraints.
/// Manages email verification tokens and validation process.
/// </summary>
public sealed class EmailVerificationEntityConfiguration : BaseEntityConfiguration<EmailVerificationEntity>
{
    public override void Configure(EntityTypeBuilder<EmailVerificationEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
builder.ToTable("EmailVerifications");

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

        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(e => e.ExpireAt)
            .IsRequired();

        builder.Property(e => e.HasBeenOpened)
            .IsRequired();

        builder.Property(e => e.HasBeenVerified)
            .IsRequired();

        builder.Property(e => e.UserEntityId)
            .HasMaxLength(40);        // Relationships
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserEntityId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_EmailVerifications_Users_UserEntityId");

        // Indexes for BaseEntity properties
        builder.HasIndex(e => e.IsSoftDeleted)
            .HasDatabaseName("IX_EmailVerifications_IsSoftDeleted");

        // Performance indexes
        builder.HasIndex(e => e.Token)
            .HasDatabaseName("IX_EmailVerifications_Token");

        builder.HasIndex(e => e.UserEntityId)
            .HasDatabaseName("IX_EmailVerifications_UserEntityId");
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailVerification;

/// <summary>
/// Entity configuration for EmailVerificationEntity defining table structure, relationships, and constraints.
/// Manages email verification tokens and validation process.
/// </summary>
public sealed class EmailVerificationEntityConfiguration : IEntityTypeConfiguration<EmailVerificationEntity>
{
    public void Configure(EntityTypeBuilder<EmailVerificationEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("EmailVerifications");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(e => e.ExpireAt)
            .IsRequired();

        builder.Property(e => e.HasBeenOpened)
            .IsRequired();

        builder.Property(e => e.HasBeenVerified)
            .IsRequired();

        // builder.HasOne(e => e.User)
        //     .WithMany(u => u.EmailVerifications)
        //     .HasForeignKey(e => e.UserEntityId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}

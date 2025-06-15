using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailInvite;

/// <summary>
/// Entity configuration for EmailInviteEntity defining table structure, relationships, and constraints.
/// Manages email invitation system and token validation.
/// </summary>
public sealed class EmailInviteEntityConfiguration : IEntityTypeConfiguration<EmailInviteEntity>
{
    public void Configure(EntityTypeBuilder<EmailInviteEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("EmailInvites");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(e => e.ReferredEmailAddress)
            .IsRequired()
            .HasMaxLength(255)
            .HasAnnotation("SensitiveData", true);

        builder.Property(e => e.ExpireAt)
            .IsRequired();

        builder.Property(e => e.InviteIsUsed)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithMany(u => u.ReferralInvitations)
            .HasForeignKey(e => e.UserEntityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

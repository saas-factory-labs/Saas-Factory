using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailInvite;

public class EmailInviteEntityConfiguration : IEntityTypeConfiguration<EmailInviteEntity>
{
    public void Configure(EntityTypeBuilder<EmailInviteEntity> builder)
    {
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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.Profile;

/// <summary>
/// Entity configuration for ProfileEntity defining table structure, relationships, and constraints.
/// Manages user profile information and personal data.
/// </summary>
public sealed class ProfileEntityConfiguration : IEntityTypeConfiguration<ProfileEntity>
{
    public void Configure(EntityTypeBuilder<ProfileEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Profiles");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(50)
            .HasAnnotation("SensitiveData", true);

        builder.Property(e => e.Bio)
            .HasMaxLength(1000);

        builder.Property(e => e.AvatarUrl)
            .HasMaxLength(2048);

        builder.Property(e => e.WebsiteUrl)
            .HasMaxLength(2048);

        builder.Property(e => e.TimeZone)
            .HasMaxLength(100);

        builder.Property(e => e.Language)
            .HasMaxLength(10);

        builder.Property(e => e.Country)
            .HasMaxLength(100);

        builder.HasOne(e => e.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<ProfileEntity>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

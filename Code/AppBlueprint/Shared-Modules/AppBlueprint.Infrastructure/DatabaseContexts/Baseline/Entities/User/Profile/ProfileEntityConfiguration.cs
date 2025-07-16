using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainProfileEntity = AppBlueprint.Domain.Entities.User.ProfileEntity;
using InfraUserEntity = AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.UserEntity;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.Profile;

/// <summary>
/// Entity configuration for ProfileEntity defining table structure, relationships, and constraints.
/// Manages user profile information and personal data.
/// </summary>
public sealed class ProfileEntityConfiguration : IEntityTypeConfiguration<DomainProfileEntity>
{
    public void Configure(EntityTypeBuilder<DomainProfileEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Profiles");

        builder.HasKey(e => e.Id);

        // Configure ULID ID with proper length for prefixed ULID
        builder.Property(e => e.Id)
            .HasMaxLength(40)
            .IsRequired();

        // Configure foreign key property
        builder.Property(e => e.UserId)
            .HasMaxLength(40)
            .IsRequired();

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt);

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(50)
            .HasAnnotation("SensitiveData", true);

        builder.Property(e => e.Bio)
            .HasMaxLength(1000);

        builder.Property(e => e.Avatar)
            .HasMaxLength(2048);

        builder.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<DomainProfileEntity>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Profiles_Users_UserId");

        // Indexes for performance
        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("IX_Profiles_UserId");

        builder.HasIndex(e => e.IsSoftDeleted)
            .HasDatabaseName("IX_Profiles_IsSoftDeleted");
    }
}

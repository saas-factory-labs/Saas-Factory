using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authentication.UserExternalIdentity;

/// <summary>
/// EF Core configuration for UserExternalIdentityEntity.
/// Creates the bridge table between Users and AuthenticationProviders.
/// </summary>
public sealed class UserExternalIdentityEntityConfiguration : IEntityTypeConfiguration<UserExternalIdentityEntity>
{
    public void Configure(EntityTypeBuilder<UserExternalIdentityEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("UserExternalIdentities");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Unique identifier for the external identity link");

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasComment("Internal user ID");

        builder.Property(e => e.AuthenticationProviderId)
            .IsRequired()
            .HasComment("Foreign key to the authentication provider");

        builder.Property(e => e.ExternalUserId)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("User ID from the external auth provider (e.g., Logto sub, Auth0 user_id, Firebase localId)");

        builder.Property(e => e.ExternalEmail)
            .HasMaxLength(320)
            .HasComment("Email from the external provider (may differ from primary email)");

        builder.Property(e => e.ExternalDisplayName)
            .HasMaxLength(200)
            .HasComment("Display name from the external provider");

        builder.Property(e => e.LinkedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("When this provider was linked to the user");

        builder.Property(e => e.LastLoginAt)
            .HasComment("Last login via this provider");

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Whether this identity link is active");

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.AuthenticationProvider)
            .WithMany()
            .HasForeignKey(e => e.AuthenticationProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        // A user can only have one identity per provider
        builder.HasIndex(e => new { e.UserId, e.AuthenticationProviderId })
            .IsUnique()
            .HasDatabaseName("IX_UserExternalIdentities_UserId_ProviderId");

        // Look up a user by their external provider ID
        builder.HasIndex(e => new { e.AuthenticationProviderId, e.ExternalUserId })
            .IsUnique()
            .HasDatabaseName("IX_UserExternalIdentities_ProviderId_ExternalUserId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserExternalIdentities_UserId");
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.UserRole;

/// <summary>
/// Entity configuration for UserRoleEntity defining table structure, relationships, and constraints.
/// </summary>
public sealed class UserRoleEntityConfiguration : BaseEntityConfiguration<UserRoleEntity>
{
    public override void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);        // Define table name
        builder.ToTable("UserRoles");

        // Define primary key - ULID as string
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

        // Properties
        builder.Property(ur => ur.UserId)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(ur => ur.RoleId)
            .IsRequired()
            .HasMaxLength(40);

        // Relationships
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex("UserId");
        builder.HasIndex(ur => ur.RoleId);
        builder.HasIndex(new[] { "UserId", "RoleId" }).IsUnique(); // Prevent duplicate role assignments
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.UserRole;

/// <summary>
/// Entity configuration for UserRoleEntity defining table structure, relationships, and constraints.
/// </summary>
public sealed class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Define table name
        builder.ToTable("UserRoles");

        // Define primary key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(ur => ur.RoleId)
            .IsRequired();

        // Relationships
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey("UserId") // Need to add UserId property to UserRoleEntity
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex("UserId");
        builder.HasIndex(ur => ur.RoleId);
        builder.HasIndex(new [] { "UserId", "RoleId" }).IsUnique(); // Prevent duplicate role assignments
    }
}

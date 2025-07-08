using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Users");

        // Primary key and indexes with standardized naming
        builder.HasKey(u => u.Id);

        // Configure ULID ID with proper length for prefixed ULID (prefix + underscore + 26 char ULID)
        builder.Property(u => u.Id)
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(u => u.Id)
            .IsUnique()
            .HasDatabaseName("IX_Users_Id");

        // Unique constraint on UserName for performance and data integrity
        builder.HasIndex(u => u.UserName)
            .IsUnique()
            .HasDatabaseName("IX_Users_UserName");        // Email uniqueness constraint (addressing missing email uniqueness from analysis)
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        // Performance index on IsActive for filtering
        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        // ITenantScoped property configuration
        builder.Property(u => u.TenantId)
            .IsRequired()
            .HasMaxLength(40);

        builder.HasIndex(u => u.TenantId)
            .HasDatabaseName("IX_Users_TenantId");        // Property configurations
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.IsActive)
            .IsRequired();

        // BaseEntity properties
        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.LastUpdatedAt);

        builder.Property(u => u.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.LastLogin)
            .IsRequired();

        // Add index for soft delete filtering
        builder.HasIndex(u => u.IsSoftDeleted)
            .HasDatabaseName("IX_Users_IsSoftDeleted");

        // Tenant relationship
        builder.HasOne(u => u.Tenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Users_Tenants_TenantId");

        // Relationships
        builder.HasMany(u => u.EmailAddresses)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_EmailAddresses_Users_UserId");
    }
}

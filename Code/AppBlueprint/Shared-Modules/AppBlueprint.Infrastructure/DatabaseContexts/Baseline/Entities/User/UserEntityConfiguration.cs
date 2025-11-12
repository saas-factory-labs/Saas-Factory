using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public sealed class UserEntityConfiguration : BaseEntityConfiguration<UserEntity>
{
    public override void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Apply base configuration including named soft delete filter
        base.Configure(builder);

        builder.ToTable("Users");

        // Primary key index with standardized naming
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

        builder.Property(u => u.LastLogin)
            .IsRequired();

        // Note: BaseEntity properties (Id, CreatedAt, LastUpdatedAt, IsSoftDeleted)
        // are configured in BaseEntityConfiguration including the soft delete index

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

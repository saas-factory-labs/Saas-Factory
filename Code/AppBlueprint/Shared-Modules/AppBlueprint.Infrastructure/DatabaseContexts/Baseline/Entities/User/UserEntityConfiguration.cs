using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");        // Primary key and indexes with standardized naming
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Id)
            .IsUnique()
            .HasDatabaseName("IX_Users_Id");

        // Unique constraint on UserName for performance and data integrity
        builder.HasIndex(u => u.UserName)
            .IsUnique()
            .HasDatabaseName("IX_Users_UserName");

        // Email uniqueness constraint (addressing missing email uniqueness from analysis)
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        // Performance index on IsActive for filtering
        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        // Properties
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(100); // You had 50 and 100—choose 100 to allow flexibility

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.LastLogin)
            .IsRequired();

        // Relationships
        builder.HasMany(u => u.EmailAddresses)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.Id); // Consider reviewing this FK setup if `Id` is the PK on both sides

        // Optional: Indexes or navigation property configs
        // builder.HasIndex(u => u.UserName).IsUnique();
        // builder.HasOne(u => u.Profile)
        //     .WithOne(p => p.Owner)
        //     .HasForeignKey<ProfileEntity>(p => p.Owner);
    }
}

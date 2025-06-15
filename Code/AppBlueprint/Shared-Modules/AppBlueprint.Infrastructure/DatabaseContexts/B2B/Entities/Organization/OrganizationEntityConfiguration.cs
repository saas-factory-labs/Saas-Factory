using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization;

/// <summary>
/// Entity configuration for B2B OrganizationEntity defining table structure, relationships, and constraints.
/// Supports multi-tenant B2B scenarios with user management and organizational hierarchy.
/// </summary>
public sealed class OrganizationEntityConfiguration : IEntityTypeConfiguration<OrganizationEntity>
{    public void Configure(EntityTypeBuilder<OrganizationEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("Organizations");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with validation and constraints
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.OwnerId)
            .IsRequired();

        // Relationships - Organization to Owner (User)
        builder.HasOne(e => e.Owner)
            .WithMany()
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Organizations_Users_OwnerId");

        // Relationships - Organization to Teams
        builder.HasMany(e => e.Teams)
            .WithOne()
            .HasForeignKey("OrganizationId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Teams_Organizations_OrganizationId");

        // Relationships - Organization to Customers
        builder.HasMany(e => e.Customers)
            .WithOne()
            .HasForeignKey("OrganizationId")
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Customers_Organizations_OrganizationId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Organizations_Name");

        builder.HasIndex(e => e.OwnerId)
            .HasDatabaseName("IX_Organizations_OwnerId");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_Organizations_CreatedAt");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Organizations_IsActive");

        // Unique constraint on organization name for business uniqueness
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("UX_Organizations_Name");
    }
}

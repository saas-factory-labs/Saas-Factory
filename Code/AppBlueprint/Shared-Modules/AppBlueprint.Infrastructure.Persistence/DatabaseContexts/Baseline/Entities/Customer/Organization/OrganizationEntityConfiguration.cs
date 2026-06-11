using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.Organization;

/// <summary>
/// Entity configuration for baseline OrganizationEntity defining table structure, relationships, and constraints.
/// Manages baseline customer organization information.
/// </summary>
public sealed class OrganizationEntityConfiguration : IEntityTypeConfiguration<OrganizationEntity>
{
    public void Configure(EntityTypeBuilder<OrganizationEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table Mapping
        builder.ToTable("Organizations");

        // Primary Key        
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Description)
            .HasMaxLength(1024);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.LastUpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Handle SensitiveData attribute
        builder.Property(e => e.Name)
            .HasAnnotation("SensitiveData", true);

        // Relationships (Optional)
        builder.HasOne(e => e.Owner)
            .WithMany()
            .HasForeignKey("OwnerId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}

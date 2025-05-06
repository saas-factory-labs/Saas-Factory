using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;

public class TenantEntityConfiguration : IEntityTypeConfiguration<TenantEntity>
{
    public void Configure(EntityTypeBuilder<TenantEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table and Primary Key
        builder.ToTable("Tenants");
        builder.HasKey(e => e.Id);

        // builder.HasQueryFilter(x => x.Id == 19158);

        // Indexes
        builder.HasIndex(e => e.Id).IsUnique();

        // Properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.IsActive)
            .IsRequired();

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Phone)
            .IsRequired();

        builder.Property(e => e.Type)
            .HasMaxLength(50);

        builder.Property(e => e.VatNumber)
            .HasMaxLength(50);

        builder.Property(e => e.Country)
            .HasMaxLength(100);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt);

        // Relationships
        builder.HasMany(e => e.ContactPersons)
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Customer)
            .WithOne()
            .HasForeignKey<TenantEntity>("Id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Users)
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Teams)
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.Email).IsUnique();
    }
}

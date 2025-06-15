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

        builder.Property(e => e.LastUpdatedAt);        // Relationships with proper foreign key constraints
        builder.HasMany(e => e.ContactPersons)
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ContactPersons_Tenants_TenantId");

        builder.HasOne(e => e.Customer)
            .WithOne()
            .HasForeignKey<TenantEntity>("CustomerId")
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Tenants_Customers_CustomerId");

        builder.HasMany(e => e.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Users_Tenants_TenantId");

        builder.HasMany(e => e.Teams)
            .WithOne(t => t.Tenant)
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Teams_Tenants_TenantId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Tenants_Name");
            
        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Tenants_Email");
            
        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Tenants_IsActive");
            
        builder.HasIndex(e => e.VatNumber)
            .HasDatabaseName("IX_Tenants_VatNumber");
            
        builder.HasIndex(e => e.Country)
            .HasDatabaseName("IX_Tenants_Country");
    }
}

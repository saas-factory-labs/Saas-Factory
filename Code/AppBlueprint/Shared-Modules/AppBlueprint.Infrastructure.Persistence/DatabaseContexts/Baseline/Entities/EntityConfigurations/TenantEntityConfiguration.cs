using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for TenantEntity supporting both B2C and B2B scenarios.
/// </summary>
public sealed class TenantEntityConfiguration : IEntityTypeConfiguration<TenantEntity>
{
    public void Configure(EntityTypeBuilder<TenantEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table and Primary Key
        builder.ToTable("Tenants");
        builder.HasKey(e => e.Id);

        // Configure ULID ID with proper length for prefixed ULID (prefix + underscore + 26 char ULID)
        builder.Property(e => e.Id)
            .HasMaxLength(40)
            .IsRequired()
            .HasComment("Unique tenant identifier with prefix (e.g., tenant_01ABCD...)");

        // Indexes
        builder.HasIndex(e => e.Id)
            .IsUnique()
            .HasDatabaseName("IX_Tenants_Id");

        // ========================================
        // Core Properties (Both B2C and B2B)
        // ========================================

        builder.Property(e => e.TenantType)
            .IsRequired()
            .HasConversion<int>()
            .HasComment("0 = Personal (B2C), 1 = Organization (B2B)");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Tenant name: Full name for Personal, Company name for Organization");

        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .HasComment("Optional description, typically used for Organization tenants");

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasComment("Whether tenant can access the system");

        builder.Property(e => e.IsPrimary)
            .IsRequired()
            .HasComment("Indicates primary tenant for multi-tenant B2C users");

        builder.Property(e => e.Email)
            .HasMaxLength(100)
            .HasComment("Contact email for the tenant");

        builder.Property(e => e.Phone)
            .HasMaxLength(20)
            .HasComment("Contact phone for the tenant");

        // ========================================
        // B2B-Specific Properties (Nullable for B2C)
        // ========================================

        builder.Property(e => e.VatNumber)
            .HasMaxLength(50)
            .HasComment("VAT/Tax number (B2B only)");

        builder.Property(e => e.Country)
            .HasMaxLength(100)
            .HasComment("Country code (B2B only)");

        // ========================================
        // BaseEntity Properties
        // ========================================

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasComment("Timestamp when tenant was created");

        builder.Property(e => e.LastUpdatedAt)
            .HasComment("Timestamp when tenant was last modified");

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Soft delete flag");

        // ========================================
        // Relationships
        // ========================================

        builder.HasMany(e => e.ContactPersons)
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ContactPersons_Tenants_TenantId");

        builder.HasOne(e => e.Customer)
            .WithMany()
            .HasForeignKey("CustomerId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false)
            .HasConstraintName("FK_Tenants_Customers_CustomerId");

        builder.HasMany(e => e.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Users_Tenants_TenantId");

        // ========================================
        // Performance Indexes
        // ========================================

        builder.HasIndex(e => e.TenantType)
            .HasDatabaseName("IX_Tenants_TenantType");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Tenants_Name");

        builder.HasIndex(e => new { e.Email, e.TenantType })
            .HasDatabaseName("IX_Tenants_Email_TenantType");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_Tenants_IsActive");

        builder.HasIndex(e => e.VatNumber)
            .HasDatabaseName("IX_Tenants_VatNumber");

        builder.HasIndex(e => e.Country)
            .HasDatabaseName("IX_Tenants_Country");

        builder.HasIndex(e => e.IsSoftDeleted)
            .HasDatabaseName("IX_Tenants_IsSoftDeleted");

        // Composite index for common queries
        builder.HasIndex(e => new { e.TenantType, e.IsActive, e.IsSoftDeleted })
            .HasDatabaseName("IX_Tenants_Type_Active_NotDeleted");

        // ========================================
        // Full-Text Search Configuration
        // ========================================

        // Note: SearchVector column exists in database as GENERATED ALWAYS AS computed column
        // with GIN index IX_Tenants_SearchVector, but is NOT mapped in EF Core to avoid
        // tsvector type mapping issues. Full-text search queries use raw SQL via
        // PostgreSqlSearchService to access this column directly.
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;

/// <summary>
/// Entity configuration for EmailAddressEntity defining table structure, relationships, and constraints.
/// </summary>
public sealed class EmailAddressEntityConfiguration : IEntityTypeConfiguration<EmailAddressEntity>
{
    public void Configure(EntityTypeBuilder<EmailAddressEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("EmailAddresses");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with validation and GDPR compliance
        builder.Property(e => e.Address)
            .IsRequired()
            .HasMaxLength(255)
            .HasAnnotation("SensitiveData", true);

        builder.Property(e => e.UserId)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany(u => u.EmailAddresses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_EmailAddresses_Users_UserId");

        builder.HasOne(e => e.Customer)
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_EmailAddresses_Customers_CustomerId");

        builder.HasOne(e => e.Tenant)
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_EmailAddresses_Tenants_TenantId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.Address)
            .HasDatabaseName("IX_EmailAddresses_Address");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_EmailAddresses_UserId");

        builder.HasIndex(e => e.CustomerId)
            .HasDatabaseName("IX_EmailAddresses_CustomerId");

        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_EmailAddresses_TenantId");
    }
}

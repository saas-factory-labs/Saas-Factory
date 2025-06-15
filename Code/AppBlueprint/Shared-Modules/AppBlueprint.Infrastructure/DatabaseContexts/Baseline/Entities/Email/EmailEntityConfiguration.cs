using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email;

/// <summary>
/// Entity configuration for EmailAddressEntity defining email addresses and their relationships to users, customers, and tenants.
/// Supports multiple email addresses per entity with proper validation and indexing.
/// </summary>
public sealed class EmailAddressesEntityConfiguration : IEntityTypeConfiguration<EmailAddressEntity>
{
    public void Configure(EntityTypeBuilder<EmailAddressEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Define table name
        builder.ToTable("EmailAddresses");

        // Define primary key
        builder.HasKey(e => e.Id)
            .HasName("PK_Emails");

        // Configure Id property
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Primary key for email address");

        // Configure Address property with GDPR compliance and validation
        builder.Property(e => e.Address)
            .IsRequired()
            .HasMaxLength(320) // RFC 5321 standard max email length
            .HasComment("Email address following RFC 5321 standards")
            .HasAnnotation("DataClassification", GDPRType.DirectlyIdentifiable);

        // Configure foreign key properties
        builder.Property(e => e.UserId)
            .IsRequired()
            .HasComment("Foreign key to the user who owns this email");

        builder.Property(e => e.CustomerId)
            .IsRequired(false)
            .HasComment("Optional foreign key to associated customer");

        builder.Property(e => e.TenantId)
            .IsRequired(false)
            .HasComment("Optional foreign key to associated tenant");

        // Configure relationship to User (required)
        builder.HasOne(e => e.User)
            .WithMany() // UserEntity doesn't have Emails navigation property
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Emails_Users_UserId");

        // Configure relationship to Customer (optional)
        builder.HasOne(e => e.Customer)
            .WithMany() // CustomerEntity doesn't have Emails navigation property
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Emails_Customers_CustomerId");

        // Configure relationship to ContactPerson
        builder.HasOne(e => e.ContactPerson)
            .WithMany() // ContactPersonEntity doesn't have Emails navigation property
            .HasForeignKey("ContactPersonId") // Shadow property
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Emails_ContactPersons_ContactPersonId");

        // Configure relationship to Tenant (optional)
        builder.HasOne(e => e.Tenant)
            .WithMany() // TenantEntity doesn't have Emails navigation property
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Emails_Tenants_TenantId");

        // Create unique index for email addresses to prevent duplicates
        builder.HasIndex(e => e.Address)
            .IsUnique()
            .HasDatabaseName("IX_Emails_Address_Unique")
            .HasFilter(null);

        // Create indexes for performance
        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_Emails_UserId")
            .HasFilter(null);        builder.HasIndex(e => e.CustomerId)
            .HasDatabaseName("IX_Emails_CustomerId")
            .HasFilter($"{nameof(EmailAddressEntity.CustomerId)} IS NOT NULL");

        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_Emails_TenantId")
            .HasFilter($"{nameof(EmailAddressEntity.TenantId)} IS NOT NULL");

        // Create composite index for common queries
        builder.HasIndex(e => new { e.UserId, e.CustomerId })
            .HasDatabaseName("IX_Emails_UserId_CustomerId")
            .HasFilter(null);
    }
}

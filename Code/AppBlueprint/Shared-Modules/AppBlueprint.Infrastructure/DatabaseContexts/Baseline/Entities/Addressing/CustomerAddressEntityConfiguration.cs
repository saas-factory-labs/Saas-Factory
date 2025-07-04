using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.CustomerAddress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for CustomerAddressEntity defining the mapping between customers and their addresses.
/// Handles the many-to-many relationship through a junction table pattern.
/// </summary>
public sealed class CustomerAddressEntityConfiguration : IEntityTypeConfiguration<CustomerAddressEntity>
{
    private const string CustomerIdProperty = "CustomerId";
    
    public void Configure(EntityTypeBuilder<CustomerAddressEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Define table name - fix typo in original template
        builder.ToTable("CustomerAddresses");

        // Define primary key
        builder.HasKey(e => e.Id)
            .HasName("PK_CustomerAddresses");

        // Configure Id property
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Primary key for customer address relationship");

        // Configure enum property with conversion
        builder.Property(e => e.CustomerAddressTypeEnum)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasComment("Type of customer address (billing, shipping, etc.)");

        // Configure boolean properties
        builder.Property(e => e.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Indicates if this is the primary address for the customer");

        builder.Property(e => e.IsSecondary)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Indicates if this is a secondary address for the customer");

        // Configure relationship to Customer
        builder.HasOne(ca => ca.Customer)
            .WithMany() // CustomerEntity doesn't have CustomerAddresses navigation property
            .HasForeignKey(CustomerIdProperty) // Shadow property for foreign key
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_CustomerAddresses_Customers_CustomerId");

        // Configure relationship to Address
        builder.HasOne(ca => ca.Address)
            .WithMany() // AddressEntity doesn't have CustomerAddresses navigation property
            .HasForeignKey("AddressId") // Shadow property for foreign key
            .OnDelete(DeleteBehavior.Restrict) // Preserve address when customer-address link is removed
            .HasConstraintName("FK_CustomerAddresses_Addresses_AddressId");

        // Create indexes for performance
        builder.HasIndex(CustomerIdProperty)
            .HasDatabaseName("IX_CustomerAddresses_CustomerId")
            .HasFilter(null);

        builder.HasIndex("AddressId")
            .HasDatabaseName("IX_CustomerAddresses_AddressId")
            .HasFilter(null);

        builder.HasIndex(e => e.CustomerAddressTypeEnum)
            .HasDatabaseName("IX_CustomerAddresses_CustomerAddressTypeEnum")
            .HasFilter(null);

        // Ensure only one primary address per customer
        builder.HasIndex(CustomerIdProperty, nameof(CustomerAddressEntity.IsPrimary))
            .HasDatabaseName("IX_CustomerAddresses_CustomerId_IsPrimary")
            .HasFilter($"{nameof(CustomerAddressEntity.IsPrimary)} = 1");

        // Create composite index for common queries
        builder.HasIndex(CustomerIdProperty, nameof(CustomerAddressEntity.CustomerAddressTypeEnum))
            .HasDatabaseName("IX_CustomerAddresses_CustomerId_CustomerAddressTypeEnum")
            .HasFilter(null);
    }
}

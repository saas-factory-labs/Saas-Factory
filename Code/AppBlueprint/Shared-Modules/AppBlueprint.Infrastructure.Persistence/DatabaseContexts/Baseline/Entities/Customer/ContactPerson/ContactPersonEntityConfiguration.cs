using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;

public sealed class ContactPersonEntityConfiguration : IEntityTypeConfiguration<ContactPersonEntity>
{
    public void Configure(EntityTypeBuilder<ContactPersonEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("ContactPersons");

        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.Id).IsUnique();

        builder.HasMany(c => c.PhoneNumbers)
            .WithOne(t => t.ContactPerson)
            .HasForeignKey("ContactPersonId")
            .HasPrincipalKey(c => c.Id);

        builder.HasMany(c => c.EmailAddresses)
            .WithOne(t => t.ContactPerson)
            .HasForeignKey("ContactPersonId")
            .HasPrincipalKey(c => c.Id);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.IsPrimary)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired();
    }
}

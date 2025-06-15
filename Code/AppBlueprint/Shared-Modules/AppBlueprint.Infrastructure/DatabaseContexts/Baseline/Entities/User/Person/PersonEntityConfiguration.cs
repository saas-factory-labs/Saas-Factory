using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.Person;

/// <summary>
/// Entity configuration for PersonEntity defining table structure, relationships, and constraints.
/// Supports shared person information model used across User and Customer contexts.
/// </summary>
public sealed class PersonEntityConfiguration : IEntityTypeConfiguration<PersonEntity>
{
    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("Persons");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with validation
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // Relationships to shared entities
        builder.HasMany(e => e.Addresses)
            .WithOne()
            .HasForeignKey("PersonId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PersonAddresses_Persons_PersonId");

        builder.HasMany(e => e.Emails)
            .WithOne()
            .HasForeignKey("PersonId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PersonEmails_Persons_PersonId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => new { e.FirstName, e.LastName })
            .HasDatabaseName("IX_Persons_FirstName_LastName");

        builder.HasIndex(e => e.FirstName)
            .HasDatabaseName("IX_Persons_FirstName");

        builder.HasIndex(e => e.LastName)
            .HasDatabaseName("IX_Persons_LastName");
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyMember;

public class FamilyMemberEntityConfiguration : IEntityTypeConfiguration<FamilyMemberEntity>
{
    public void Configure(EntityTypeBuilder<FamilyMemberEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("FamilyMembers");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property
        builder.ToTable("FamilyMembers");

        // Primary Key
        builder.HasKey(fm => fm.Id);

        // Properties
        builder.Property(fm => fm.Alias)
            .HasMaxLength(100);

        builder.Property(fm => fm.IsActive)
            .IsRequired();

        // Relationships
        builder.HasOne(fm => fm.User)
            .WithMany()
            .HasForeignKey(fm => fm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fm => fm.Family)
            .WithMany(f => f.FamilyMembers)
            .HasForeignKey(fm => fm.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(fm => fm.Id).IsUnique();

        // Define properties
        builder.Property(e => e)
            .IsRequired() // Example property requirement
            .HasMaxLength(100) // Example max length
            .HasAnnotation("SensitiveData", true); // Handle SensitiveData attribute

        builder.Property(e => e.LastName)
            .IsRequired() // Example property requirement
            .HasMaxLength(100) // Example max length
            .HasAnnotation("SensitiveData", true); // Handle SensitiveData attribute

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.FamilyMemberEntity)
        //        .HasForeignKey(re => re.FamilyMemberEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}

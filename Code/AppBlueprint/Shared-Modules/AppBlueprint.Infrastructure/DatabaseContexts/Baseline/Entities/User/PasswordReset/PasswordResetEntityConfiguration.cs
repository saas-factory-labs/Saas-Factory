using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.PasswordReset;

public class PasswordResetEntityConfiguration : IEntityTypeConfiguration<PasswordResetEntity>
{
    public void Configure(EntityTypeBuilder<PasswordResetEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("PasswordResets");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Token)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.PasswordResetEntity)
        //        .HasForeignKey(re => re.PasswordResetEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class AuthorizationProviderEntityConfiguration : IEntityTypeConfiguration<AuthenticationProviderEntity>
{
    public void Configure(EntityTypeBuilder<AuthenticationProviderEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("AuthorizationProviders");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Name)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.AuthorizationProviderEntity)
        //        .HasForeignKey(re => re.AuthorizationProviderEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}

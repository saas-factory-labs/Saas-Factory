using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authentication.EntityConfigurations;

public class AuthorizationProviderEntityConfiguration : IEntityTypeConfiguration<AuthenticationProviderEntity>
{
    public void Configure(EntityTypeBuilder<AuthenticationProviderEntity> builder)
    {
        builder.ToTable("AuthorizationProviders");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.AuthorizationProviderEntity)
        //        .HasForeignKey(re => re.AuthorizationProviderEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}


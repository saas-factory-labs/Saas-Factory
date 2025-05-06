using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class TenantUserRoleEntityConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("Accounts");

        // Define primary key
        builder.HasKey(e => e.AccountId); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.IsActive) // Example property
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.AccountEntity)
        //        .HasForeignKey(re => re.AccountEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}

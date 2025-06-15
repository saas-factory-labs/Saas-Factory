using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.CustomerOnboarding;

/// <summary>
/// Entity configuration for CustomerOnboardingEntity defining table structure, relationships, and constraints.
/// </summary>
public sealed class CustomerOnboardingEntityConfiguration : IEntityTypeConfiguration<CustomerOnboardingEntity>
{
    public void Configure(EntityTypeBuilder<CustomerOnboardingEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Define table name
        builder.ToTable("CustomerOnboardings");        // Define primary key
        builder.HasKey(e => e.Id);

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.Id)
            .IsUnique()
            .HasDatabaseName("IX_CustomerOnboardings_Id");

        // Self-referencing relationship (if needed based on the CustomerOnboarding property)
        // This seems to be a circular reference that might need to be removed or properly configured
        // For now, commenting out until the entity design is clarified
        
        // Define relationships as needed based on actual entity requirements
        // Example: if there are relationships to Customer or User entities
        // builder.HasOne(e => e.Customer)
        //     .WithMany()
        //     .HasForeignKey(e => e.CustomerId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}

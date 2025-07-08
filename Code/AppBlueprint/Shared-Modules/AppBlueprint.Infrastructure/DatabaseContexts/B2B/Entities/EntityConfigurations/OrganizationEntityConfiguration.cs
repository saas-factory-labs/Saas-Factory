using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.EntityConfigurations;

public sealed class OrganizationEntityConfiguration : IEntityTypeConfiguration<OrganizationEntity>
{
    public void Configure(EntityTypeBuilder<OrganizationEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Organizations"); // Corrected table name
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Description)
            .HasMaxLength(500);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.LastUpdatedAt);

        // Relationships
        // builder.HasMany(o => o.Teams)
        //     .WithOne()
        //     .HasForeignKey(t => t.OrganizationId)
        //     .OnDelete(DeleteBehavior.Cascade);
        //
        // builder.HasOne(o => o.Customer)
        //     .WithOne()
        //     .HasForeignKey<OrganizationEntity>(o => o.Id) // Assuming CustomerEntity uses OrganizationEntity.Id as its key
        //     .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.Name)
            .IsUnique();
    }
}

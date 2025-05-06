using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization;

public class OrganizationEntityConfiguration : IEntityTypeConfiguration<OrganizationEntity>
{
    public void Configure(EntityTypeBuilder<OrganizationEntity> builder)
    {
        // builder.HasKey(e => e.Id);
        // builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        // builder.Property(e => e.CreatedAt).IsRequired();
        // builder.Property(e => e.UpdatedAt).IsRequired(false);

        // builder.HasMany(e => e.Users)
        //     .WithOne(u => u.Organization)
        //     .HasForeignKey(u => u.OrganizationId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}

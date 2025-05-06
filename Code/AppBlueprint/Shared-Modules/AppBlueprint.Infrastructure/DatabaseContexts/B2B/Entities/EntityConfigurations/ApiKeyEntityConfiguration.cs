using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.EntityConfigurations;

public class ApiKeyEntityConfiguration : IEntityTypeConfiguration<ApiKeyEntity>
{
    public void Configure(EntityTypeBuilder<ApiKeyEntity> builder)
    {
        builder.ToTable("ApiKeys");
        builder.HasKey(t => t.Id);
        // builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        // builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Credit.EntityConfigurations;

public class CreditEntityConfiguration : IEntityTypeConfiguration<CreditEntity>
{
    public void Configure(EntityTypeBuilder<CreditEntity> builder)
    {
        builder.ToTable("Credits");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreditRemaining)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasIndex(e => e.Id)
            .IsUnique();
    }
}

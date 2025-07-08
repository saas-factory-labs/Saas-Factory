using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public sealed class AdminEntityConfiguration : IEntityTypeConfiguration<AdminEntity>
{
    public void Configure(EntityTypeBuilder<AdminEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Admins");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasAnnotation("SensitiveData", true);

        builder.Property(e => e.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.Email)
            .IsUnique();
    }
}

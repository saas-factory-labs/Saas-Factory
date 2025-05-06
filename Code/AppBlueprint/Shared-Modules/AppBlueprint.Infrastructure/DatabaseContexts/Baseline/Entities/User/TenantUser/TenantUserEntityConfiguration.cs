using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.TenantUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class TenantUserEntityConfiguration : IEntityTypeConfiguration<TenantUserEntity>
{
    public void Configure(EntityTypeBuilder<TenantUserEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table Mapping
        builder.ToTable("TenantUsers");

        // Primary Key        
        builder.HasKey(e => e.TenantId);

        // Properties
        builder.Property(e => e.UserId)
            .IsRequired();

        // Handle SensitiveData attribute
        builder.Property(e => e.UserId)
            .HasAnnotation("SensitiveData", true);
    }
}
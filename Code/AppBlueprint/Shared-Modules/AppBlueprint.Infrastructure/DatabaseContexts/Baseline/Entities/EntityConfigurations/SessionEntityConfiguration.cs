using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Session.EntityConfigurations;

public sealed class SessionEntityConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Sessions");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.SessionKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.SessionData)
            .IsRequired();

        builder.Property(e => e.ExpireDate)
            .IsRequired();

        builder.HasIndex(e => e.SessionKey)
            .IsUnique();
    }
}

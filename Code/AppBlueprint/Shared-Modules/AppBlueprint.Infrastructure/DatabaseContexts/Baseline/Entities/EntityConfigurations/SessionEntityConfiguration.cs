using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Session.EntityConfigurations;

public sealed class SessionEntityConfiguration : BaseEntityConfiguration<SessionEntity>
{
    public override void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Apply base configuration including named soft delete filter
        base.Configure(builder);

        builder.ToTable("Sessions");

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

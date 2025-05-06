using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyInvite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Family.FamilyInvite;

public class FamilyInviteEntityConfiguration : IEntityTypeConfiguration<FamilyInviteEntity>
{
    public void Configure(EntityTypeBuilder<FamilyInviteEntity> builder)
    {
        // Table Mapping
        builder.ToTable("FamilyInvites");

        // Primary Key
        builder.HasKey(fi => fi.Id);

        // Properties
        builder.Property(fi => fi.ExpireAt)
            .IsRequired();

        builder.Property(fi => fi.IsActive)
            .IsRequired();

        builder.Property(fi => fi.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(fi => fi.Family)
            .WithMany(f => f.FamilyInvites)
            .HasForeignKey(fi => fi.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        // builder.HasOne(fi => fi.User)
        //     .WithMany()
        //     .HasForeignKey(fi => fi.UserId)
        //     .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(fi => fi.Id).IsUnique();
    }
}

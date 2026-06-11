using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family;
using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyInvite;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C;

public partial class B2CdbContext
{
    public DbSet<FamilyEntity> Families { get; set; }
    public DbSet<FamilyInviteEntity> FamilyInvites { get; set; }


    partial void OnModelCreating_Family(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FamilyEntityConfiguration());
        modelBuilder.ApplyConfiguration(new FamilyInviteEntityConfiguration());
    }
}

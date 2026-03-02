using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authentication.AuthenticationProvider;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authentication.UserExternalIdentity;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline;

public partial class BaselineDbContext
{
    public DbSet<AuthenticationProviderEntity> AuthenticationProviders { get; set; } = null!;
    public DbSet<UserExternalIdentityEntity> UserExternalIdentities { get; set; } = null!;

    partial void OnModelCreating_Authentication(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuthorizationProviderEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserExternalIdentityEntityConfiguration());
    }
}

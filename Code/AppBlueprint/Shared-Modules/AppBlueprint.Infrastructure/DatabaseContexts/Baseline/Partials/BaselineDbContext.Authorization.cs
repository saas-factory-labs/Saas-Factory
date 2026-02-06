using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Admin;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.Permission;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.ResourcePermission;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.ResourcePermissionType;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.Role;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.RolePermission;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.UserRole;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Partials;

public partial class BaselineDbContext
{
    public DbSet<AdminEntity> Admins { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<ResourcePermissionEntity> ResourcePermissions { get; set; }
    public DbSet<ResourcePermissionTypeEntity> ResourcePermissionTypes { get; set; }


    // public DbSet<AuthorizationProviderEntity> AuthorizationProviders { get; set; }

    partial void OnModelCreating_Authorization(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AdminEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RolePermissionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ResourcePermissionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ResourcePermissionTypeEntityConfiguration());
    }
}

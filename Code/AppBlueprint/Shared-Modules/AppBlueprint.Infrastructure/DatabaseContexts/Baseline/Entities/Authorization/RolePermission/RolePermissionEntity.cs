using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class RolePermissionEntity
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public RoleEntity Role { get; set; }

    // public Guid? ResourceId { get; set; }

    // public PermissionType Permission { get; set; } // Enum


    // public Resource Resource { get; set; }

    // public TYPE Type { get; set; }

    // public PermissionEntity Permission { get; set; }
}

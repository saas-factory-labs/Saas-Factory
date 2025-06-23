using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class RolePermissionEntity : BaseEntity
{
    public RolePermissionEntity()
    {
        Id = PrefixedUlid.Generate("role_perm");
    }

    public string RoleId { get; set; }
    public RoleEntity Role { get; set; }

    // public Guid? ResourceId { get; set; }

    // public PermissionType Permission { get; set; } // Enum


    // public Resource Resource { get; set; }

    // public TYPE Type { get; set; }

    // public PermissionEntity Permission { get; set; }
}

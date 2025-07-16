using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.ResourcePermissionType;

public class ResourcePermissionTypeEntity : BaseEntity
{
    public ResourcePermissionTypeEntity()
    {
        Id = PrefixedUlid.Generate("resource_permission_type");
        ResourcePermissionId = string.Empty;
    }

    public string ResourcePermissionId { get; set; }
    public required ResourcePermissionEntity ResourcePermission { get; set; }

    // public PermissionEntity Permission { get; set; }
    // public string PermissionId { get; set; }
}

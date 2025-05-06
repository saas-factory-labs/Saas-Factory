namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.ResourcePermissionType;

public class ResourcePermissionTypeEntity
{
    public Guid Id { get; set; }

    public Guid ResourcePermissionId { get; set; }
    public ResourcePermissionEntity ResourcePermission { get; set; }

    // public PermissionEntity Permission { get; set; }
    // public Guid PermissionId { get; set; }
}

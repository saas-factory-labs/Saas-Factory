using AppBlueprint.AdminPortalKernel.Domain;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>Writes audit entries for admin portal actions. A failed audit write fails the operation.</summary>
public interface IAdminAuditWriter
{
    /// <summary>
    /// Persists an audit entry attributed to the signed-in admin.
    /// Throws when the caller is not a DeploymentManagerAdmin or required fields are missing.
    /// </summary>
    Task<AdminAuditEntryEntity> WriteAsync(
        string appSlug,
        string action,
        string reason,
        string? targetType = null,
        string? targetId = null,
        string? tenantId = null,
        string? details = null);
}

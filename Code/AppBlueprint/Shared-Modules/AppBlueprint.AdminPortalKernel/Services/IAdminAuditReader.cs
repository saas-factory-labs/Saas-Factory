using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Domain.Dtos;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>Reads the audit trail for an app's admin portal pages.</summary>
public interface IAdminAuditReader
{
    Task<PagedResult<AdminAuditEntryEntity>> SearchAsync(string appSlug, AuditSearchRequest request);

    /// <summary>Newest entries first, for the dashboard.</summary>
    Task<IReadOnlyList<AdminAuditEntryEntity>> GetRecentAsync(string appSlug, int count);
}

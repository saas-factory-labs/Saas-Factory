using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.AdminPortalKernel.Services;

public sealed class AdminAuditWriter : IAdminAuditWriter
{
    private readonly IDbContextFactory<AdminPortalAuditDbContext> _contextFactory;
    private readonly IAdminPortalUserContext _userContext;

    public AdminAuditWriter(
        IDbContextFactory<AdminPortalAuditDbContext> contextFactory,
        IAdminPortalUserContext userContext)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        ArgumentNullException.ThrowIfNull(userContext);
        _contextFactory = contextFactory;
        _userContext = userContext;
    }

    public async Task<AdminAuditEntryEntity> WriteAsync(
        string appSlug,
        string action,
        string reason,
        string? targetType = null,
        string? targetId = null,
        string? tenantId = null,
        string? details = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appSlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        if (!await _userContext.IsDeploymentManagerAdminAsync())
        {
            throw new UnauthorizedAccessException(
                "Writing admin portal audit entries requires the DeploymentManagerAdmin role.");
        }

        var entry = new AdminAuditEntryEntity
        {
            AppSlug = appSlug,
            AdminUserId = await _userContext.GetUserIdAsync() ?? "unknown",
            AdminEmail = await _userContext.GetEmailAsync() ?? "unknown",
            Action = action,
            TargetType = targetType,
            TargetId = targetId,
            TenantId = tenantId,
            Reason = reason,
            Details = details,
            OccurredAtUtc = DateTime.UtcNow
        };

        AdminPortalAuditDbContext context = await _contextFactory.CreateDbContextAsync();
        await using (context)
        {
            context.AuditEntries.Add(entry);
            await context.SaveChangesAsync();
        }

        return entry;
    }
}

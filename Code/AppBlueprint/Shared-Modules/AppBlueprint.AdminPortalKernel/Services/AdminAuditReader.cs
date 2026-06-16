using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Domain.Dtos;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.AdminPortalKernel.Services;

public sealed class AdminAuditReader : IAdminAuditReader
{
    private readonly IDbContextFactory<AdminPortalAuditDbContext> _contextFactory;
    private readonly IAdminPortalUserContext _userContext;

    public AdminAuditReader(
        IDbContextFactory<AdminPortalAuditDbContext> contextFactory,
        IAdminPortalUserContext userContext)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        ArgumentNullException.ThrowIfNull(userContext);
        _contextFactory = contextFactory;
        _userContext = userContext;
    }

    public async Task<PagedResult<AdminAuditEntryEntity>> SearchAsync(string appSlug, AuditSearchRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appSlug);
        ArgumentNullException.ThrowIfNull(request);
        await EnsureDeploymentManagerAdminAsync();

        int page = Math.Max(1, request.Page);
        int pageSize = Math.Clamp(request.PageSize, 1, 200);

        AdminPortalAuditDbContext context = await _contextFactory.CreateDbContextAsync();
        await using (context)
        {
            IQueryable<AdminAuditEntryEntity> query = context.AuditEntries
                .AsNoTracking()
                .Where(entry => entry.AppSlug == appSlug);

            if (!string.IsNullOrWhiteSpace(request.ActionContains))
            {
                string pattern = $"%{request.ActionContains}%";
                query = query.Where(entry => EF.Functions.ILike(entry.Action, pattern));
            }

            if (request.FromUtc is not null)
            {
                query = query.Where(entry => entry.OccurredAtUtc >= request.FromUtc);
            }

            if (request.ToUtc is not null)
            {
                query = query.Where(entry => entry.OccurredAtUtc <= request.ToUtc);
            }

            int totalCount = await query.CountAsync();
            List<AdminAuditEntryEntity> items = await query
                .OrderByDescending(entry => entry.OccurredAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<AdminAuditEntryEntity>(items, totalCount, page, pageSize);
        }
    }

    public async Task<IReadOnlyList<AdminAuditEntryEntity>> GetRecentAsync(string appSlug, int count)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appSlug);
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        await EnsureDeploymentManagerAdminAsync();

        AdminPortalAuditDbContext context = await _contextFactory.CreateDbContextAsync();
        await using (context)
        {
            return await context.AuditEntries
                .AsNoTracking()
                .Where(entry => entry.AppSlug == appSlug)
                .OrderByDescending(entry => entry.OccurredAtUtc)
                .Take(count)
                .ToListAsync();
        }
    }

    private async Task EnsureDeploymentManagerAdminAsync()
    {
        if (!await _userContext.IsDeploymentManagerAdminAsync())
        {
            throw new UnauthorizedAccessException(
                "Reading admin portal audit entries requires the DeploymentManagerAdmin role.");
        }
    }
}

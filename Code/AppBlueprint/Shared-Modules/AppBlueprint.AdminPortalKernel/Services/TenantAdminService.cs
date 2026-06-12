using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Domain.Dtos;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.AdminPortalKernel.Services;

public sealed class TenantAdminService : ITenantAdminService
{
    private readonly AdminQuerySession _session;

    public TenantAdminService(AdminQuerySession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
    }

    public Task<PagedResult<AdminTenantRecord>> SearchAsync(string slug, string? nameContains, int page, int pageSize)
    {
        int safePage = Math.Max(1, page);
        int safePageSize = Math.Clamp(pageSize, 1, 200);

        return _session.ExecuteReadAsync(slug, "tenants.search", async context =>
        {
            IQueryable<AdminTenantRecord> query = context.Tenants;

            if (!string.IsNullOrWhiteSpace(nameContains))
            {
                string pattern = $"%{nameContains.Replace(@"\", @"\\", StringComparison.Ordinal).Replace("%", @"\%", StringComparison.Ordinal).Replace("_", @"\_", StringComparison.Ordinal)}%";
                query = query.Where(tenant => EF.Functions.ILike(tenant.Name, pattern));
            }

            int totalCount = await query.CountAsync();
            List<AdminTenantRecord> items = await query
                .OrderByDescending(tenant => tenant.CreatedAt)
                .Skip((safePage - 1) * safePageSize)
                .Take(safePageSize)
                .ToListAsync();

            return new PagedResult<AdminTenantRecord>(items, totalCount, safePage, safePageSize);
        });
    }
}

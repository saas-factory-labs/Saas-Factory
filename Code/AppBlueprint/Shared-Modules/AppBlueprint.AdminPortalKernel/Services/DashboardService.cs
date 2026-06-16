using AppBlueprint.AdminPortalKernel.Domain.Dtos;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.AdminPortalKernel.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly AdminQuerySession _session;

    public DashboardService(AdminQuerySession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
    }

    public Task<DashboardStats> GetStatsAsync(string slug)
    {
        return _session.ExecuteReadAsync(slug, "dashboard.stats", async context =>
        {
            DateTime signupCutoffUtc = DateTime.UtcNow.AddDays(-30);

            int totalUsers = await context.Users.CountAsync();
            int activeUsers = await context.Users.CountAsync(user => user.IsActive);
            int totalTenants = await context.Tenants.CountAsync();
            int activeTenants = await context.Tenants.CountAsync(tenant => tenant.IsActive);
            int signupsLast30Days = await context.Users.CountAsync(user => user.CreatedAt >= signupCutoffUtc);

            return new DashboardStats(totalUsers, activeUsers, totalTenants, activeTenants, signupsLast30Days);
        });
    }
}

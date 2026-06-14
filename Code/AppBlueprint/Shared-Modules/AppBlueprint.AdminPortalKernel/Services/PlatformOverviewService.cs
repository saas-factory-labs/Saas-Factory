using AppBlueprint.AdminPortalKernel.Billing;
using AppBlueprint.AdminPortalKernel.Modules;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>Platform-wide totals aggregated across every loaded app, for the executive dashboard.</summary>
/// <param name="Errors">One message per app that could not be read, so a single failure never blanks the figures.</param>
public sealed record PlatformOverview(
    int ActiveUsers,
    int ActiveTenants,
    decimal MonthlyRecurringRevenue,
    bool BillingConfigured,
    IReadOnlyList<string> Errors);

/// <summary>Aggregates headline numbers across all active tenant databases.</summary>
public interface IPlatformOverviewService
{
    Task<PlatformOverview> GetAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Sums each loaded module's dashboard stats through the secured <see cref="IDashboardService"/>
/// (so every read passes the <c>AdminQuerySession</c> guard) and reads billing configuration from
/// <see cref="ISaasBillingProvider"/>. MRR stays 0 until a real billing provider is registered.
/// </summary>
public sealed class PlatformOverviewService : IPlatformOverviewService
{
    private readonly AdminPortalModuleRegistry _registry;
    private readonly IDashboardService _dashboard;
    private readonly ISaasBillingProvider _billing;

    public PlatformOverviewService(
        AdminPortalModuleRegistry registry,
        IDashboardService dashboard,
        ISaasBillingProvider billing)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(dashboard);
        ArgumentNullException.ThrowIfNull(billing);
        _registry = registry;
        _dashboard = dashboard;
        _billing = billing;
    }

    public async Task<PlatformOverview> GetAsync(CancellationToken cancellationToken = default)
    {
        int activeUsers = 0;
        int activeTenants = 0;
        var errors = new List<string>();

        foreach (IAdminPortalModule module in _registry.Modules)
        {
            try
            {
                Domain.Dtos.DashboardStats stats = await _dashboard.GetStatsAsync(module.Slug);
                activeUsers += stats.ActiveUsers;
                activeTenants += stats.ActiveTenants;
            }
            catch (UnauthorizedAccessException ex)
            {
                errors.Add($"{module.DisplayName}: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                errors.Add($"{module.DisplayName}: {ex.Message}");
            }
        }

        return new PlatformOverview(activeUsers, activeTenants, 0m, _billing.IsConfigured, errors);
    }
}

using AppBlueprint.AdminPortalKernel.Billing;
using AppBlueprint.AdminPortalKernel.Modules;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>A tenant ("customer") from one app, enriched with its billing status, for the global Customers view.</summary>
public sealed record ConsolidatedCustomer(
    string AppSlug,
    string AppName,
    string TenantId,
    string TenantName,
    string? Email,
    string? Country,
    bool IsActive,
    DateTime CreatedAt,
    bool IsPaying,
    decimal MonthlyRecurringRevenue);

/// <summary>
/// Outcome of consolidating customers across every loaded app. <see cref="Errors"/> holds a message
/// per app that could not be read (so one failing app never blanks the whole view), and
/// <see cref="BillingConfigured"/> tells the UI whether the paying/MRR columns are real or placeholders.
/// </summary>
public sealed record ConsolidatedCustomersResult(
    IReadOnlyList<ConsolidatedCustomer> Customers,
    IReadOnlyList<string> Errors,
    bool BillingConfigured);

/// <summary>Consolidates tenant/customer records across all active app databases plus billing data.</summary>
public interface IConsolidatedCustomerService
{
    Task<ConsolidatedCustomersResult> GetAllAsync(int perAppLimit = 100, CancellationToken cancellationToken = default);
}

/// <summary>
/// Reads each loaded module's tenants through the secured <see cref="ITenantAdminService"/> (so every
/// read passes the <c>AdminQuerySession</c> guard - role, MFA, rate limit, SIEM stream) and merges
/// billing data from <see cref="ISaasBillingProvider"/>.
/// </summary>
public sealed class ConsolidatedCustomerService : IConsolidatedCustomerService
{
    private readonly AdminPortalModuleRegistry _registry;
    private readonly ITenantAdminService _tenants;
    private readonly ISaasBillingProvider _billing;

    public ConsolidatedCustomerService(
        AdminPortalModuleRegistry registry,
        ITenantAdminService tenants,
        ISaasBillingProvider billing)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(tenants);
        ArgumentNullException.ThrowIfNull(billing);
        _registry = registry;
        _tenants = tenants;
        _billing = billing;
    }

    public async Task<ConsolidatedCustomersResult> GetAllAsync(int perAppLimit = 100, CancellationToken cancellationToken = default)
    {
        int safeLimit = Math.Clamp(perAppLimit, 1, 200);
        var customers = new List<ConsolidatedCustomer>();
        var errors = new List<string>();

        foreach (IAdminPortalModule module in _registry.GetModules())
        {
            try
            {
                Domain.Dtos.PagedResult<Domain.AdminTenantRecord> page =
                    await _tenants.SearchAsync(module.Slug, nameContains: null, page: 1, pageSize: safeLimit);

                foreach (Domain.AdminTenantRecord tenant in page.Items)
                {
                    SaasBillingSnapshot billing = await _billing.GetTenantBillingAsync(module.Slug, tenant.Id, cancellationToken);
                    customers.Add(new ConsolidatedCustomer(
                        module.Slug,
                        module.DisplayName,
                        tenant.Id,
                        tenant.Name,
                        tenant.Email,
                        tenant.Country,
                        tenant.IsActive,
                        tenant.CreatedAt,
                        billing.IsPaying,
                        billing.MonthlyRecurringRevenue));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Security gate (role/MFA/nonce) rejected this app - record it, keep consolidating the rest.
                errors.Add($"{module.DisplayName}: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                // Rate limit or app-database failure surfaced by AdminQuerySession.
                errors.Add($"{module.DisplayName}: {ex.Message}");
            }
        }

        return new ConsolidatedCustomersResult(customers, errors, _billing.IsConfigured);
    }
}

namespace AppBlueprint.AdminPortalKernel.Billing;

/// <summary>Billing snapshot for a single tenant, sourced from the SaaS app's payment provider.</summary>
/// <param name="IsPaying">True when the tenant has an active paid subscription.</param>
/// <param name="MonthlyRecurringRevenue">The tenant's MRR contribution in the reporting currency.</param>
public sealed record SaasBillingSnapshot(bool IsPaying, decimal MonthlyRecurringRevenue)
{
    /// <summary>The "no billing data" snapshot returned when no provider is configured.</summary>
    public static SaasBillingSnapshot None { get; } = new(false, 0m);
}

/// <summary>
/// Resolves payment/subscription data for a tenant in a given app (Stripe, etc.). The default
/// <see cref="NotConfiguredSaasBillingProvider"/> returns <see cref="SaasBillingSnapshot.None"/>;
/// a host registers a real provider to light up MRR/paying-customer figures in the consolidated
/// Customers view and per-app SaaS Metrics.
/// </summary>
public interface ISaasBillingProvider
{
    /// <summary>False when no real billing source is wired up (figures are placeholders).</summary>
    bool IsConfigured { get; }

    /// <summary>Returns the billing snapshot for one tenant, or <see cref="SaasBillingSnapshot.None"/> when unknown.</summary>
    Task<SaasBillingSnapshot> GetTenantBillingAsync(string appSlug, string tenantId, CancellationToken cancellationToken = default);
}

/// <summary>Default no-op provider used until a real billing integration (e.g. Stripe) is registered.</summary>
public sealed class NotConfiguredSaasBillingProvider : ISaasBillingProvider
{
    public bool IsConfigured => false;

    public Task<SaasBillingSnapshot> GetTenantBillingAsync(string appSlug, string tenantId, CancellationToken cancellationToken = default) =>
        Task.FromResult(SaasBillingSnapshot.None);
}

namespace AppBlueprint.AdminPortalKernel.Modules;

/// <summary>
/// Live business metrics for a single SaaS app, surfaced on the shell's per-app "SaaS Metrics"
/// page. Revenue-derived figures (MRR, churn) originate from the billing provider; volume
/// figures (signups, paying customers) can be derived from the app database. A module returns
/// <see cref="Empty"/> until it has a metrics source wired up.
/// </summary>
/// <param name="MonthlyRecurringRevenue">Current MRR in the app's reporting currency.</param>
/// <param name="ChurnRatePercent">Trailing churn rate as a percentage (0-100).</param>
/// <param name="Signups">New signups within the reporting window.</param>
/// <param name="PayingCustomers">Count of currently paying customers.</param>
/// <param name="GeneratedAtUtc">When the metrics were computed; <see cref="DateTimeOffset.MinValue"/> means "not generated".</param>
public sealed record SaasAppMetrics(
    decimal MonthlyRecurringRevenue,
    decimal ChurnRatePercent,
    int Signups,
    int PayingCustomers,
    DateTimeOffset GeneratedAtUtc)
{
    /// <summary>Placeholder used before a module supplies real metrics.</summary>
    public static SaasAppMetrics Empty { get; } = new(0m, 0m, 0, 0, DateTimeOffset.MinValue);

    /// <summary>True when these are the placeholder values rather than a computed snapshot.</summary>
    public bool IsEmpty => GeneratedAtUtc == DateTimeOffset.MinValue;
}

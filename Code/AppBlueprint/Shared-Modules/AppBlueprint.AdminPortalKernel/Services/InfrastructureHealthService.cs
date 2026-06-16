using AppBlueprint.AdminPortalKernel.Billing;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>Connectivity state of one core infrastructure dependency.</summary>
public enum InfrastructureStatus
{
    Healthy,
    Degraded,
    NotConfigured,
    Unknown,
}

/// <summary>Health of one infrastructure component shown on the control plane.</summary>
public sealed record InfrastructureComponentHealth(string Name, InfrastructureStatus Status, string Detail);

/// <summary>Aggregate health of the platform's core infrastructure dependencies.</summary>
public sealed record InfrastructureHealth(IReadOnlyList<InfrastructureComponentHealth> Components);

/// <summary>Reports live connectivity/configuration state for Logto, PostgreSQL and Stripe.</summary>
public interface IInfrastructureHealthService
{
    Task<InfrastructureHealth> CheckAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Logto and Stripe report configuration state; PostgreSQL is probed live via the audit
/// DbContext when the audit store is registered. Probes never throw - a failure becomes a
/// Degraded status so the dashboard always renders.
/// </summary>
public sealed class InfrastructureHealthService : IInfrastructureHealthService
{
    private readonly IConfiguration _configuration;
    private readonly ISaasBillingProvider _billing;
    private readonly IServiceProvider _serviceProvider;

    public InfrastructureHealthService(
        IConfiguration configuration,
        ISaasBillingProvider billing,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(billing);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        _configuration = configuration;
        _billing = billing;
        _serviceProvider = serviceProvider;
    }

    public async Task<InfrastructureHealth> CheckAsync(CancellationToken cancellationToken = default)
    {
        var components = new List<InfrastructureComponentHealth>
        {
            CheckLogto(),
            await CheckDatabaseAsync(cancellationToken),
            CheckStripe(),
        };

        return new InfrastructureHealth(components);
    }

    private InfrastructureComponentHealth CheckLogto()
    {
        string? endpoint = _configuration["Logto:Endpoint"];
        return string.IsNullOrWhiteSpace(endpoint)
            ? new InfrastructureComponentHealth("Logto Auth Server", InfrastructureStatus.NotConfigured, "No Logto:Endpoint configured")
            : new InfrastructureComponentHealth("Logto Auth Server", InfrastructureStatus.Healthy, endpoint);
    }

    private async Task<InfrastructureComponentHealth> CheckDatabaseAsync(CancellationToken cancellationToken)
    {
        IDbContextFactory<AdminPortalAuditDbContext>? factory =
            _serviceProvider.GetService<IDbContextFactory<AdminPortalAuditDbContext>>();

        if (factory is null)
        {
            return new InfrastructureComponentHealth("Neon PostgreSQL", InfrastructureStatus.Unknown, "Audit store not registered");
        }

        try
        {
            await using AdminPortalAuditDbContext db = await factory.CreateDbContextAsync(cancellationToken);
            bool canConnect = await db.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? new InfrastructureComponentHealth("Neon PostgreSQL", InfrastructureStatus.Healthy, "Connection OK")
                : new InfrastructureComponentHealth("Neon PostgreSQL", InfrastructureStatus.Degraded, "Cannot connect");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new InfrastructureComponentHealth("Neon PostgreSQL", InfrastructureStatus.Degraded, ex.Message);
        }
    }

    private InfrastructureComponentHealth CheckStripe() =>
        _billing.IsConfigured
            ? new InfrastructureComponentHealth("Stripe Gateway", InfrastructureStatus.Healthy, "Billing provider configured")
            : new InfrastructureComponentHealth("Stripe Gateway", InfrastructureStatus.NotConfigured, "No billing provider configured");
}

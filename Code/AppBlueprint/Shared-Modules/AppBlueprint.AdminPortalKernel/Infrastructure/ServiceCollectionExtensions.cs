using AppBlueprint.AdminPortalKernel.Billing;
using AppBlueprint.AdminPortalKernel.Configuration;
using AppBlueprint.AdminPortalKernel.Modules;
using AppBlueprint.AdminPortalKernel.Security;
using AppBlueprint.AdminPortalKernel.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Infrastructure;

/// <summary>Host registration entry point for the admin portal kernel.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the admin portal kernel: module registry, options bound from the
    /// <c>AdminPortal</c> configuration section, and fail-fast startup validation.
    /// Chain <see cref="AdminPortalBuilder.AddAdminPortalModule{TModule}"/> and
    /// <see cref="AdminPortalBuilder.AddAdminPortalPlugins"/> on the returned builder.
    /// </summary>
    public static AdminPortalBuilder AddAdminPortalKernel(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var registry = new AdminPortalModuleRegistry();
        services.AddSingleton(registry);

        services.AddOptions<AdminPortalOptions>()
            .Bind(configuration.GetSection(AdminPortalOptions.SectionName))
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<AdminPortalOptions>>(new AdminPortalOptionsValidator(registry));

        services.AddSingleton<IAdminPortalDbContextFactory, AdminPortalDbContextFactory>();
        services.AddScoped<IAdminPortalUserContext, BlazorAdminPortalUserContext>();
        services.AddScoped<AdminQuerySession>();
        services.AddScoped<IUserAdminService, UserAdminService>();
        services.AddScoped<ITenantAdminService, TenantAdminService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAdminPortalDiagnostics, AdminPortalDiagnostics>();

        // Cross-app customer consolidation + billing. The billing provider is TryAdd so a host can
        // register a real (e.g. Stripe) provider before calling AddAdminPortalKernel and keep it.
        services.TryAddSingleton<ISaasBillingProvider, NotConfiguredSaasBillingProvider>();
        services.AddScoped<IConsolidatedCustomerService, ConsolidatedCustomerService>();
        services.AddScoped<IPlatformOverviewService, PlatformOverviewService>();
        services.AddScoped<IInfrastructureHealthService, InfrastructureHealthService>();

        AddAdminPortalSecurity(services, configuration);

        return new AdminPortalBuilder(services, registry);
    }

    /// <summary>
    /// Registers the security-hardening pipeline that gates every administrative data access.
    /// The stateful limiters (rate limit, nonce, lockout) are singletons so their in-memory state
    /// survives across scopes; the guard is scoped because it reads the per-circuit user context.
    /// </summary>
    private static void AddAdminPortalSecurity(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AdminPortalSecurityOptions>()
            .Bind(configuration.GetSection(AdminPortalSecurityOptions.SectionName));

        services.TryAddSingleton(TimeProvider.System);
        services.AddHttpClient();

        services.AddSingleton<IAdminNonceService, AdminNonceService>();
        services.AddSingleton<IAdminAccessRateLimiter, AdminAccessRateLimiter>();
        services.AddSingleton<IAdminAccountLockoutService, AdminAccountLockoutService>();
        services.AddSingleton<IDeviceFingerprintService, DeviceFingerprintService>();
        services.AddSingleton<ITicketValidationService, TicketValidationService>();
        services.AddSingleton<IAdminAlertingService, AdminAlertingService>();
        services.AddSingleton<IExternalAuditLogSink, ExternalAuditLogSink>();
        services.AddScoped<IAdminAccessGuard, AdminAccessGuard>();
    }

    /// <summary>
    /// Registers the audit store against DeploymentManager's own database. The
    /// dm_admin_audit table is created by DeploymentManagerDbContext's migrations
    /// (ApiService); the Web host only reads/writes through it.
    /// </summary>
    public static IServiceCollection AddAdminPortalAuditStore(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        // Accept both keyword=value and postgresql:// URI forms (Neon/Railway).
        string normalized = PostgresConnectionString.Normalize(connectionString);
        services.AddDbContextFactory<AdminPortalAuditDbContext>(options => options.UseNpgsql(normalized));
        services.AddScoped<IAdminAuditWriter, AdminAuditWriter>();
        services.AddScoped<IAdminAuditReader, AdminAuditReader>();

        return services;
    }
}

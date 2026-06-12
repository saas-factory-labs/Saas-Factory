using AppBlueprint.AdminPortalKernel.Configuration;
using AppBlueprint.AdminPortalKernel.Modules;
using AppBlueprint.AdminPortalKernel.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        return new AdminPortalBuilder(services, registry);
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

        services.AddDbContextFactory<AdminPortalAuditDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAdminAuditWriter, AdminAuditWriter>();
        services.AddScoped<IAdminAuditReader, AdminAuditReader>();

        return services;
    }
}

using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Application.Services;
using AppBlueprint.Domain.Interfaces.Repositories;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Interceptors;
using AppBlueprint.Infrastructure.Persistence.Repositories;
using AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;
using AppBlueprint.Infrastructure.Persistence.Services;
using AppBlueprint.Infrastructure.Persistence.Services.Webhooks;
using AppBlueprint.Infrastructure.Persistence.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Persistence.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint persistence services
/// (repositories, unit of work and tenant isolation services).
/// </summary>
public static class PersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Registers repository implementations from AppBlueprint.Infrastructure.Persistence.
    /// </summary>
    public static IServiceCollection AddAppBlueprintRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IDataExportRepository, DataExportRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IWebhookRepository, WebhookRepository>();
        services.AddScoped<IWebhookEventRepository, WebhookEventRepository>();
        services.AddScoped<IWebhookDeliveryService, WebhookDeliveryService>();
        services.AddHttpClient("WebhookDelivery");

        return services;
    }

    /// <summary>
    /// Registers tenant-scoped services for multi-tenant isolation and admin access.
    /// </summary>
    public static IServiceCollection AddAppBlueprintTenantServices(this IServiceCollection services)
    {
        // Multi-tenant isolation
        services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();
        services.AddScoped<TenantConnectionInterceptor>();
        services.AddScoped<TenantSecurityInterceptor>();
        services.AddScoped<TenantRlsInterceptor>();

        // User context and admin access
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<IAdminTenantAccessService, AdminTenantAccessService>();

        // Signup database context provider (EF Core)
        services.AddScoped<Application.Interfaces.ISignupDbContextProvider, SignupDbContextProvider>();

        return services;
    }

    /// <summary>
    /// Registers Unit of Work pattern implementation from AppBlueprint.Infrastructure.Persistence.
    /// </summary>
    public static IServiceCollection AddAppBlueprintUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWorkImplementation>();
        return services;
    }
}

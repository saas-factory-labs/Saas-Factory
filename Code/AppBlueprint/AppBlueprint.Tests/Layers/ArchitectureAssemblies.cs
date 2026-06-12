using System.Reflection;
using AppBlueprint.Application.Services;
using AppBlueprint.Domain.Entities.User;
using AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Tests.Layers;

/// <summary>
/// Central anchor for the assemblies covered by the architecture tests.
/// The Infrastructure layer is split into multiple module assemblies, so
/// Infrastructure-wide rules must scan all of them, not just one.
/// </summary>
internal static class ArchitectureAssemblies
{
    public static readonly Assembly Domain = typeof(UserEntity).Assembly;
    public static readonly Assembly Application = typeof(SignupService).Assembly;
    public static readonly Assembly Presentation = typeof(AuthenticationController).Assembly;
    public static readonly Assembly SharedKernel = typeof(PrefixedUlid).Assembly;

    /// <summary>
    /// All Infrastructure assemblies: the facade plus every extracted module.
    /// Anchored on one type per assembly so a missing module fails compilation
    /// rather than silently dropping out of the scan.
    /// </summary>
    public static readonly IReadOnlyList<Assembly> Infrastructure =
    [
        typeof(AppBlueprint.Infrastructure.Extensions.ServiceCollectionExtensions).Assembly,            // facade
        typeof(AppBlueprint.Infrastructure.Authentication.WebAuthenticationExtensions).Assembly,        // Authentication
        typeof(AppBlueprint.Infrastructure.Compliance.PII.PIIEngine).Assembly,                          // Compliance
        typeof(AppBlueprint.Infrastructure.Email.TransactionEmailService).Assembly,                     // Email
        typeof(AppBlueprint.Infrastructure.Notifications.NotificationService).Assembly,                 // Notifications
        typeof(AppBlueprint.Infrastructure.Payments.StripeSubscriptionService).Assembly,                // Payments
        typeof(AppBlueprint.Infrastructure.Persistence.Repositories.UserRepository).Assembly,           // Persistence
        typeof(AppBlueprint.Infrastructure.Realtime.SignalR.NotificationHub).Assembly,                  // Realtime
        typeof(AppBlueprint.Infrastructure.Search.PostgreSqlSearchService<,>).Assembly,                 // Search
        typeof(AppBlueprint.Infrastructure.Storage.R2FileStorageService).Assembly                       // Storage
    ];

    /// <summary>
    /// Assembly names of all Infrastructure assemblies, for dependency rules.
    /// </summary>
    public static string[] InfrastructureNames =>
        Infrastructure.Select(a => a.GetName().Name!).ToArray();
}

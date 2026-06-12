using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.Infrastructure.Services.PII;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint PII detection services.
/// </summary>
public static class PIIServiceCollectionExtensions
{
    /// <summary>
    /// Registers PII detection engine and scanners.
    /// Note: the EF Core PII interceptor lives in AppBlueprint.Infrastructure and is registered separately.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPIIDetection(this IServiceCollection services)
    {
        services.AddScoped<IPIIScanner, RegexPIIScanner>();
        services.AddScoped<IPIIScanner, NerPIIScannerPlaceholder>();
        services.AddScoped<IPIIScanner, LlmPIIScannerPlaceholder>();
        services.AddScoped<IPIIEngine, PIIEngine>();
        services.AddScoped<PIITaggingService>();

        Console.WriteLine("[AppBlueprint.Infrastructure] PII detection services registered (Regex, NER/LLM Placeholders)");

        return services;
    }
}

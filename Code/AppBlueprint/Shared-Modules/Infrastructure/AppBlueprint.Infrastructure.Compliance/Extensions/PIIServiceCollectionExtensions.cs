using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.Infrastructure.Compliance.PII;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Compliance.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint PII detection services.
/// </summary>
public static class PiiServiceCollectionExtensions
{
    /// <summary>
    /// Registers PII detection engine and scanners.
    /// Note: the EF Core PII interceptor lives in AppBlueprint.Infrastructure and is registered separately.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPiiDetection(this IServiceCollection services)
    {
        services.AddScoped<IPiiScanner, RegexPiiScanner>();
        services.AddScoped<IPiiScanner, NerPiiScannerPlaceholder>();
        services.AddScoped<IPiiScanner, LlmPiiScannerPlaceholder>();
        services.AddScoped<IPiiEngine, PiiEngine>();
        services.AddScoped<PiiTaggingService>();

        Console.WriteLine("[AppBlueprint.Infrastructure] PII detection services registered (Regex, NER/LLM Placeholders)");

        return services;
    }
}

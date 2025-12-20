using AppBlueprint.Presentation.ApiModule.Middleware;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Presentation.ApiModule.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint Presentation layer services and configuring middleware.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds AppBlueprint Presentation services including controllers, API versioning, 
    /// CORS, and endpoint configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <example>
    /// <code>
    /// // In Program.cs - after adding Infrastructure and Application
    /// builder.Services.AddAppBlueprintPresentation();
    /// </code>
    /// </example>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintPresentation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        services.AddAntiforgery();
        services.AddHttpContextAccessor();

        // Register global exception handler for consistent error responses and detailed logging
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddCorsPolicy();
        services.ConfigureApiVersioning();

        return services;
    }

    /// <summary>
    /// Configures CORS policy to allow any origin, method, and header.
    /// </summary>
    private static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(policy => policy.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }));

        return services;
    }

    /// <summary>
    /// Configures API versioning with support for URL segments, query strings, and headers.
    /// </summary>
    private static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("apiVersion"),
                    new HeaderApiVersionReader("X-Version")
                );
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    /// <summary>
    /// Configures the AppBlueprint middleware pipeline including HTTPS redirection,
    /// routing, authentication, authorization, and controller mapping.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication ConfigureAppBlueprintMiddleware(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
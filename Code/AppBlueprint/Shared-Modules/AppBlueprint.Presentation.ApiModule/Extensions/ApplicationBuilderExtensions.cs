using AppBlueprint.Presentation.ApiModule.Middleware;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
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
    /// <param name="environment">The web host environment.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <example>
    /// <code>
    /// // In Program.cs - after adding Infrastructure and Application
    /// builder.Services.AddAppBlueprintPresentation(builder.Environment, builder.Configuration);
    /// </code>
    /// </example>
    /// <returns>The service collection for chaining.</returns>
    /// <summary>
    /// Adds AppBlueprint Presentation services with prototype-friendly defaults.
    /// CORS is set to allow any origin (suitable for local development / prototyping).
    /// For production use, call the overload with explicit environment and configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintPresentation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        services.AddAntiforgery();
        services.AddHttpContextAccessor();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Default to open CORS for prototype/development use
        services.AddCors(policy => policy.AddDefaultPolicy(builder =>
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        services.ConfigureApiVersioning();

        return services;
    }

    public static IServiceCollection AddAppBlueprintPresentation(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        services.AddAntiforgery();
        services.AddHttpContextAccessor();

        // Register global exception handler for consistent error responses and detailed logging
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddCorsPolicy(environment, configuration);
        services.ConfigureApiVersioning();

        return services;
    }

    /// <summary>
    /// Configures CORS policy.
    /// In development: allows any origin for testing.
    /// In production: restricts to specific allowed origins from configuration.
    /// </summary>
    private static IServiceCollection AddCorsPolicy(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
    {
        services.AddCors(policy => policy.AddDefaultPolicy(builder =>
        {
            // Early return pattern: Development environment
            if (environment.IsDevelopment())
            {
                // Development only - allow any origin for testing
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                return;
            }

            // Production - restrict to specific origins
            string[] allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? Array.Empty<string>();

            // Guard clause: Validate production configuration
            if (allowedOrigins.Length == 0)
            {
                throw new InvalidOperationException(
                    "CORS allowed origins must be configured in production. Add 'Cors:AllowedOrigins' to appsettings.json");
            }

            builder.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
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

    /// <summary>
    /// Applies AppBlueprint migrations AND your consuming app's DbContext migrations, then
    /// configures the full middleware pipeline. One-call startup for prototype integration.
    /// </summary>
    /// <typeparam name="TDbContext">Your app's DbContext (e.g. DatingDbContext).</typeparam>
    /// <param name="app">The web application.</param>
    /// <returns>A task that completes once migrations are applied and middleware is configured.</returns>
    /// <example>
    /// <code>
    /// var app = builder.Build();
    /// await app.UseAppBlueprintAsync&lt;DatingDbContext&gt;();
    /// app.MapRazorComponents&lt;App&gt;().AddInteractiveServerRenderMode();
    /// app.Run();
    /// </code>
    /// </example>
    public static async Task<WebApplication> UseAppBlueprintAsync<TDbContext>(this WebApplication app)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(app);

        // Apply AppBlueprint's own context migrations (ApplicationDbContext, B2BDbContext, BaselineDbContext)
        await AppBlueprint.Infrastructure.MigrationExtensions.ApplyMigrationsAsync(app);

        // Apply the consuming app's DbContext migrations
        using (IServiceScope scope = app.Services.CreateScope())
        {
            TDbContext? dbContext = scope.ServiceProvider.GetService<TDbContext>();
            if (dbContext is not null)
            {
                await dbContext.Database.MigrateAsync();
            }
        }

        // Configure the standard middleware pipeline
        app.ConfigureAppBlueprintMiddleware();

        return app;
    }

    /// <summary>
    /// Applies AppBlueprint migrations only (no consuming-app DbContext), then configures
    /// the middleware pipeline. Use when your app shares AppBlueprint's DbContext directly.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>A task that completes once migrations are applied and middleware is configured.</returns>
    public static async Task<WebApplication> UseAppBlueprintAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        await AppBlueprint.Infrastructure.MigrationExtensions.ApplyMigrationsAsync(app);
        app.ConfigureAppBlueprintMiddleware();

        return app;
    }
}

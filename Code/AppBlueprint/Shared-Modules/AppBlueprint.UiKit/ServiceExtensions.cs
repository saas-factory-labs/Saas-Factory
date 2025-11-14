using AppBlueprint.UiKit.Configuration;
using AppBlueprint.UiKit.Services;
using AppBlueprint.UiKit.Themes;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;

namespace AppBlueprint.UiKit;

/// <summary>
/// Extension methods for registering AppBlueprint UiKit services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds AppBlueprint UiKit services with default configuration.
    /// Uses the Superhero theme and enables all features.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddMudServices();
    /// builder.Services.AddUiKit();
    /// </code>
    /// </example>
    public static IServiceCollection AddUiKit(this IServiceCollection services)
    {
        return services.AddUiKit(options => { });
    }

    /// <summary>
    /// Adds AppBlueprint UiKit services with custom configuration.
    /// Allows customization of theme, features, and service registrations.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Configuration callback for UiKit options</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddMudServices();
    /// builder.Services.AddUiKit(options =>
    /// {
    ///     // Use custom theme
    ///     options.Theme = new ThemeBuilder()
    ///         .WithPrimaryColor("#1E40AF")
    ///         .Build();
    ///
    ///     // Disable unused features
    ///     options.Features.EnableCharts = false;
    ///
    ///     // Add custom services
    ///     options.ConfigureServices = services =>
    ///     {
    ///         services.AddScoped<IMyService, MyImplementation>();
    ///     };
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddUiKit(
        this IServiceCollection services,
        Action<UiKitOptions> configureOptions)
    {
        // Create and configure options
        var options = new UiKitOptions();
        configureOptions(options);

        // Register the configured theme (use custom or default Superhero theme)
        var theme = options.Theme ?? CustomThemes.Superherotheme;
        services.AddSingleton(theme);

        // Register core UiKit services
        services.AddScoped<NavigationService>();
        services.AddSingleton<BreadcrumbService>();

        // Conditionally register feature-specific services
        if (options.Features.EnableCharts)
        {
            // Chart services would be registered here if needed
            // services.AddScoped<IChartService, ChartService>();
        }

        if (options.Features.EnableThemeManager)
        {
            // Theme manager service registration
            // Already handled by MudBlazor.ThemeManager package
        }

        // Register navigation options for access in components
        services.AddSingleton(options.Navigation);

        // Allow consumers to register additional services
        options.ConfigureServices?.Invoke(services);

        return services;
    }

    /// <summary>
    /// Adds AppBlueprint UiKit with a pre-configured theme builder.
    /// Convenience method for fluent theme configuration.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureTheme">Theme configuration callback</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddUiKitWithTheme(theme => theme
    ///     .WithPrimaryColor("#1E40AF")
    ///     .WithSecondaryColor("#10B981")
    ///     .WithBorderRadius("8px"));
    /// </code>
    /// </example>
    public static IServiceCollection AddUiKitWithTheme(
        this IServiceCollection services,
        Action<ThemeBuilder> configureTheme)
    {
        var builder = new ThemeBuilder();
        configureTheme(builder);
        var theme = builder.Build();

        return services.AddUiKit(options =>
        {
            options.Theme = theme;
        });
    }

    /// <summary>
    /// Adds AppBlueprint UiKit with a preset theme.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="preset">The preset theme to use</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddUiKitWithPreset(ThemePreset.ProfessionalBlue);
    /// </code>
    /// </example>
    public static IServiceCollection AddUiKitWithPreset(
        this IServiceCollection services,
        ThemePreset preset)
    {
        var builder = new ThemeBuilder();
        var theme = preset switch
        {
            ThemePreset.ProfessionalBlue => builder.UseProfessionalBluePreset().Build(),
            ThemePreset.ModernDark => builder.UseModernDarkPreset().Build(),
            ThemePreset.Minimal => builder.UseMinimalPreset().Build(),
            ThemePreset.Superhero => CustomThemes.Superherotheme,
            _ => CustomThemes.Superherotheme
        };

        return services.AddUiKit(options =>
        {
            options.Theme = theme;
        });
    }
}

/// <summary>
/// Pre-defined theme presets for quick configuration.
/// </summary>
public enum ThemePreset
{
    /// <summary>
    /// Default Superhero theme (orange and navy blue).
    /// </summary>
    Superhero,

    /// <summary>
    /// Professional blue theme suitable for business applications.
    /// </summary>
    ProfessionalBlue,

    /// <summary>
    /// Modern dark theme with vibrant purple and pink accents.
    /// </summary>
    ModernDark,

    /// <summary>
    /// Clean, minimal theme with soft colors.
    /// </summary>
    Minimal
}

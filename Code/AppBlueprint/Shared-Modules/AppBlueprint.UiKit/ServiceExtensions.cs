using AppBlueprint.Application.Extensions;
using AppBlueprint.Application.Services.Blog;
using AppBlueprint.UiKit.Configuration;
using AppBlueprint.UiKit.Models.Blog;
using AppBlueprint.UiKit.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.UiKit;

/// <summary>
/// Extension methods for registering AppBlueprint UiKit services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds AppBlueprint UiKit services with default configuration.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddUiKit(this IServiceCollection services)
    {
        // Register core UiKit services
        services.AddScoped<NavigationService>();
        services.AddSingleton<BreadcrumbService>();
        services.AddSingleton<ThemeService>();
        services.AddOptions<AppBlueprintBlogOptions>();
        services.AddAppBlueprintBlogContent();

        return services;
    }

    /// <summary>
    /// Adds the reusable AppBlueprint blog module with default options-backed content.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureBlog">Optional blog configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddUiKitBlog(
        this IServiceCollection services,
        Action<AppBlueprintBlogOptions>? configureBlog = null)
    {
        services.AddUiKit();

        if (configureBlog is not null)
        {
            services.Configure(configureBlog);
        }

        return services;
    }

    /// <summary>
    /// Adds the reusable AppBlueprint blog module with a host-provided content service.
    /// </summary>
    /// <typeparam name="TContentService">The host application's blog content service</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configureBlog">Optional blog configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddUiKitBlog<TContentService>(
        this IServiceCollection services,
        Action<AppBlueprintBlogOptions>? configureBlog = null)
        where TContentService : class, IBlogContentService
    {
        services.AddUiKitBlog(configureBlog);
        services.AddSingleton<IBlogContentService, TContentService>();

        return services;
    }

    /// <summary>
    /// Adds AppBlueprint UiKit services with theme configuration from IConfiguration.
    /// Loads theme settings from appsettings.json section.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration containing theme settings</param>
    /// <param name="sectionName">Configuration section name (default: "Theme")</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddUiKitWithTheme(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Theme")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // Register core services first
        services.AddUiKit();

        // Load and apply theme configuration
        ThemeConfiguration? themeConfig = configuration
            .GetSection(sectionName)
            .Get<ThemeConfiguration>();

        if (themeConfig is not null)
        {
            var serviceProvider = services.BuildServiceProvider();
            ThemeService? themeService = serviceProvider.GetService<ThemeService>();
            themeService?.SetTheme(themeConfig);
        }

        return services;
    }

    /// <summary>
    /// Adds AppBlueprint UiKit with programmatic theme configuration.
    /// Useful for runtime theme customization.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureTheme">Action to configure theme settings</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddUiKitWithTheme(
        this IServiceCollection services,
        Action<ThemeConfiguration> configureTheme)
    {
        ArgumentNullException.ThrowIfNull(configureTheme);

        // Register core services
        services.AddUiKit();

        // Apply programmatic configuration
        var themeConfig = new ThemeConfiguration();
        configureTheme(themeConfig);

        var serviceProvider = services.BuildServiceProvider();
        ThemeService? themeService = serviceProvider.GetService<ThemeService>();
        themeService?.SetTheme(themeConfig);

        return services;
    }
}

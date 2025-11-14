using Microsoft.Extensions.DependencyInjection;
using MudBlazor;

namespace AppBlueprint.UiKit.Configuration;

/// <summary>
/// Configuration options for the AppBlueprint UiKit.
/// Allows consumers to customize theme, services, and enabled features.
/// </summary>
public class UiKitOptions
{
    /// <summary>
    /// Custom MudBlazor theme to use instead of the default Superhero theme.
    /// If null, the default theme will be used.
    /// </summary>
    /// <example>
    /// <code>
    /// options.Theme = new ThemeBuilder()
    ///     .WithPrimaryColor("#1E40AF")
    ///     .Build();
    /// </code>
    /// </example>
    public MudTheme? Theme { get; set; }

    /// <summary>
    /// Optional callback to configure additional services.
    /// Use this to replace default implementations or add custom services.
    /// </summary>
    /// <example>
    /// <code>
    /// options.ConfigureServices = services =>
    /// {
    ///     services.AddScoped&lt;IMyCustomService, MyImplementation&gt;();
    /// };
    /// </code>
    /// </example>
    public Action<IServiceCollection>? ConfigureServices { get; set; }

    /// <summary>
    /// Feature flags to enable/disable specific UiKit components.
    /// Disabling unused features can reduce bundle size.
    /// </summary>
    public UiKitFeatures Features { get; set; } = new();

    /// <summary>
    /// Navigation configuration options.
    /// </summary>
    public NavigationOptions Navigation { get; set; } = new();
}

/// <summary>
/// Feature flags for enabling/disabling specific UiKit functionality.
/// </summary>
public class UiKitFeatures
{
    /// <summary>
    /// Enable chart components (BarChart, LineChart, PieChart, etc.).
    /// Default: true
    /// </summary>
    public bool EnableCharts { get; set; } = true;

    /// <summary>
    /// Enable account settings components and pages.
    /// Default: true
    /// </summary>
    public bool EnableAccountSettings { get; set; } = true;

    /// <summary>
    /// Enable dashboard components and layouts.
    /// Default: true
    /// </summary>
    public bool EnableDashboard { get; set; } = true;

    /// <summary>
    /// Enable theme manager for runtime theme customization.
    /// Default: false (reduces bundle size)
    /// </summary>
    public bool EnableThemeManager { get; set; } = false;
}

/// <summary>
/// Navigation configuration options.
/// </summary>
public class NavigationOptions
{
    /// <summary>
    /// Collapse sidebar by default on mobile devices.
    /// Default: true
    /// </summary>
    public bool CollapseSidebarOnMobile { get; set; } = true;

    /// <summary>
    /// Enable breadcrumb navigation.
    /// Default: true
    /// </summary>
    public bool EnableBreadcrumbs { get; set; } = true;

    /// <summary>
    /// Sidebar width in pixels.
    /// Default: 240
    /// </summary>
    public int SidebarWidth { get; set; } = 240;

    /// <summary>
    /// Mini sidebar width when collapsed (pixels).
    /// Default: 56
    /// </summary>
    public int MiniSidebarWidth { get; set; } = 56;
}

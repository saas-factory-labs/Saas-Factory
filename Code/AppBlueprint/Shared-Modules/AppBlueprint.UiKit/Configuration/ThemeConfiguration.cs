namespace AppBlueprint.UiKit.Configuration;

/// <summary>
/// Configuration for application theming using Tailwind CSS color system.
/// Supports multiple application types from a single codebase.
/// </summary>
public sealed class ThemeConfiguration
{
    /// <summary>
    /// Primary brand color from Tailwind palette.
    /// Valid values: "slate", "gray", "zinc", "neutral", "stone", "red", "orange", 
    /// "amber", "yellow", "lime", "green", "emerald", "teal", "cyan", "sky", "blue", 
    /// "indigo", "violet", "purple", "fuchsia", "pink", "rose"
    /// </summary>
    public string PrimaryColor { get; set; } = "violet";

    /// <summary>
    /// Accent/secondary color from Tailwind palette.
    /// </summary>
    public string AccentColor { get; set; } = "sky";

    /// <summary>
    /// Default shade for primary color (e.g., "500", "600", "700").
    /// Can be overridden per component via parameters.
    /// </summary>
    public string DefaultPrimaryShade { get; set; } = "500";

    /// <summary>
    /// Default shade for accent color (e.g., "500", "600", "700").
    /// Can be overridden per component via parameters.
    /// </summary>
    public string DefaultAccentShade { get; set; } = "500";

    /// <summary>
    /// Application type for content customization.
    /// Supported: "saas", "dating", "crm", "ecommerce"
    /// </summary>
    public string ApplicationType { get; set; } = "saas";

    /// <summary>
    /// Custom text/label overrides for UI elements.
    /// Key format: "category.element" (e.g., "analytics.topProducts")
    /// </summary>
    public Dictionary<string, string> CustomLabels { get; set; } = [];

    /// <summary>
    /// Brand name displayed throughout the application.
    /// </summary>
    public string BrandName { get; set; } = "AppBlueprint";

    /// <summary>
    /// Logo URL (optional).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI properties should not be strings", Justification = "Needs to be string for JSON configuration binding")]
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Icon pack to use. Future: support different icon sets per app type.
    /// </summary>
    public string IconPack { get; set; } = "heroicons";

    /// <summary>
    /// Custom class overrides for specific components.
    /// Key format: "ComponentName.ElementId" (e.g., "AnalyticsCard11.Icon.Accent")
    /// Value: Complete Tailwind classes to replace theme classes.
    /// </summary>
    public Dictionary<string, string> CustomClassOverrides { get; set; } = [];
}

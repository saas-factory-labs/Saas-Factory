using AppBlueprint.UiKit.Configuration;

namespace AppBlueprint.UiKit.Services;

/// <summary>
/// Centralized theme service that generates Tailwind CSS classes dynamically.
/// No custom CSS required - uses Tailwind's built-in color system.
/// </summary>
public sealed class ThemeService
{
    /// <summary>
    /// Gets the current theme configuration.
    /// </summary>
    public ThemeConfiguration CurrentTheme { get; private set; } = new();

    /// <summary>
    /// Event raised when theme changes. Subscribed components should re-render.
    /// </summary>
    public event Action? OnThemeChanged;

    /// <summary>
    /// Updates theme configuration and notifies subscribers.
    /// </summary>
    /// <param name="configuration">New theme configuration</param>
    public void SetTheme(ThemeConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        CurrentTheme = configuration;
        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Gets a Tailwind class for the primary color.
    /// </summary>
    /// <param name="prefix">Tailwind prefix: "bg", "text", "border", "ring", etc.</param>
    /// <param name="shade">Tailwind shade: "50", "100", ... "900", "950". If null, uses DefaultPrimaryShade from config.</param>
    /// <returns>Complete Tailwind class (e.g., "bg-violet-500")</returns>
    public string GetPrimaryClass(string prefix, string? shade = null)
    {
        ArgumentNullException.ThrowIfNull(prefix);
        string resolvedShade = shade ?? CurrentTheme.DefaultPrimaryShade;
        return $"{prefix}-{CurrentTheme.PrimaryColor}-{resolvedShade}";
    }

    /// <summary>
    /// Gets a Tailwind class for the accent color.
    /// </summary>
    /// <param name="prefix">Tailwind prefix: "bg", "text", "border", "ring", etc.</param>
    /// <param name="shade">Tailwind shade: "50", "100", ... "900", "950". If null, uses DefaultAccentShade from config.</param>
    /// <returns>Complete Tailwind class (e.g., "bg-sky-500")</returns>
    public string GetAccentClass(string prefix, string? shade = null)
    {
        ArgumentNullException.ThrowIfNull(prefix);
        string resolvedShade = shade ?? CurrentTheme.DefaultAccentShade;
        return $"{prefix}-{CurrentTheme.AccentColor}-{resolvedShade}";
    }

    /// <summary>
    /// Gets multiple Tailwind classes for primary color (space-separated).
    /// </summary>
    /// <param name="classes">Tuples of (prefix, shade)</param>
    /// <returns>Space-separated Tailwind classes</returns>
    public string GetPrimaryClasses(params (string prefix, string shade)[] classes)
    {
        return string.Join(" ", classes.Select(c => GetPrimaryClass(c.prefix, c.shade)));
    }

    /// <summary>
    /// Gets hover state classes for primary color.
    /// </summary>
    /// <param name="prefix">Tailwind prefix</param>
    /// <param name="shade">Tailwind shade (default: "600" for hover darkening)</param>
    public string GetPrimaryHoverClass(string prefix, string shade = "600")
    {
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentNullException.ThrowIfNull(shade);
        return $"hover:{prefix}-{CurrentTheme.PrimaryColor}-{shade}";
    }

    /// <summary>
    /// Gets focus state classes for primary color.
    /// </summary>
    /// <param name="prefix">Tailwind prefix</param>
    /// <param name="shade">Tailwind shade</param>
    public string GetPrimaryFocusClass(string prefix, string shade = "500")
    {
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentNullException.ThrowIfNull(shade);
        return $"focus:{prefix}-{CurrentTheme.PrimaryColor}-{shade}";
    }

    /// <summary>
    /// Gets dark mode classes for primary color.
    /// </summary>
    /// <param name="prefix">Tailwind prefix</param>
    /// <param name="shade">Tailwind shade (default: "400" for lighter dark mode)</param>
    public string GetPrimaryDarkClass(string prefix, string shade = "400")
    {
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentNullException.ThrowIfNull(shade);
        return $"dark:{prefix}-{CurrentTheme.PrimaryColor}-{shade}";
    }

    /// <summary>
    /// Gets a themed label with fallback.
    /// </summary>
    /// <param name="key">Label key (e.g., "analytics.topProducts")</param>
    /// <param name="defaultValue">Fallback value if key not found</param>
    /// <returns>Configured label or default value</returns>
    public string GetLabel(string key, string defaultValue)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(defaultValue);
        return CurrentTheme.CustomLabels.GetValueOrDefault(key, defaultValue);
    }

    /// <summary>
    /// Gets label by application type with specific variants.
    /// </summary>
    /// <param name="saasLabel">Label for SaaS applications</param>
    /// <param name="datingLabel">Label for dating applications</param>
    /// <param name="crmLabel">Label for CRM applications</param>
    /// <param name="ecommerceLabel">Label for e-commerce applications</param>
    /// <returns>Label matching current application type</returns>
    public string GetLabelByType(
        string saasLabel,
        string datingLabel,
        string crmLabel,
        string ecommerceLabel)
    {
        return CurrentTheme.ApplicationType switch
        {
            "dating" => datingLabel,
            "crm" => crmLabel,
            "ecommerce" => ecommerceLabel,
            _ => saasLabel
        };
    }

    /// <summary>
    /// Gets custom class override if configured, otherwise returns the theme-based classes.
    /// Allows consumers to override specific component styling without modifying package code.
    /// </summary>
    /// <param name="overrideKey">Key for custom override (e.g., "AnalyticsCard11.Icon.Accent")</param>
    /// <param name="defaultClasses">Default theme-based classes if no override exists</param>
    /// <returns>Override classes if configured, otherwise default classes</returns>
    public string GetClassesOrOverride(string overrideKey, string defaultClasses)
    {
        ArgumentNullException.ThrowIfNull(overrideKey);
        ArgumentNullException.ThrowIfNull(defaultClasses);

        return CurrentTheme.CustomClassOverrides.TryGetValue(overrideKey, out string? overrideClasses)
            ? overrideClasses
            : defaultClasses;
    }

    /// <summary>
    /// Gets a complete button class string for primary buttons.
    /// Includes background, hover, focus, and transition effects.
    /// </summary>
    /// <returns>Space-separated Tailwind classes</returns>
    public string GetPrimaryButtonClasses()
    {
        return $"{GetPrimaryClass("bg", "500")} {GetPrimaryHoverClass("bg", "600")} " +
               $"{GetPrimaryFocusClass("ring", "500")} text-white font-semibold rounded-lg px-4 py-2 " +
               $"transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2";
    }

    /// <summary>
    /// Gets outline button classes (transparent with colored border).
    /// </summary>
    /// <returns>Space-separated Tailwind classes</returns>
    public string GetOutlineButtonClasses()
    {
        return $"{GetPrimaryClass("border", "500")} {GetPrimaryClass("text", "500")} " +
               $"{GetPrimaryHoverClass("bg", "50")} {GetPrimaryDarkClass("text", "400")} " +
               $"border-2 font-semibold rounded-lg px-4 py-2 transition-colors duration-200";
    }

    /// <summary>
    /// Gets badge classes for status indicators and labels.
    /// </summary>
    /// <param name="shade">Background shade (default: "100")</param>
    /// <param name="textShade">Text shade (default: "800")</param>
    /// <returns>Space-separated Tailwind classes</returns>
    public string GetBadgeClasses(string shade = "100", string textShade = "800")
    {
        return $"{GetPrimaryClass("bg", shade)} {GetPrimaryClass("text", textShade)} " +
               $"px-2 py-1 rounded-full text-xs font-medium";
    }

    /// <summary>
    /// Gets link classes with hover effects.
    /// </summary>
    /// <returns>Space-separated Tailwind classes</returns>
    public string GetLinkClasses()
    {
        return $"{GetPrimaryClass("text", "600")} {GetPrimaryHoverClass("text", "700")} " +
               $"{GetPrimaryDarkClass("text", "400")} font-medium transition-colors duration-150";
    }

    /// <summary>
    /// Gets card accent border classes (thin colored border).
    /// </summary>
    /// <param name="shade">Border shade</param>
    /// <returns>Space-separated Tailwind classes</returns>
    public string GetCardAccentClasses(string shade = "500")
    {
        return $"border-t-4 {GetPrimaryClass("border-t", shade)}";
    }

    /// <summary>
    /// Gets gradient background classes for primary color.
    /// Commonly used for active states in navigation and subtle backgrounds.
    /// </summary>
    /// <param name="direction">Gradient direction: "to-r", "to-l", "to-t", "to-b", "to-br", etc.</param>
    /// <param name="fromShade">Starting shade (default: "500")</param>
    /// <param name="toShade">Ending shade (default: "500")</param>
    /// <param name="fromOpacity">Opacity for "from" color using Tailwind arbitrary values (default: "0.12")</param>
    /// <param name="toOpacity">Opacity for "to" color using Tailwind arbitrary values (default: "0.04")</param>
    /// <returns>Complete gradient class string</returns>
    public string GetPrimaryGradient(
        string direction = "to-r",
        string fromShade = "500",
        string toShade = "500",
        string fromOpacity = "0.12",
        string toOpacity = "0.04")
    {
        ArgumentNullException.ThrowIfNull(direction);
        ArgumentNullException.ThrowIfNull(fromShade);
        ArgumentNullException.ThrowIfNull(toShade);
        ArgumentNullException.ThrowIfNull(fromOpacity);
        ArgumentNullException.ThrowIfNull(toOpacity);

        return $"bg-gradient-{direction} from-{CurrentTheme.PrimaryColor}-{fromShade}/[{fromOpacity}] " +
               $"to-{CurrentTheme.PrimaryColor}-{toShade}/[{toOpacity}]";
    }

    /// <summary>
    /// Gets gradient background classes for primary color with dark mode support.
    /// Used for navigation items, active states, and hover effects.
    /// </summary>
    /// <param name="direction">Gradient direction</param>
    /// <param name="fromShade">Starting shade</param>
    /// <param name="toShade">Ending shade</param>
    /// <param name="lightOpacity">Opacity in light mode (default: "0.12")</param>
    /// <param name="darkOpacity">Opacity in dark mode (default: "0.24")</param>
    /// <param name="toOpacity">Opacity for "to" color (default: "0.04")</param>
    /// <returns>Complete gradient class string with dark mode variants</returns>
    public string GetPrimaryGradientWithDark(
        string direction = "to-r",
        string fromShade = "500",
        string toShade = "500",
        string lightOpacity = "0.12",
        string darkOpacity = "0.24",
        string toOpacity = "0.04")
    {
        ArgumentNullException.ThrowIfNull(direction);
        ArgumentNullException.ThrowIfNull(fromShade);
        ArgumentNullException.ThrowIfNull(toShade);
        ArgumentNullException.ThrowIfNull(lightOpacity);
        ArgumentNullException.ThrowIfNull(darkOpacity);
        ArgumentNullException.ThrowIfNull(toOpacity);

        return $"bg-gradient-{direction} from-{CurrentTheme.PrimaryColor}-{fromShade}/[{lightOpacity}] " +
               $"dark:from-{CurrentTheme.PrimaryColor}-{fromShade}/[{darkOpacity}] " +
               $"to-{CurrentTheme.PrimaryColor}-{toShade}/[{toOpacity}]";
    }

    /// <summary>
    /// Gets solid gradient background classes (no transparency).
    /// Used for buttons and prominent UI elements.
    /// </summary>
    /// <param name="direction">Gradient direction</param>
    /// <param name="fromShade">Starting shade (default: "500")</param>
    /// <param name="toShade">Ending shade (default: "600")</param>
    /// <returns>Complete gradient class string</returns>
    public string GetPrimarySolidGradient(
        string direction = "to-r",
        string fromShade = "500",
        string toShade = "600")
    {
        ArgumentNullException.ThrowIfNull(direction);
        ArgumentNullException.ThrowIfNull(fromShade);
        ArgumentNullException.ThrowIfNull(toShade);

        return $"bg-gradient-{direction} from-{CurrentTheme.PrimaryColor}-{fromShade} " +
               $"to-{CurrentTheme.PrimaryColor}-{toShade}";
    }

    /// <summary>
    /// Gets gradient background classes for accent color.
    /// </summary>
    /// <param name="direction">Gradient direction</param>
    /// <param name="fromShade">Starting shade</param>
    /// <param name="toShade">Ending shade</param>
    /// <param name="fromOpacity">Opacity for "from" color</param>
    /// <param name="toOpacity">Opacity for "to" color</param>
    /// <returns>Complete gradient class string</returns>
    public string GetAccentGradient(
        string direction = "to-r",
        string fromShade = "500",
        string toShade = "500",
        string fromOpacity = "0.12",
        string toOpacity = "0.04")
    {
        ArgumentNullException.ThrowIfNull(direction);
        ArgumentNullException.ThrowIfNull(fromShade);
        ArgumentNullException.ThrowIfNull(toShade);
        ArgumentNullException.ThrowIfNull(fromOpacity);
        ArgumentNullException.ThrowIfNull(toOpacity);

        return $"bg-gradient-{direction} from-{CurrentTheme.AccentColor}-{fromShade}/[{fromOpacity}] " +
               $"to-{CurrentTheme.AccentColor}-{toShade}/[{toOpacity}]";
    }

    /// <summary>
    /// Gets RGB color value for Chart.js and other libraries requiring RGB strings.
    /// Maps Tailwind color shades to approximate RGB values.
    /// </summary>
    /// <param name="colorType">"primary" or "accent"</param>
    /// <param name="shade">Tailwind shade: "50" through "950"</param>
    /// <returns>RGB string in format "rgb(r, g, b)"</returns>
    public string GetRgbColor(string colorType, string shade)
    {
        ArgumentNullException.ThrowIfNull(colorType);
        ArgumentNullException.ThrowIfNull(shade);

        string color = string.Equals(colorType, "accent", StringComparison.OrdinalIgnoreCase)
            ? CurrentTheme.AccentColor
            : CurrentTheme.PrimaryColor;

        // Tailwind color-to-RGB mappings (approximate values for common shades)
        // These are standard Tailwind v3 RGB values
        Dictionary<string, Dictionary<string, string>> colorMap = new()
        {
            ["violet"] = new()
            {
                ["50"] = "rgb(245, 243, 255)",
                ["100"] = "rgb(237, 233, 254)",
                ["200"] = "rgb(221, 214, 254)",
                ["300"] = "rgb(196, 181, 253)",
                ["400"] = "rgb(167, 139, 250)",
                ["500"] = "rgb(139, 92, 246)",
                ["600"] = "rgb(124, 58, 237)",
                ["700"] = "rgb(109, 40, 217)",
                ["800"] = "rgb(91, 33, 182)",
                ["900"] = "rgb(76, 29, 149)"
            },
            ["blue"] = new()
            {
                ["50"] = "rgb(239, 246, 255)",
                ["100"] = "rgb(219, 234, 254)",
                ["200"] = "rgb(191, 219, 254)",
                ["300"] = "rgb(147, 197, 253)",
                ["400"] = "rgb(96, 165, 250)",
                ["500"] = "rgb(59, 130, 246)",
                ["600"] = "rgb(37, 99, 235)",
                ["700"] = "rgb(29, 78, 216)",
                ["800"] = "rgb(30, 64, 175)",
                ["900"] = "rgb(30, 58, 138)"
            },
            ["pink"] = new()
            {
                ["50"] = "rgb(253, 242, 248)",
                ["100"] = "rgb(252, 231, 243)",
                ["200"] = "rgb(251, 207, 232)",
                ["300"] = "rgb(249, 168, 212)",
                ["400"] = "rgb(244, 114, 182)",
                ["500"] = "rgb(236, 72, 153)",
                ["600"] = "rgb(219, 39, 119)",
                ["700"] = "rgb(190, 24, 93)",
                ["800"] = "rgb(157, 23, 77)",
                ["900"] = "rgb(131, 24, 67)"
            },
            ["purple"] = new()
            {
                ["50"] = "rgb(250, 245, 255)",
                ["100"] = "rgb(243, 232, 255)",
                ["200"] = "rgb(233, 213, 255)",
                ["300"] = "rgb(216, 180, 254)",
                ["400"] = "rgb(192, 132, 252)",
                ["500"] = "rgb(168, 85, 247)",
                ["600"] = "rgb(147, 51, 234)",
                ["700"] = "rgb(126, 34, 206)",
                ["800"] = "rgb(107, 33, 168)",
                ["900"] = "rgb(88, 28, 135)"
            },
            ["sky"] = new()
            {
                ["50"] = "rgb(240, 249, 255)",
                ["100"] = "rgb(224, 242, 254)",
                ["200"] = "rgb(186, 230, 253)",
                ["300"] = "rgb(125, 211, 252)",
                ["400"] = "rgb(56, 189, 248)",
                ["500"] = "rgb(14, 165, 233)",
                ["600"] = "rgb(2, 132, 199)",
                ["700"] = "rgb(3, 105, 161)",
                ["800"] = "rgb(7, 89, 133)",
                ["900"] = "rgb(12, 74, 110)"
            },
            ["emerald"] = new()
            {
                ["50"] = "rgb(236, 253, 245)",
                ["100"] = "rgb(209, 250, 229)",
                ["200"] = "rgb(167, 243, 208)",
                ["300"] = "rgb(110, 231, 183)",
                ["400"] = "rgb(52, 211, 153)",
                ["500"] = "rgb(16, 185, 129)",
                ["600"] = "rgb(5, 150, 105)",
                ["700"] = "rgb(4, 120, 87)",
                ["800"] = "rgb(6, 95, 70)",
                ["900"] = "rgb(6, 78, 59)"
            },
            ["amber"] = new()
            {
                ["50"] = "rgb(255, 251, 235)",
                ["100"] = "rgb(254, 243, 199)",
                ["200"] = "rgb(253, 230, 138)",
                ["300"] = "rgb(252, 211, 77)",
                ["400"] = "rgb(251, 191, 36)",
                ["500"] = "rgb(245, 158, 11)",
                ["600"] = "rgb(217, 119, 6)",
                ["700"] = "rgb(180, 83, 9)",
                ["800"] = "rgb(146, 64, 14)",
                ["900"] = "rgb(120, 53, 15)"
            },
            ["rose"] = new()
            {
                ["50"] = "rgb(255, 241, 242)",
                ["100"] = "rgb(255, 228, 230)",
                ["200"] = "rgb(254, 205, 211)",
                ["300"] = "rgb(253, 164, 175)",
                ["400"] = "rgb(251, 113, 133)",
                ["500"] = "rgb(244, 63, 94)",
                ["600"] = "rgb(225, 29, 72)",
                ["700"] = "rgb(190, 18, 60)",
                ["800"] = "rgb(159, 18, 57)",
                ["900"] = "rgb(136, 19, 55)"
            }
        };

        if (colorMap.TryGetValue(color, out Dictionary<string, string>? shades) &&
            shades.TryGetValue(shade, out string? rgb))
        {
            return rgb;
        }

        // Fallback to violet-500 if color not found
        return "rgb(139, 92, 246)";
    }
}

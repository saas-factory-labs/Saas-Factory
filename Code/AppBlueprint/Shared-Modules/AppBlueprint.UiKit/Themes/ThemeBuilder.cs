using MudBlazor;

namespace AppBlueprint.UiKit.Themes;

/// <summary>
/// Fluent builder for creating custom MudBlazor themes based on AppBlueprint defaults.
/// Provides a type-safe, intuitive API for customizing theme colors, typography, and layout.
/// </summary>
/// <example>
/// <code>
/// var theme = new ThemeBuilder()
///     .WithPrimaryColor("#1E40AF")
///     .WithSecondaryColor("#10B981")
///     .WithBorderRadius("8px")
///     .Build();
/// </code>
/// </example>
public class ThemeBuilder
{
    private readonly MudTheme _theme;

    /// <summary>
    /// Initializes a new ThemeBuilder with the Superhero theme as the base.
    /// </summary>
    public ThemeBuilder()
    {
        // Clone the Superhero theme to avoid modifying the original
        _theme = CloneTheme(CustomThemes.Superherotheme);
    }

    /// <summary>
    /// Initializes a new ThemeBuilder with a custom base theme.
    /// </summary>
    /// <param name="baseTheme">The theme to use as a starting point</param>
    public ThemeBuilder(MudTheme baseTheme)
    {
        _theme = CloneTheme(baseTheme);
    }

    #region Color Methods

    /// <summary>
    /// Sets the primary color used throughout the application.
    /// </summary>
    /// <param name="color">Hex color code (e.g., "#1E40AF")</param>
    public ThemeBuilder WithPrimaryColor(string color)
    {
        _theme.PaletteLight.Primary = color;
        return this;
    }

    /// <summary>
    /// Sets the secondary color used for accents and secondary actions.
    /// </summary>
    /// <param name="color">Hex color code (e.g., "#10B981")</param>
    public ThemeBuilder WithSecondaryColor(string color)
    {
        _theme.PaletteLight.Secondary = color;
        return this;
    }

    /// <summary>
    /// Sets the tertiary color.
    /// </summary>
    /// <param name="color">Hex color code</param>
    public ThemeBuilder WithTertiaryColor(string color)
    {
        _theme.PaletteLight.Tertiary = color;
        return this;
    }

    /// <summary>
    /// Sets the info color used for informational messages.
    /// </summary>
    /// <param name="color">Hex color code (e.g., "#0EA5E9")</param>
    public ThemeBuilder WithInfoColor(string color)
    {
        _theme.PaletteLight.Info = color;
        return this;
    }

    /// <summary>
    /// Sets the success color used for success messages and indicators.
    /// </summary>
    /// <param name="color">Hex color code (e.g., "#22C55E")</param>
    public ThemeBuilder WithSuccessColor(string color)
    {
        _theme.PaletteLight.Success = color;
        return this;
    }

    /// <summary>
    /// Sets the warning color used for warning messages.
    /// </summary>
    /// <param name="color">Hex color code (e.g., "#F59E0B")</param>
    public ThemeBuilder WithWarningColor(string color)
    {
        _theme.PaletteLight.Warning = color;
        return this;
    }

    /// <summary>
    /// Sets the error color used for error messages and validation.
    /// </summary>
    /// <param name="color">Hex color code (e.g., "#EF4444")</param>
    public ThemeBuilder WithErrorColor(string color)
    {
        _theme.PaletteLight.Error = color;
        return this;
    }

    #endregion

    #region Layout Methods

    /// <summary>
    /// Sets the AppBar (top navigation bar) background color.
    /// </summary>
    /// <param name="color">Hex color code</param>
    public ThemeBuilder WithAppBarBackground(string color)
    {
        _theme.PaletteLight.AppbarBackground = color;
        return this;
    }

    /// <summary>
    /// Sets the AppBar text color.
    /// </summary>
    /// <param name="color">Hex color code</param>
    public ThemeBuilder WithAppBarText(string color)
    {
        _theme.PaletteLight.AppbarText = color;
        return this;
    }

    /// <summary>
    /// Sets the drawer (sidebar) background color.
    /// </summary>
    /// <param name="color">Hex color code</param>
    public ThemeBuilder WithDrawerBackground(string color)
    {
        _theme.PaletteLight.DrawerBackground = color;
        return this;
    }

    /// <summary>
    /// Sets the drawer text color.
    /// </summary>
    /// <param name="color">Hex color code</param>
    public ThemeBuilder WithDrawerText(string color)
    {
        _theme.PaletteLight.DrawerText = color;
        return this;
    }

    /// <summary>
    /// Sets the drawer icon color.
    /// </summary>
    /// <param name="color">Hex color code</param>
    public ThemeBuilder WithDrawerIcon(string color)
    {
        _theme.PaletteLight.DrawerIcon = color;
        return this;
    }

    /// <summary>
    /// Sets the background color for the main content area.
    /// </summary>
    /// <param name="color">Hex color code</param>
    public ThemeBuilder WithBackground(string color)
    {
        _theme.PaletteLight.Background = color;
        return this;
    }

    /// <summary>
    /// Sets the surface color (used for cards, dialogs, etc.).
    /// </summary>
    /// <param name="color">Hex color code</param>
    public ThemeBuilder WithSurface(string color)
    {
        _theme.PaletteLight.Surface = color;
        return this;
    }

    #endregion

    #region Typography Methods

    /// <summary>
    /// Sets the primary text color.
    /// </summary>
    /// <param name="color">Hex color code</param>
    public ThemeBuilder WithTextPrimary(string color)
    {
        _theme.PaletteLight.TextPrimary = color;
        return this;
    }

    /// <summary>
    /// Sets the secondary text color (used for less prominent text).
    /// </summary>
    /// <param name="color">Hex color code</param>
    public ThemeBuilder WithTextSecondary(string color)
    {
        _theme.PaletteLight.TextSecondary = color;
        return this;
    }

    /// <summary>
    /// Sets the disabled text color.
    /// </summary>
    /// <param name="color">Hex color code or rgba string</param>
    public ThemeBuilder WithTextDisabled(string color)
    {
        _theme.PaletteLight.TextDisabled = color;
        return this;
    }

    #endregion

    #region Layout Properties

    /// <summary>
    /// Sets the default border radius for components.
    /// </summary>
    /// <param name="radius">CSS border-radius value (e.g., "8px", "0.5rem")</param>
    public ThemeBuilder WithBorderRadius(string radius)
    {
        _theme.LayoutProperties.DefaultBorderRadius = radius;
        return this;
    }

    /// <summary>
    /// Sets the AppBar height.
    /// </summary>
    /// <param name="height">CSS height value (e.g., "64px")</param>
    public ThemeBuilder WithAppBarHeight(string height)
    {
        _theme.LayoutProperties.AppbarHeight = height;
        return this;
    }

    /// <summary>
    /// Sets the drawer width when expanded.
    /// </summary>
    /// <param name="width">CSS width value (e.g., "240px")</param>
    public ThemeBuilder WithDrawerWidth(string width)
    {
        _theme.LayoutProperties.DrawerWidthLeft = width;
        _theme.LayoutProperties.DrawerWidthRight = width;
        return this;
    }

    /// <summary>
    /// Sets the drawer mini width when collapsed.
    /// </summary>
    /// <param name="width">CSS width value (e.g., "56px")</param>
    public ThemeBuilder WithDrawerMiniWidth(string width)
    {
        _theme.LayoutProperties.DrawerMiniWidthLeft = width;
        _theme.LayoutProperties.DrawerMiniWidthRight = width;
        return this;
    }

    #endregion

    #region Advanced Configuration

    /// <summary>
    /// Provides full access to configure the light palette.
    /// Use this for advanced customization beyond the fluent methods.
    /// </summary>
    /// <param name="configurePalette">Callback to configure the palette</param>
    /// <example>
    /// <code>
    /// builder.ConfigurePalette(palette =>
    /// {
    ///     palette.Black = "#000000";
    ///     palette.White = "#FFFFFF";
    ///     palette.LinesDefault = "rgba(0,0,0,0.12)";
    /// });
    /// </code>
    /// </example>
    public ThemeBuilder ConfigurePalette(Action<PaletteLight> configurePalette)
    {
        configurePalette(_theme.PaletteLight);
        return this;
    }

    /// <summary>
    /// Provides full access to configure layout properties.
    /// </summary>
    /// <param name="configureLayout">Callback to configure layout properties</param>
    public ThemeBuilder ConfigureLayout(Action<LayoutProperties> configureLayout)
    {
        configureLayout(_theme.LayoutProperties);
        return this;
    }

    /// <summary>
    /// Provides full access to configure dark mode palette.
    /// </summary>
    /// <param name="configureDarkPalette">Callback to configure dark palette</param>
    public ThemeBuilder ConfigureDarkPalette(Action<PaletteDark> configureDarkPalette)
    {
        if (_theme.PaletteDark == null)
        {
            _theme.PaletteDark = new PaletteDark();
        }
        configureDarkPalette(_theme.PaletteDark);
        return this;
    }

    #endregion

    #region Preset Themes

    /// <summary>
    /// Applies a professional blue theme (suitable for business applications).
    /// </summary>
    public ThemeBuilder UseProfessionalBluePreset()
    {
        return this
            .WithPrimaryColor("#1E40AF")
            .WithSecondaryColor("#64748B")
            .WithSuccessColor("#22C55E")
            .WithInfoColor("#0EA5E9")
            .WithWarningColor("#F59E0B")
            .WithErrorColor("#EF4444")
            .WithAppBarBackground("#FFFFFF")
            .WithDrawerBackground("#F8FAFC")
            .WithBorderRadius("6px");
    }

    /// <summary>
    /// Applies a modern dark theme with vibrant accents.
    /// </summary>
    public ThemeBuilder UseModernDarkPreset()
    {
        return this
            .WithPrimaryColor("#8B5CF6")
            .WithSecondaryColor("#EC4899")
            .WithBackground("#0F172A")
            .WithSurface("#1E293B")
            .WithTextPrimary("#F1F5F9")
            .WithTextSecondary("#94A3B8")
            .WithAppBarBackground("#1E293B")
            .WithDrawerBackground("#0F172A")
            .WithBorderRadius("8px");
    }

    /// <summary>
    /// Applies a clean, minimal theme with soft colors.
    /// </summary>
    public ThemeBuilder UseMinimalPreset()
    {
        return this
            .WithPrimaryColor("#6366F1")
            .WithSecondaryColor("#94A3B8")
            .WithSuccessColor("#10B981")
            .WithBackground("#FFFFFF")
            .WithSurface("#F9FAFB")
            .WithTextPrimary("#111827")
            .WithTextSecondary("#6B7280")
            .WithBorderRadius("4px");
    }

    #endregion

    /// <summary>
    /// Builds and returns the configured MudBlazor theme.
    /// </summary>
    /// <returns>The fully configured MudTheme instance</returns>
    public MudTheme Build()
    {
        return _theme;
    }

    /// <summary>
    /// Creates a deep clone of a MudTheme to avoid mutating the original.
    /// </summary>
    private static MudTheme CloneTheme(MudTheme source)
    {
        return new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Black = source.PaletteLight.Black,
                White = source.PaletteLight.White,
                Primary = source.PaletteLight.Primary,
                PrimaryContrastText = source.PaletteLight.PrimaryContrastText,
                Secondary = source.PaletteLight.Secondary,
                SecondaryContrastText = source.PaletteLight.SecondaryContrastText,
                Tertiary = source.PaletteLight.Tertiary,
                TertiaryContrastText = source.PaletteLight.TertiaryContrastText,
                Info = source.PaletteLight.Info,
                InfoContrastText = source.PaletteLight.InfoContrastText,
                Success = source.PaletteLight.Success,
                SuccessContrastText = source.PaletteLight.SuccessContrastText,
                Warning = source.PaletteLight.Warning,
                WarningContrastText = source.PaletteLight.WarningContrastText,
                Error = source.PaletteLight.Error,
                ErrorContrastText = source.PaletteLight.ErrorContrastText,
                Dark = source.PaletteLight.Dark,
                DarkContrastText = source.PaletteLight.DarkContrastText,
                TextPrimary = source.PaletteLight.TextPrimary,
                TextSecondary = source.PaletteLight.TextSecondary,
                TextDisabled = source.PaletteLight.TextDisabled,
                ActionDefault = source.PaletteLight.ActionDefault,
                ActionDisabled = source.PaletteLight.ActionDisabled,
                ActionDisabledBackground = source.PaletteLight.ActionDisabledBackground,
                Background = source.PaletteLight.Background,
                BackgroundGray = source.PaletteLight.BackgroundGray,
                Surface = source.PaletteLight.Surface,
                DrawerBackground = source.PaletteLight.DrawerBackground,
                DrawerText = source.PaletteLight.DrawerText,
                DrawerIcon = source.PaletteLight.DrawerIcon,
                AppbarBackground = source.PaletteLight.AppbarBackground,
                AppbarText = source.PaletteLight.AppbarText,
                LinesDefault = source.PaletteLight.LinesDefault,
                LinesInputs = source.PaletteLight.LinesInputs,
                TableLines = source.PaletteLight.TableLines,
                TableStriped = source.PaletteLight.TableStriped,
                TableHover = source.PaletteLight.TableHover,
                Divider = source.PaletteLight.Divider
            },
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = source.LayoutProperties.DefaultBorderRadius,
                DrawerMiniWidthLeft = source.LayoutProperties.DrawerMiniWidthLeft,
                DrawerMiniWidthRight = source.LayoutProperties.DrawerMiniWidthRight,
                DrawerWidthLeft = source.LayoutProperties.DrawerWidthLeft,
                DrawerWidthRight = source.LayoutProperties.DrawerWidthRight,
                AppbarHeight = source.LayoutProperties.AppbarHeight
            }
        };
    }
}

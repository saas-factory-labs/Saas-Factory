using MudBlazor;

namespace AppBlueprint.Uikit.Themes;

public static class CustomThemes
{
    public static readonly MudTheme Superherotheme = new()
    {
        PaletteLight = new PaletteLight
        {
            Black = "#000",
            White = "#fff",
            Primary = "#df6919",
            PrimaryContrastText = "#f9e1d1",
            Secondary = "#4e5d6c",
            SecondaryContrastText = "#dcdfe2",
            Tertiary = "rgba(89,132,106,1)",
            TertiaryContrastText = "#ebebeb",
            Info = "#5bc0de",
            InfoContrastText = "#def2f8",
            Success = "#5cb85c",
            SuccessContrastText = "#def1de",
            Warning = "#ffc107",
            WarningContrastText = "#fff3cd",
            Error = "rgba(244,67,54,1)",
            ErrorContrastText = "rgba(255,255,255,1)",
            Dark = "#20374c",
            DarkContrastText = "#1a1d20",
            TextPrimary = "#592a0a",
            TextSecondary = "#1f252b",
            TextDisabled = "rgba(0,0,0,0.3764705882352941)",
            ActionDefault = "rgba(0,0,0,0.5372549019607843)",
            ActionDisabled = "rgba(0,0,0,0.25882352941176473)",
            ActionDisabledBackground = "rgba(0,0,0,0.11764705882352941)",
            Background = "rgba(255,255,255,1)",
            BackgroundGray = "rgba(245,245,245,1)",
            Surface = "rgba(255,255,255,1)",
            DrawerBackground = "#dcdfe2",
            DrawerText = "#4e5d6c",
            DrawerIcon = "#4e5d6c",
            AppbarBackground = "#f9e1d1",
            AppbarText = "#df6919",
            LinesDefault = "rgba(0,0,0,0.11764705882352941)",
            LinesInputs = "rgba(189,189,189,1)",
            TableLines = "rgba(224,224,224,1)",
            TableStriped = "rgba(0,0,0,0.0196078431372549)",
            TableHover = "rgba(0,0,0,0.0392156862745098)",
            Divider = "rgba(224,224,224,1)"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "4px",
            DrawerMiniWidthLeft = "56px",
            DrawerMiniWidthRight = "56px",
            DrawerWidthLeft = "240px",
            DrawerWidthRight = "240px",
            AppbarHeight = "64px"
        }
    };
}

public class Typography
{
    public required IDefault Default { get; set; }
    public required IH1 H1 { get; set; }
    public required IH2 H2 { get; set; }
    public required IH3 H3 { get; set; }
    public required IH4 H4 { get; set; }
    public required IH5 H5 { get; set; }
    public required IH6 H6 { get; set; }
    public required ISubtitle1 Subtitle1 { get; set; }
    public required ISubtitle2 Subtitle2 { get; set; }
    public required IBody1 Body1 { get; set; }
    public required IBody2 Body2 { get; set; }
    public required IInputTypography Input { get; set; }
    public required IButton Button { get; set; }
    public required ICaption Caption { get; set; }
    public required IOverline Overline { get; set; }
}

public interface IDefault
{
    IReadOnlyList<string> FontFamily { get; set; }
    int FontWeight { get; set; }
    string FontSize { get; set; }
    double LineHeight { get; set; }
    string LetterSpacing { get; set; }
    string TextTransform { get; set; }
}

public interface IH1 : IDefault
{
}

public interface IH2 : IDefault
{
}

public interface IH3 : IDefault
{
}

public interface IH4 : IDefault
{
}

public interface IH5 : IDefault
{
}

public interface IH6 : IDefault
{
}

public interface ISubtitle1 : IDefault
{
}

public interface ISubtitle2 : IDefault
{
}

public interface IBody1 : IDefault
{
}

public interface IBody2 : IDefault
{
}

public interface IInputTypography : IDefault
{
}

public interface IButton : IDefault
{
}

public interface ICaption : IDefault
{
}

public interface IOverline : IDefault
{
}

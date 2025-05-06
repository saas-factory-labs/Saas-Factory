using MudBlazor;

namespace AppBlueprint.UiKit.Enums;

public enum CustomColors
{
    Primary,
    Secondary,
    Success,
    Warning,
    Error,
    Info,
    DeepOrange,
    Amber,
    Lime,
    BlueGray,
    Orange
}

public static class CustomColorMapper
{
    public static string GetHex(CustomColors color)
    {
        return color switch
        {
            CustomColors.Primary => Colors.Blue.Default,
            CustomColors.Secondary => Colors.Purple.Default,
            CustomColors.Success => Colors.Green.Default,
            CustomColors.Warning => Colors.Yellow.Default,
            CustomColors.Error => Colors.Red.Default,
            CustomColors.Info => Colors.LightBlue.Default,
            CustomColors.DeepOrange => Colors.DeepOrange.Default,
            CustomColors.Amber => Colors.Amber.Default,
            CustomColors.Lime => Colors.Lime.Default,
            CustomColors.Orange => Colors.Orange.Default,
            _ => Colors.Shades.White // Fallback
        };
    }
}
// using MudBlazor;
//
// var extendedPalette = new Palette()
// {
//     Primary = MudBlazor.Colors.Blue.Default,
//     Secondary = MudBlazor.Colors.Purple.Default,
//     Success = MudBlazor.Colors.Green.Default,
//     Warning = MudBlazor.Colors.Yellow.Default,
//     Error = MudBlazor.Colors.Red.Default,
//     Info = MudBlazor.Colors.LightBlue.Default,
//     Dark = MudBlazor.Colors.Grey.Darken4,
//     Light = MudBlazor.Colors.Grey.Lighten4,
//     // Add custom Material colors here
//     AppbarBackground = MudBlazor.Colors.DeepOrange.Darken2,
//     DrawerBackground = MudBlazor.Colors.BlueGrey.Darken4,
//     DrawerText = MudBlazor.Colors.Grey.Lighten4
// };
//
// var customTheme = new MudTheme()
// {
//     Palette = extendedPalette
// };

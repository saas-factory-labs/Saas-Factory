namespace AppBlueprint.UiKit.Models;

public class BreadcrumbItem(string text, string href, bool disabled = false, string? icon = null)
{
    public string Text { get; set; } = text;
    public string Href { get; set; } = href;
    public bool Disabled { get; set; } = disabled;
    public string? Icon { get; set; } = icon;
}

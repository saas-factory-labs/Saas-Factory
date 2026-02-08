namespace AppBlueprint.UiKit.Models;

/// <summary>
/// Represents a single item in a breadcrumb navigation trail.
/// </summary>
/// <param name="text">The display text for the breadcrumb.</param>
/// <param name="href">The navigation URL.</param>
/// <param name="disabled">Whether the breadcrumb is disabled.</param>
/// <param name="icon">Optional icon identifier.</param>
public class BreadcrumbItem(string text, string href, bool disabled = false, string? icon = null)
{
    public string Text { get; } = text;
    public string Href { get; } = href;
    public bool Disabled { get; } = disabled;
    public string? Icon { get; } = icon;
}

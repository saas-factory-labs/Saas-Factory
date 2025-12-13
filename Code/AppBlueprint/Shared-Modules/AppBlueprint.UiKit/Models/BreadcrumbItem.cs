namespace AppBlueprint.UiKit.Models;

public class BreadcrumbItem
{
    public string Text { get; set; }
    public string Href { get; set; }
    public bool Disabled { get; set; }
    public string Icon { get; set; }

    public BreadcrumbItem(string text, string href, bool disabled = false, string icon = null)
    {
        Text = text;
        Href = href;
        Disabled = disabled;
        Icon = icon;
    }
}

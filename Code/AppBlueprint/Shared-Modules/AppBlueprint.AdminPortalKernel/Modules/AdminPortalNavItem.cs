namespace AppBlueprint.AdminPortalKernel.Modules;

/// <summary>Navigation entry contributed by an admin portal module to the shell menu.</summary>
/// <param name="Title">Link text shown in the navigation menu.</param>
/// <param name="Href">Absolute path of the page; must live under <c>/apps/{slug}/admin</c>.</param>
/// <param name="Icon">MudBlazor icon name.</param>
public sealed record AdminPortalNavItem(string Title, string Href, string Icon);

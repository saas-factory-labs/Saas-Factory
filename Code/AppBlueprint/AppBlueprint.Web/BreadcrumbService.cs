using MudBlazor;

namespace AppBlueprint.Web;

internal sealed class BreadcrumbService
{
    public List<BreadcrumbItem> Breadcrumbs { get; private set; } = new();
    public event EventHandler? OnBreadcrumbsChanged;

    public void UpdateBreadcrumbs(List<BreadcrumbItem> breadcrumbs)
    {
        Breadcrumbs = breadcrumbs;
        OnBreadcrumbsChanged?.Invoke(this, EventArgs.Empty);
    }
}

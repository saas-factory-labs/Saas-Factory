using MudBlazor;

namespace AppBlueprint.UiKit.Services;

/// <summary>
/// Service for managing breadcrumb navigation throughout the application.
/// Provides centralized breadcrumb state management with event notifications.
/// </summary>
public class BreadcrumbService
{
    private List<BreadcrumbItem> _breadcrumbs = new();

    /// <summary>
    /// Gets the current breadcrumb items.
    /// </summary>
    public IReadOnlyList<BreadcrumbItem> Breadcrumbs => _breadcrumbs.AsReadOnly();

    /// <summary>
    /// Event raised when breadcrumbs are updated.
    /// </summary>
    public event Action? OnBreadcrumbsChanged;

    /// <summary>
    /// Sets the breadcrumb items and notifies subscribers of the change.
    /// </summary>
    /// <param name="breadcrumbs">The new breadcrumb items</param>
    public void SetBreadcrumbs(List<BreadcrumbItem> breadcrumbs)
    {
        ArgumentNullException.ThrowIfNull(breadcrumbs);

        _breadcrumbs = breadcrumbs;
        OnBreadcrumbsChanged?.Invoke();
    }

    /// <summary>
    /// Clears all breadcrumb items.
    /// </summary>
    public void Clear()
    {
        _breadcrumbs.Clear();
        OnBreadcrumbsChanged?.Invoke();
    }

    /// <summary>
    /// Adds a breadcrumb item to the end of the breadcrumb trail.
    /// </summary>
    /// <param name="item">The breadcrumb item to add</param>
    public void AddBreadcrumb(BreadcrumbItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _breadcrumbs.Add(item);
        OnBreadcrumbsChanged?.Invoke();
    }

    /// <summary>
    /// Removes the last breadcrumb item from the trail.
    /// </summary>
    public void RemoveLastBreadcrumb()
    {
        if (_breadcrumbs.Count > 0)
        {
            _breadcrumbs.RemoveAt(_breadcrumbs.Count - 1);
            OnBreadcrumbsChanged?.Invoke();
        }
    }
}


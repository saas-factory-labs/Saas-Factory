namespace AppBlueprint.UiKit.Services;

/// <summary>
/// Service for determining which menu items should be visible based on application-specific logic.
/// Implement this in your application to customize menu visibility.
/// </summary>
public interface IMenuConfigurationService
{
    /// <summary>
    /// Determines if a menu item should be visible to the current user.
    /// </summary>
    /// <param name="menuItemId">Unique identifier for the menu item (e.g., "dashboard", "shop", "users")</param>
    /// <returns>True if the menu item should be visible, false otherwise</returns>
    Task<bool> ShouldShowMenuItemAsync(string menuItemId);
}

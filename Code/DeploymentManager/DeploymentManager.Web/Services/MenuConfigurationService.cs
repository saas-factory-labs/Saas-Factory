using AppBlueprint.UiKit.Services;

namespace DeploymentManager.Web.Services;

internal sealed class MenuConfigurationService : IMenuConfigurationService
{
    public Task<bool> ShouldShowMenuItemAsync(string menuItemId) => Task.FromResult(true);
}

using AppBlueprint.Application.Services;
using AppBlueprint.SharedKernel.Enums;
using AppBlueprint.UiKit.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace AppBlueprint.Web.Services;

public sealed class MenuConfigurationService(
    AuthenticationStateProvider authenticationStateProvider,
    ICurrentTenantService currentTenantService) : IMenuConfigurationService
{
    // Menu items visible only to unauthenticated users (demo mode)
    private static readonly HashSet<string> DemoOnlyMenuItems = new()
    {
        "shop",
        "customers",
        "cart2",
        "cart3",
        "tasks",
        "job-board",
        "finance"
    };

    // Menu items visible only to B2B (Organization) tenants
    private static readonly HashSet<string> B2BOnlyMenuItems = new()
    {
        "community-users",
        "account",
        "notifications",
        "billing"
    };

    // Menu items visible to all authenticated users (B2C and B2B)
    private static readonly HashSet<string> AuthenticatedMenuItems = new()
    {
        "dashboard",
        "orders",
        "invoices",
        "shop2",
        "single-product",
        "cart",
        "pay"
    };

    public async Task<bool> ShouldShowMenuItemAsync(string menuItemId)
    {
        AuthenticationState authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        bool isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;

        // Check if this is a demo user (has "demo" role claim)
        bool isDemoUser = authState.User.HasClaim("role", "demo");

        // Demo users - show everything from Cruip template
        if (isDemoUser)
        {
            return true;
        }

        // Unauthenticated users - redirect to login (show nothing)
        if (!isAuthenticated)
        {
            return true;
        }

        // Authenticated users - hide demo-only items
        if (DemoOnlyMenuItems.Contains(menuItemId))
        {
            return false;
        }

        // Check tenant type for B2B-specific items
        if (B2BOnlyMenuItems.Contains(menuItemId))
        {
            TenantType? tenantType = await currentTenantService.GetTenantTypeAsync();
            return tenantType == TenantType.Organization;
        }

        // All other items (authenticated items) are visible to authenticated users
        return AuthenticatedMenuItems.Contains(menuItemId) || !DemoOnlyMenuItems.Contains(menuItemId);
    }
}

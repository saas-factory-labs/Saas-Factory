# AppBlueprint UiKit - Usage Guide for Consuming Applications

A flexible, customizable Blazor UI component library for building SaaS applications with role-based menu visibility and authorization.

## Architecture Overview

The UiKit is designed to be **application-agnostic** while providing powerful UI components and layout management. Business logic for menu visibility and authorization is **implemented by the consuming application**, not hardcoded in the library.

### Key Principles

- **Dependency Inversion**: UiKit depends on abstractions (`IMenuConfigurationService`), consuming apps provide implementations
- **Separation of Concerns**: UI components in UiKit, business rules in your application
- **Flexibility**: Different apps can have completely different authorization models (B2C/B2B, roles, subscriptions, etc.)
- **Maintainability**: Upgrade UiKit without breaking your business logic

## Getting Started

### 1. Add Package Reference

```xml
<!-- YourApp.csproj -->
<ItemGroup>
  <ProjectReference Include="..\AppBlueprint.UiKit\AppBlueprint.UiKit.csproj" />
  <ProjectReference Include="..\AppBlueprint.SharedKernel\AppBlueprint.SharedKernel.csproj" />
</ItemGroup>
```

### 2. Implement IMenuConfigurationService

The `IMenuConfigurationService` interface controls which menu items are visible to the current user. This is where you implement **your application's business logic**.

```csharp
namespace AppBlueprint.UiKit.Services;

public interface IMenuConfigurationService
{
    /// <summary>
    /// Determines if a menu item should be visible to the current user.
    /// </summary>
    /// <param name="menuItemId">Unique identifier for the menu item (e.g., "dashboard", "reports", "settings")</param>
    /// <returns>True if the menu item should be visible, false otherwise</returns>
    Task<bool> ShouldShowMenuItemAsync(string menuItemId);
}
```

### 3. Register Your Service

```csharp
// Program.cs
using AppBlueprint.UiKit.Services;
using YourApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Register your custom menu configuration service
builder.Services.AddScoped<IMenuConfigurationService, YourMenuService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
```

### 4. Use UiKit Components

```razor
@* YourApp.Web/Components/Layout/MainLayout.razor *@
@using AppBlueprint.UiKit.Components.Layout
@inherits LayoutComponentBase

<Sidebar SidebarOpen="@sidebarOpen" OnCloseSidebar="@(() => sidebarOpen = false)" />

<div class="flex flex-col flex-1">
    <Header OnToggleSidebar="@(() => sidebarOpen = !sidebarOpen)" />
    
    <main class="grow">
        @Body
    </main>
</div>

@code {
    private bool sidebarOpen = false;
}
```

## Implementation Examples

### Example 1: B2C/B2B Multi-Tenant SaaS

```csharp
// YourApp.Web/Services/MultiTenantMenuService.cs
using AppBlueprint.Application.Services;
using AppBlueprint.SharedKernel.Enums;
using AppBlueprint.UiKit.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace YourApp.Web.Services;

public sealed class MultiTenantMenuService(
    AuthenticationStateProvider authenticationStateProvider,
    ICurrentTenantService currentTenantService) : IMenuConfigurationService
{
    // Menu items visible only to unauthenticated users (demo/marketing)
    private static readonly HashSet<string> DemoOnlyMenuItems = new()
    {
        "features",
        "pricing",
        "about"
    };

    // Menu items visible only to B2B (Organization) tenants
    private static readonly HashSet<string> B2BOnlyMenuItems = new()
    {
        "team-management",
        "admin-settings",
        "billing"
    };

    // Menu items visible to all authenticated users (B2C and B2B)
    private static readonly HashSet<string> AuthenticatedMenuItems = new()
    {
        "dashboard",
        "profile",
        "settings"
    };

    public async Task<bool> ShouldShowMenuItemAsync(string menuItemId)
    {
        AuthenticationState authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        bool isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;

        // Unauthenticated users - show demo/marketing items only
        if (!isAuthenticated)
        {
            return DemoOnlyMenuItems.Contains(menuItemId);
        }

        // Authenticated users - hide demo items
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

        // Show authenticated items to all authenticated users
        return AuthenticatedMenuItems.Contains(menuItemId);
    }
}
```

### Example 2: Dating App with Free/Premium Tiers

```csharp
// DatingApp.Web/Services/DatingAppMenuService.cs
using AppBlueprint.UiKit.Services;
using DatingApp.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace DatingApp.Web.Services;

public sealed class DatingAppMenuService(
    AuthenticationStateProvider authStateProvider,
    IUserProfileService profileService) : IMenuConfigurationService
{
    // Features available only to premium subscribers
    private static readonly HashSet<string> PremiumOnlyItems = new()
    {
        "advanced-search",
        "unlimited-likes",
        "see-who-liked-you",
        "boost-profile",
        "read-receipts"
    };

    // Features available to all authenticated users
    private static readonly HashSet<string> FreeUserItems = new()
    {
        "matches",
        "messages",
        "profile",
        "discover",
        "settings"
    };

    // Public pages for unauthenticated users
    private static readonly HashSet<string> PublicItems = new()
    {
        "home",
        "how-it-works",
        "success-stories",
        "pricing"
    };

    public async Task<bool> ShouldShowMenuItemAsync(string menuItemId)
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        bool isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;

        // Unauthenticated users - show public pages only
        if (!isAuthenticated)
        {
            return PublicItems.Contains(menuItemId);
        }

        // Check premium features
        if (PremiumOnlyItems.Contains(menuItemId))
        {
            var userProfile = await profileService.GetCurrentUserProfileAsync();
            return userProfile.IsPremium;
        }

        // All authenticated users can see free features
        return FreeUserItems.Contains(menuItemId);
    }
}
```

### Example 3: CRM with Role-Based Access

```csharp
// CrmApp.Web/Services/CrmMenuService.cs
using AppBlueprint.UiKit.Services;
using CrmApp.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace CrmApp.Web.Services;

public sealed class CrmMenuService(
    AuthenticationStateProvider authStateProvider,
    IRoleService roleService) : IMenuConfigurationService
{
    // Define which roles can access each menu item
    private static readonly Dictionary<string, string[]> MenuItemRoles = new()
    {
        ["dashboard"] = ["Admin", "Manager", "SalesRep"],
        ["contacts"] = ["Admin", "Manager", "SalesRep"],
        ["leads"] = ["Admin", "Manager", "SalesRep"],
        ["deals"] = ["Admin", "Manager", "SalesRep"],
        ["reports"] = ["Admin", "Manager"],
        ["analytics"] = ["Admin", "Manager"],
        ["team-performance"] = ["Admin", "Manager"],
        ["settings"] = ["Admin"],
        ["user-management"] = ["Admin"],
        ["billing"] = ["Admin"],
        ["integrations"] = ["Admin"]
    };

    public async Task<bool> ShouldShowMenuItemAsync(string menuItemId)
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        
        // Must be authenticated to see any CRM features
        if (!authState.User.Identity?.IsAuthenticated ?? true)
        {
            return false;
        }

        // Get user's roles
        var userRoles = await roleService.GetUserRolesAsync();
        
        // Check if menu item has role restrictions
        if (!MenuItemRoles.TryGetValue(menuItemId, out var allowedRoles))
        {
            // If not configured, allow by default (or return false to deny by default)
            return true;
        }

        // Check if user has any of the required roles
        return userRoles.Any(role => allowedRoles.Contains(role));
    }
}
```

## Using TenantTypeAuthorize Component

The `TenantTypeAuthorize` component provides route-level protection based on tenant types. It's **generic and reusable** - you can use it as-is or create your own authorization components.

### TenantTypeAuthorize Usage

```razor
@page "/admin/settings"
@using AppBlueprint.SharedKernel.Enums

<TenantTypeAuthorize AllowedTenantTypes="@(new List<TenantType> { TenantType.Organization })">
    <h1>Admin Settings</h1>
    <!-- B2B Organization-only content -->
</TenantTypeAuthorize>
```

### TenantTypeAuthorize Parameters

| Parameter                | Type                      | Description |
|--------------------------|---------------------------|-------------|
| `AllowedTenantTypes`     | `List<TenantType>?`       | List of tenant types allowed to access this page (e.g., `TenantType.Personal`, `TenantType.Organization`) |
| `AllowUnauthenticated`   | `bool`                    | If `true`, only unauthenticated users can access (useful for demo pages) |
| `ChildContent`           | `RenderFragment?`         | Content to render if authorized |

### TenantTypeAuthorize Behavior

- If **unauthenticated** and `AllowUnauthenticated=true` → Shows content
- If **unauthenticated** and `AllowUnauthenticated=false` → Redirects to `/signup`
- If **authenticated** and tenant type matches `AllowedTenantTypes` → Shows content
- If **authenticated** and tenant type doesn't match → Redirects to `/`
- If **authenticated** and `AllowUnauthenticated=true` with no `AllowedTenantTypes` → Redirects to `/` (demo-only page)

### Examples

**Demo-only page (unauthenticated users only):**
```razor
@page "/demo/features"

<TenantTypeAuthorize AllowUnauthenticated="true">
    <h1>Try Our Features</h1>
    <!-- Marketing demo content -->
</TenantTypeAuthorize>
```

**B2B-only page:**
```razor
@page "/team/members"
@using AppBlueprint.SharedKernel.Enums

<TenantTypeAuthorize AllowedTenantTypes="@(new List<TenantType> { TenantType.Organization })">
    <h1>Team Members</h1>
    <!-- Organization team management -->
</TenantTypeAuthorize>
```

**Multiple tenant types:**
```razor
@page "/reports/analytics"
@using AppBlueprint.SharedKernel.Enums

<TenantTypeAuthorize AllowedTenantTypes="@(new List<TenantType> { TenantType.Personal, TenantType.Organization })">
    <h1>Analytics Dashboard</h1>
    <!-- Available to both B2C and B2B users -->
</TenantTypeAuthorize>
```

## Creating Custom Authorization Components

If `TenantTypeAuthorize` doesn't fit your needs, create your own:

```razor
@* YourApp.Web/Components/Shared/PremiumOnly.razor *@
@inject ISubscriptionService SubscriptionService
@inject NavigationManager Navigation

@if (_isLoading)
{
    <div class="flex items-center justify-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-violet-500"></div>
    </div>
}
else if (_hasAccess)
{
    @ChildContent
}
else
{
    <div class="bg-yellow-50 border border-yellow-200 rounded-lg p-6">
        <h3 class="text-xl font-bold text-yellow-800">Premium Feature</h3>
        <p class="text-yellow-700 mt-2">Upgrade to Premium to access this feature!</p>
        <button @onclick="NavigateToUpgrade" class="mt-4 btn btn-primary">
            Upgrade Now
        </button>
    </div>
}

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    
    private bool _isLoading = true;
    private bool _hasAccess;
    
    protected override async Task OnInitializedAsync()
    {
        var subscription = await SubscriptionService.GetCurrentSubscriptionAsync();
        _hasAccess = subscription?.IsPremium ?? false;
        _isLoading = false;
    }
    
    private void NavigateToUpgrade()
    {
        Navigation.NavigateTo("/upgrade");
    }
}
```

**Usage:**
```razor
@page "/advanced-search"

<PremiumOnly>
    <h1>Advanced Search</h1>
    <!-- Premium feature content -->
</PremiumOnly>
```

## Available Components

### Layout Components
- `Sidebar` - Responsive sidebar with menu management
- `Header` - Top navigation bar
- `Footer` - Application footer

### UI Components
- Buttons, Cards, Modals, Tooltips
- Forms and Input Components
- Tables and Data Grids
- Loading States and Skeletons

### Authorization Components
- `TenantTypeAuthorize` - Tenant-based route protection

## What to Use vs. What to Ignore

### ✅ Reusable Components (Use in Your App)
- All UI components (buttons, cards, modals, etc.)
- Layout components (Sidebar, Header, Footer)
- `IMenuConfigurationService` interface
- `TenantTypeAuthorize` component

### 🔧 Implement in Your App
- **Custom `IMenuConfigurationService`** - Your business logic for menu visibility
- **Your application pages** - Don't reuse template pages
- **Custom authorization components** - If TenantTypeAuthorize doesn't fit

### 🗑️ Template Examples (Reference Only)
The following pages are **Cruip template examples** for styling reference only:
- Shop, Shop2, Customers, Single Product, Cart, Pay
- Tasks, Job Board, Finance
- Community Users
- Messages, Campaigns, Inbox

**Don't use these in production** - Create your own pages with your business logic.

## Best Practices

### 1. Keep Business Logic in Your App
```csharp
// ✅ GOOD - Business logic in your MenuConfigurationService
public async Task<bool> ShouldShowMenuItemAsync(string menuItemId)
{
    var user = await GetCurrentUserAsync();
    return user.CanAccessFeature(menuItemId);
}
```

```csharp
// ❌ BAD - Don't modify UiKit components directly
// UiKit/Components/Layout/Sidebar.razor
@if (user.IsPremium) // Don't hardcode business logic here!
{
    <li>Premium Feature</li>
}
```

### 2. Use Meaningful Menu Item IDs
```csharp
// ✅ GOOD - Clear, descriptive IDs
"dashboard", "user-profile", "admin-settings", "billing-reports"

// ❌ BAD - Generic or unclear IDs
"page1", "menu-item", "feature"
```

### 3. Pre-load Menu Visibility
The Sidebar component pre-loads all menu item visibility on initialization for performance. Make sure your `ShouldShowMenuItemAsync` is efficient.

```csharp
// ✅ GOOD - Cache user permissions
private UserPermissions? _cachedPermissions;

public async Task<bool> ShouldShowMenuItemAsync(string menuItemId)
{
    _cachedPermissions ??= await LoadUserPermissionsAsync();
    return _cachedPermissions.HasAccess(menuItemId);
}
```

### 4. Handle Authorization Consistently
Use both UI hiding (menu visibility) and route protection (TenantTypeAuthorize or custom components):

```razor
<!-- Hide from menu -->
@* Menu service returns false for "admin-settings" *@

<!-- AND protect the route -->
@page "/admin/settings"

<TenantTypeAuthorize AllowedTenantTypes="@AdminOnly">
    <!-- Protected content -->
</TenantTypeAuthorize>
```

## Upgrading the Library

When UiKit is updated:

1. **Your `IMenuConfigurationService` implementation is safe** - It's in your app, not the library
2. **Your business logic is preserved** - Menu visibility rules won't break
3. **UI improvements automatically apply** - Get new components and styling updates
4. **Breaking changes are isolated** - Only affects component APIs, not your business rules

## Support

For issues, questions, or contributions, please visit:
- GitHub: [saas-factory-labs/Saas-Factory](https://github.com/saas-factory-labs/Saas-Factory)
- Documentation: [docs/README.md](../../../../docs/README.md)

## License

See [LICENSE](../../../../LICENSE) for details.

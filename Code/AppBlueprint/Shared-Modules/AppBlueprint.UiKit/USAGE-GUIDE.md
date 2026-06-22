# AppBlueprint UiKit Usage Guide

This is the canonical usage guide for `AppBlueprint.UiKit`.

It replaces the older split documentation that previously lived in:

- `USAGE.md`
- `THEME-USAGE-GUIDE.md`

## Overview

`AppBlueprint.UiKit` is a Razor class library for AppBlueprint-style layouts, pages, and reusable UI components built around:

- Tailwind-based styling
- a singleton `ThemeService` for dynamic theme classes
- reusable layout components such as `Sidebar`, `Header`, and `MainLayout`
- application-owned menu visibility through `IMenuConfigurationService`
- tenant-aware route protection through `TenantTypeAuthorize`

Important architectural boundary:

- UiKit owns reusable UI and theme primitives.
- The consuming application still owns business rules, authorization rules, and menu visibility.

## Current Public Integration Surface

The main entry points currently exposed by the library are:

- `AddUiKit(this IServiceCollection services)`
- `AddUiKitWithTheme(this IServiceCollection services, IConfiguration configuration, string sectionName = "Theme")`
- `AddUiKitWithTheme(this IServiceCollection services, Action<ThemeConfiguration> configureTheme)`
- `AddUiKitBlog(...)`
- `IMenuConfigurationService`
- `ThemeService`
- `TenantTypeAuthorize`

Do not rely on older docs that reference `AddUiKitWithPreset`, `ThemePreset`, or `ThemeBuilder` as if they are part of the current public registration API. Those references do not reflect the current `ServiceExtensions.cs` surface.

## Getting Started

### 1. Reference the library

Inside this repository, use a project reference:

```xml
<ItemGroup>
  <ProjectReference Include="..\AppBlueprint.UiKit\AppBlueprint.UiKit.csproj" />
</ItemGroup>
```

If you consume the packaged library externally, use the published NuGet package version that matches your target release.

### 2. Register UiKit services

At minimum:

```csharp
using AppBlueprint.UiKit;

builder.Services.AddUiKit();
```

If your app uses the built-in `Sidebar`, you also need to register an `IMenuConfigurationService` implementation:

```csharp
using AppBlueprint.UiKit.Services;

builder.Services.AddScoped<IMenuConfigurationService, YourMenuConfigurationService>();
builder.Services.AddUiKit();
```

### 3. Import the namespaces you use

For layout components:

```razor
@using AppBlueprint.UiKit.Components.Layout
@using AppBlueprint.UiKit.Components.Shared
```

Add additional namespaces only for the component areas you actually consume, for example:

- `AppBlueprint.UiKit.Components.Charts`
- `AppBlueprint.UiKit.Components.Dashboard`
- `AppBlueprint.UiKit.Components.Settings`

### 4. Use the layout components

The built-in UiKit main layout currently follows this pattern:

```razor
@inherits LayoutComponentBase

<div class="flex h-[100dvh] overflow-hidden bg-gray-100 dark:bg-gray-900">
    <Sidebar SidebarOpen="@sidebarOpen" OnCloseSidebar="@(() => sidebarOpen = false)" />

    <div class="content-area-bg relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden">
        <Header SidebarOpen="@sidebarOpen" OnToggleSidebar="@(() => sidebarOpen = !sidebarOpen)" />

        <main class="grow">
            @Body
        </main>
    </div>
</div>

@code {
    private bool sidebarOpen;
}
```

You can either reuse that structure directly or adapt it inside your host app's own layout.

## Menu Visibility and Navigation

`Sidebar.razor` injects `IMenuConfigurationService` and preloads visibility for a fixed set of menu item ids.

Current ids preloaded by the built-in sidebar are:

- `dashboard`
- `orders`
- `invoices`
- `shop2`
- `single-product`
- `cart`
- `cart2`
- `cart3`
- `pay`
- `shop`
- `customers`
- `community-users`
- `account`
- `notifications`
- `billing`
- `tasks`
- `job-board`
- `finance`

### Implement `IMenuConfigurationService`

Current interface:

```csharp
public interface IMenuConfigurationService
{
    Task<bool> ShouldShowMenuItemAsync(string menuItemId);
}
```

Minimal example:

```csharp
using AppBlueprint.UiKit.Services;

public sealed class YourMenuConfigurationService : IMenuConfigurationService
{
    public Task<bool> ShouldShowMenuItemAsync(string menuItemId)
    {
        return Task.FromResult(menuItemId switch
        {
            "dashboard" => true,
            "notifications" => true,
            "billing" => false,
            _ => false
        });
    }
}
```

### Recommended pattern

Keep the business rule in your app, not in UiKit components. Typical checks include:

- authenticated vs unauthenticated user
- tenant type
- product plan or subscription tier
- role membership
- feature flags

If your `ShouldShowMenuItemAsync(...)` implementation hits the database or external services, cache or preload the current user's permissions because the sidebar evaluates multiple ids during initialization.

## Route Protection with `TenantTypeAuthorize`

`TenantTypeAuthorize.razor` currently depends on:

- `AuthenticationStateProvider`
- `ICurrentTenantService`
- `NavigationManager`
- `ThemeService`

Parameters:

- `AllowedTenantTypes`
- `AllowUnauthenticated`
- `ChildContent`

### Basic usage

Allow authenticated users from any tenant type:

```razor
<TenantTypeAuthorize>
    <h1>Protected page</h1>
</TenantTypeAuthorize>
```

B2B-only page:

```razor
@using AppBlueprint.SharedKernel.Enums

<TenantTypeAuthorize AllowedTenantTypes="@(new List<TenantType> { TenantType.Organization })">
    <h1>Organization-only page</h1>
</TenantTypeAuthorize>
```

Demo-only page for unauthenticated users:

```razor
<TenantTypeAuthorize AllowUnauthenticated="true">
    <h1>Public demo page</h1>
</TenantTypeAuthorize>
```

### Current behavior

The component currently behaves like this:

- unauthenticated user + `AllowUnauthenticated=true` => content renders
- unauthenticated user + `AllowUnauthenticated=false` => redirects to `/signup`
- authenticated user + allowed tenant type => content renders
- authenticated user + disallowed tenant type => redirects to `/`
- authenticated user + `AllowUnauthenticated=true` with no allowed tenant types => redirects to `/`

Use both route protection and menu hiding together. Hiding a menu item is not a substitute for protecting the page itself.

## Theme System

### Register a theme from configuration

```csharp
using AppBlueprint.UiKit;

builder.Services.AddUiKitWithTheme(builder.Configuration);
```

Default section name:

```json
{
  "Theme": {
    "ApplicationType": "crm",
    "PrimaryColor": "blue",
    "AccentColor": "emerald",
    "DefaultPrimaryShade": "600",
    "DefaultAccentShade": "500",
    "BrandName": "SalesPro",
    "CustomLabels": {
      "analytics.topProducts": "Top Deals"
    }
  }
}
```

### Register a theme programmatically

```csharp
builder.Services.AddUiKitWithTheme(theme =>
{
    theme.ApplicationType = "dating";
    theme.PrimaryColor = "rose";
    theme.AccentColor = "pink";
    theme.DefaultPrimaryShade = "400";
    theme.DefaultAccentShade = "300";
    theme.BrandName = "LoveConnect";
    theme.CustomLabels["analytics.topProducts"] = "Top Matches";
});
```

### `ThemeConfiguration`

The current configuration model includes:

- `PrimaryColor`
- `AccentColor`
- `DefaultPrimaryShade`
- `DefaultAccentShade`
- `ApplicationType`
- `CustomLabels`
- `BrandName`
- `LogoUrl`
- `IconPack`
- `CustomClassOverrides`

Supported `ApplicationType` values documented in code are:

- `saas`
- `dating`
- `crm`
- `ecommerce`

### `ThemeService`

`ThemeService` is registered as a singleton and exposes:

- `CurrentTheme`
- `SetTheme(...)`
- `OnThemeChanged`
- `GetPrimaryClass(...)`
- `GetAccentClass(...)`
- `GetPrimaryHoverClass(...)`
- `GetPrimaryFocusClass(...)`
- `GetPrimaryDarkClass(...)`
- `GetLabel(...)`
- `GetLabelByType(...)`
- `GetClassesOrOverride(...)`
- `GetPrimaryButtonClasses()`
- `GetOutlineButtonClasses()`
- `GetBadgeClasses(...)`
- `GetLinkClasses()`
- `GetCardAccentClasses(...)`
- `GetPrimaryGradient(...)`
- `GetPrimaryGradientWithDark(...)`
- `GetPrimarySolidGradient(...)`
- `GetAccentGradient(...)`
- `GetRgbColor(...)`

### Theme usage in components

Inject `ThemeService`:

```razor
@inject ThemeService ThemeService
```

Generate Tailwind classes dynamically:

```razor
<button class="@ThemeService.GetPrimaryButtonClasses()">
    Save
</button>

<div class="@ThemeService.GetPrimaryClass("bg") @ThemeService.GetPrimaryClass("text", "50")">
    Themed panel
</div>

<a class="@ThemeService.GetLinkClasses()">
    Themed link
</a>
```

Use app-specific labels:

```razor
<h2>@ThemeService.GetLabelByType(
    saasLabel: "Top Products",
    datingLabel: "Top Matches",
    crmLabel: "Top Deals",
    ecommerceLabel: "Bestsellers")</h2>
```

### Runtime theme switching

If your component must react to runtime theme changes, subscribe to `OnThemeChanged`:

```razor
@inject ThemeService ThemeService
@implements IDisposable

@code {
    protected override void OnInitialized()
    {
        ThemeService.OnThemeChanged += StateHasChanged;
    }

    public void Dispose()
    {
        ThemeService.OnThemeChanged -= StateHasChanged;
    }
}
```

If your app never switches themes at runtime, this subscription is optional.

### Tailwind safelist note

`ThemeService` builds classes dynamically, for example `bg-violet-500` or `text-rose-400`.

If your consuming application compiles Tailwind itself, you need a safelist pattern that keeps those classes from being purged.

## Available Component Areas

The library currently includes components under these main areas:

- `Components/Layout`
- `Components/Shared`
- `Components/Charts`
- `Components/Dashboard`
- `Components/Settings`
- `Components/Pages`

Important distinction:

- Layout and shared primitives are the most reusable starting point.
- Many components under `Components/Pages` are fuller page examples and templates. Treat them as integration reference or starter material, not as business-complete production pages.

## Blog Module

UiKit also exposes blog registration helpers:

- `AddUiKitBlog(Action<AppBlueprintBlogOptions>? configureBlog = null)`
- `AddUiKitBlog<TContentService>(...) where TContentService : class, IBlogContentService`

Use those when you want the reusable blog module in addition to the core UiKit services.

## Best Practices

- Register `IMenuConfigurationService` explicitly if you use the built-in sidebar.
- Keep menu rules and authorization rules in your host app, not in UiKit component code.
- Protect routes as well as menus.
- Prefer `GetPrimaryClass(...)` and related helpers over hardcoded Tailwind brand colors in reusable components.
- Use `CustomLabels` and `GetLabelByType(...)` for app-specific wording instead of cloning components.
- Subscribe to `OnThemeChanged` only where runtime theme switching matters.
- Treat `Components/Pages` as reference implementations unless they match your product semantics directly.

## Troubleshooting

### Sidebar fails to render because of DI

Typical cause:

- `IMenuConfigurationService` was not registered

Fix:

```csharp
builder.Services.AddScoped<IMenuConfigurationService, YourMenuConfigurationService>();
builder.Services.AddUiKit();
```

### Theme classes do not apply

Typical causes:

- dynamic Tailwind classes were purged by the host app build
- configured color names do not match supported Tailwind palette names

Fix:

- add a Tailwind safelist for dynamic `bg-*`, `text-*`, `border-*`, and related classes
- use valid Tailwind colors such as `violet`, `blue`, `emerald`, `pink`, or `rose`

### Theme changes do not update visible components

Typical cause:

- the component never subscribed to `ThemeService.OnThemeChanged`

Fix:

- subscribe in `OnInitialized()`
- unsubscribe in `Dispose()`

### Route access does not match menu visibility

Typical cause:

- menu hiding was implemented, but the page itself is not protected

Fix:

- pair menu visibility rules with `TenantTypeAuthorize` or another route-level authorization mechanism

## Summary

For current consumers, the practical integration model is:

1. Register `AddUiKit()`
2. Register `IMenuConfigurationService` if you use `Sidebar`
3. Optionally register theme configuration through `AddUiKitWithTheme(...)`
4. Use `Sidebar`, `Header`, and related layout components
5. Use `TenantTypeAuthorize` for route protection
6. Use `ThemeService` helpers instead of hardcoded brand classes

This file is now the single source of truth for UiKit usage.

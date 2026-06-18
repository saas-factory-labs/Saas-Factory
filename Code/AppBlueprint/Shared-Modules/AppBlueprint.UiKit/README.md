# AppBlueprint.UiKit

[![NuGet Version](https://img.shields.io/nuget/v/SaaS-Factory.AppBlueprint.UiKit)](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.UiKit)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Reusable Blazor UI components, layouts, and theming utilities for AppBlueprint-style applications.

## Features

- Configuration-driven Tailwind theming via `ThemeService`
- Reusable layout components such as `MainLayout`, `Header`, and `Sidebar`
- Chart, dashboard, settings, and shared UI component areas
- Application-owned menu visibility through `IMenuConfigurationService`
- Tenant-aware route protection through `TenantTypeAuthorize`

## Quick Start

```bash
dotnet add package SaaS-Factory.AppBlueprint.UiKit --version 0.1.*
```

```csharp
// Program.cs
using AppBlueprint.UiKit;
using AppBlueprint.UiKit.Services;

builder.Services.AddScoped<IMenuConfigurationService, YourMenuConfigurationService>();
builder.Services.AddUiKit();
```

```razor
@* _Imports.razor *@
@using AppBlueprint.UiKit.Components.Layout
@using AppBlueprint.UiKit.Components.Shared

@* MainLayout.razor *@
<Sidebar SidebarOpen="@sidebarOpen" OnCloseSidebar="@(() => sidebarOpen = false)" />
<Header SidebarOpen="@sidebarOpen" OnToggleSidebar="@(() => sidebarOpen = !sidebarOpen)" />

<main>@Body</main>

@code {
    private bool sidebarOpen;
}
```

## Theme Customization

```csharp
// Load from configuration
builder.Services.AddUiKitWithTheme(builder.Configuration);

// Or customize programmatically
builder.Services.AddUiKitWithTheme(theme =>
{
    theme.ApplicationType = "crm";
    theme.PrimaryColor = "blue";
    theme.AccentColor = "emerald";
});
```

## Component Examples

```razor
@* Dashboard Card *@
<DashboardCard Title="Revenue" Body="Current month performance">
    <div class="text-sm text-gray-500">$24,563</div>
</DashboardCard>

@* Charts *@
<BarChart Labels="@labels" Datasets="@datasets" Height="300" />
```

## Documentation

- [Usage Guide](./USAGE-GUIDE.md) - Canonical guide for setup, menu integration, route protection, and theming

## Component Library

### Layout Components
`MainLayout`, `Header`, `Sidebar`, `NavMenu`, `BreadcrumbService`

### Dashboard Components
`DashboardCard`, `DashboardGrid`, `WelcomeBanner`

### Chart Components
`BarChart`, `LineChart`, `PieChart`, `DonutChart`, `DoughnutChart`, `TimeSeriesChart`, `StackedBarChart`

### Shared Utilities
`ThemeService`, `TenantTypeAuthorize`, `ThemeSwitcher`

## Requirements

- .NET 10.0
- Blazor application hosting the Razor class library
- Tailwind support in the consuming app if you compile and purge host-side utility classes dynamically

## License

MIT License - see [LICENSE](../../../../LICENSE)

## Links

- NuGet: https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.UiKit
- GitHub: https://github.com/saas-factory-labs/Saas-Factory
- Issues: https://github.com/saas-factory-labs/Saas-Factory/issues

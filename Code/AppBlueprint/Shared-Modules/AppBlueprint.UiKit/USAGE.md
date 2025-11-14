# AppBlueprint.UiKit - Complete Usage Guide

This guide covers installation, theme customization, component usage, and advanced configuration.

## Table of Contents
1. [Installation](#installation)
2. [Theme Customization](#theme-customization)
3. [Using Components](#using-components)
4. [Feature Flags](#feature-flags)
5. [Customization & Extension](#customization--extension)
6. [Component Library Reference](#component-library-reference)
7. [Configuration Reference](#configuration-reference)
8. [Troubleshooting](#troubleshooting)

## Installation

### 1. Install Packages
```bash
dotnet add package SaaS-Factory.AppBlueprint.UiKit --version 0.1.*
dotnet add package MudBlazor --version 8.14.0
```

### 2. Register Services
```csharp
// Program.cs
using AppBlueprint.UiKit;

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddUiKit();
```

### 3. Add Component Imports
```razor
@* Components/_Imports.razor *@
@using MudBlazor
@using AppBlueprint.UiKit
@using AppBlueprint.UiKit.Components
```

### 4. Configure Layout
```razor
@* Components/Layout/MainLayout.razor *@
@inherits LayoutComponentBase

<MudThemeProvider />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<NavigationMenu />
<main>
    @Body
</main>
```

## Theme Customization

### Option 1: Preset Themes
```csharp
builder.Services.AddUiKitWithPreset(ThemePreset.ProfessionalBlue);
```

Available: `Superhero`, `ProfessionalBlue`, `ModernDark`, `Minimal`

### Option 2: Fluent Theme Builder
```csharp
builder.Services.AddUiKitWithTheme(theme => theme
    .WithPrimaryColor("#1E40AF")
    .WithSecondaryColor("#10B981")
    .WithBorderRadius("8px"));
```

### Option 3: Advanced Configuration
```csharp
builder.Services.AddUiKit(options =>
{
    options.Theme = new ThemeBuilder()
        .UseProfessionalBluePreset()
        .WithPrimaryColor("#2563EB")
        .Build();
    
    options.Features.EnableCharts = false;
    options.Navigation.SidebarWidth = 280;
});
```

## Using Components

### Dashboard Cards
```razor
<DashboardCard Title="Total Revenue" Value="$24,563" Icon="@Icons.Material.Filled.AttachMoney" TrendPercentage=12.5 TrendDirection="up" />
```

### Charts
```razor
<BarChart Title="Monthly Sales" Data="@chartData" Height="300" />
<PieChart Title="Revenue by Category" Data="@pieData" Height="300" />
```

### Navigation
```razor
<NavigationMenu />

@inject NavigationService NavigationService
@code {
    protected override void OnInitialized()
    {
        NavigationService.AddRoute(new NavLinkMetadata { 
            Name = "Dashboard", 
            Href = "/dashboard", 
            Icon = Icons.Material.Filled.Dashboard 
        });
    }
}
```

## Feature Flags

Reduce bundle size by disabling unused features:

```csharp
builder.Services.AddUiKit(options =>
{
    options.Features.EnableCharts = false;           // Saves ~50KB
    options.Features.EnableAccountSettings = false;   // Saves ~30KB
    options.Features.EnableDashboard = false;
    options.Features.EnableThemeManager = false;      // Saves ~20KB
});
```

## Customization & Extension

### Override CSS
Create `wwwroot/css/custom.css`:
```css
.mud-card {
    border-radius: 12px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}
```

Load after UiKit styles:
```html
<link href="_content/AppBlueprint.UiKit/superherotheme-static.css" rel="stylesheet" />
<link href="css/custom.css" rel="stylesheet" />
```

### Extend Services
```csharp
builder.Services.AddUiKit(options =>
{
    options.ConfigureServices = services =>
    {
        services.AddScoped<NavigationService, CustomNavigationService>();
    };
});
```

## Component Library Reference

### Layout
- `NavigationMenu` - Sidebar navigation
- `Sidebar` - Responsive drawer
- `BreadcrumbService` - Breadcrumb tracking

### Dashboard
- `DashboardCard` - Metric cards with trends
- `DashboardCardLineChart` - Card with inline chart
- `DashboardGrid` - Responsive grid

### Charts
- `BarChart`, `LineChart`, `PieChart`, `DonutChart`
- `TimeSeriesChart`, `HeatMapChart`, `StackedBarChart`

### Account Settings
- `MyAccount`, `MyNotifications`, `MyConnectedApps`
- `Plans`, `BillingInvoices`, `Feedback`

## Configuration Reference

```csharp
public class UiKitOptions
{
    public MudTheme? Theme { get; set; }
    public Action<IServiceCollection>? ConfigureServices { get; set; }
    public UiKitFeatures Features { get; set; }
    public NavigationOptions Navigation { get; set; }
}

public class UiKitFeatures
{
    public bool EnableCharts { get; set; } = true;
    public bool EnableAccountSettings { get; set; } = true;
    public bool EnableDashboard { get; set; } = true;
    public bool EnableThemeManager { get; set; } = false;
}

public class NavigationOptions
{
    public bool CollapseSidebarOnMobile { get; set; } = true;
    public bool EnableBreadcrumbs { get; set; } = true;
    public int SidebarWidth { get; set; } = 240;
    public int MiniSidebarWidth { get; set; } = 56;
}
```

## Troubleshooting

### Components not styling correctly
Ensure MudBlazor CSS/JS are loaded:
```html
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
```

### Custom theme not applying
Register theme as singleton:
```csharp
builder.Services.AddSingleton(yourTheme);
```

Use in layout:
```razor
@inject MudTheme Theme
<MudThemeProvider Theme="@Theme" />
```

### Navigation menu empty
Register routes in your layout/page:
```razor
@inject NavigationService NavigationService
@code {
    protected override void OnInitialized()
    {
        NavigationService.AddRoute(new NavLinkMetadata { ... });
    }
}
```

## Links

- **NuGet:** https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.UiKit
- **GitHub:** https://github.com/saas-factory-labs/Saas-Factory
- **MudBlazor:** https://mudblazor.com/

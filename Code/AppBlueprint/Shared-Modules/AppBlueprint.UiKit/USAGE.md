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
```

### 2. Register Services
```csharp
// Program.cs
using AppBlueprint.UiKit;

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddUiKit();
```

### 3. Add Component Imports
```razor
@* Components/_Imports.razor *@
@using AppBlueprint.UiKit
@using AppBlueprint.UiKit.Components
```

### 4. Configure Layout
```razor
@* Components/Layout/MainLayout.razor *@
@inherits LayoutComponentBase

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
<MetricCard Title="Total Revenue" Value="$24,563" Icon="heroicons-outline:currency-dollar" TrendPercentage=12.5 TrendDirection="up" />
```

### Charts
```razor
<BarChartCard Title="Monthly Sales" Data="@chartData" Height="300" />
<DoughnutChartCard Title="Revenue by Category" Data="@pieData" Height="300" />
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
            Icon = "heroicons-outline:home" 
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
.card {
    border-radius: 12px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}
```

Load after UiKit styles:
```html
<link href="_content/AppBlueprint.UiKit/css/app.css" rel="stylesheet" />
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
- `MetricCard` - Metric cards with trends
- `LineChartCard` - Card with inline chart
- `DashboardGrid` - Responsive grid

### Charts
- `BarChartCard`, `LineChartCard`, `DoughnutChartCard`

### Account Settings
- `MyAccount`, `MyNotifications`, `MyConnectedApps`
- `Plans`, `BillingInvoices`, `Feedback`

## Configuration Reference

```csharp
public class UiKitOptions
{
    public ThemeConfiguration? Theme { get; set; }
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
Ensure Tailwind CSS is compiled and loaded:
```html
<link href="css/app.css" rel="stylesheet" />
```

### Custom theme not applying
Register theme as singleton:
```csharp
builder.Services.AddSingleton(yourTheme);
```

Use in layout:
```razor
@inject ThemeService ThemeService
<div class="@ThemeService.GetThemeClasses()">
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
- **Tailwind CSS:** https://tailwindcss.com/

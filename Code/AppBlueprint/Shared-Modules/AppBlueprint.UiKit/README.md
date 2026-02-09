# AppBlueprint.UiKit

[![NuGet Version](https://img.shields.io/nuget/v/SaaS-Factory.AppBlueprint.UiKit)](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.UiKit)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A comprehensive Blazor Razor Class Library providing reusable UI components, themes, and layouts built with MudBlazor for rapid SaaS application development.

## Features

- 🎨 **Customizable Themes** - Built-in themes + fluent ThemeBuilder API
- 🧩 **Rich Component Library** - Dashboard cards, charts, forms, navigation
- 📱 **Responsive Layouts** - Mobile-first design
- 🎯 **Type-Safe Configuration** - Strongly-typed options
- ⚡ **Performance Optimized** - Feature flags to reduce bundle size
- 🔧 **Extensible** - Easy to override and extend

## Quick Start

```bash
dotnet add package SaaS-Factory.AppBlueprint.UiKit --version 0.1.*
dotnet add package MudBlazor --version 8.14.0
```

```csharp
// Program.cs
builder.Services.AddMudServices();
builder.Services.AddUiKit();
```

```razor
@* _Imports.razor *@
@using AppBlueprint.UiKit.Components

@* MainLayout.razor *@
<MudThemeProvider />
<NavigationMenu />
<main>@Body</main>
```

## Theme Customization

```csharp
// Use preset themes
builder.Services.AddUiKitWithPreset(ThemePreset.ProfessionalBlue);

// Or customize with ThemeBuilder
builder.Services.AddUiKitWithTheme(theme => theme
    .WithPrimaryColor("#1E40AF")
    .WithSecondaryColor("#10B981")
    .WithBorderRadius("8px"));

// Or advanced configuration
builder.Services.AddUiKit(options =>
{
    options.Theme = new ThemeBuilder()
        .UseProfessionalBluePreset()
        .WithPrimaryColor("#2563EB")
        .Build();
    
    options.Features.EnableCharts = false; // Reduce bundle size
    options.Navigation.SidebarWidth = 280;
});
```

## Component Examples

```razor
@* Dashboard Card *@
<DashboardCard 
    Title="Total Revenue" 
    Value="$24,563" 
    TrendPercentage=12.5 
    TrendDirection="up" />

@* Charts *@
<BarChart Title="Monthly Sales" Data="@chartData" Height="300" />
<PieChart Title="Revenue" Data="@pieData" Height="300" />

@* Navigation *@
<NavigationMenu />
```

## Documentation

- **[Complete Usage Guide](./USAGE.md)** - Installation, configuration, components
- **[Architecture & Integration Guide](./USAGE-GUIDE.md)** - IMenuConfigurationService implementation
- **[Theme Usage Guide](./THEME-USAGE-GUIDE.md)** - Theme system and customization

## Component Library

### Layout Components
`NavigationMenu`, `Sidebar`, `BreadcrumbService`

### Dashboard Components  
`DashboardCard`, `DashboardCardLineChart`, `DashboardGrid`

### Chart Components
`BarChart`, `LineChart`, `PieChart`, `DonutChart`, `TimeSeriesChart`, `HeatMapChart`, `StackedBarChart`

### Account Settings
`MyAccount`, `MyNotifications`, `MyConnectedApps`, `Plans`, `BillingInvoices`, `Feedback`

## Requirements

- .NET 10.0
- MudBlazor 8.14.0+
- Blazor Server or Blazor WebAssembly

## License

MIT License - see [LICENSE](../../../../LICENSE)

## Links

- **NuGet:** https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.UiKit  
- **GitHub:** https://github.com/saas-factory-labs/Saas-Factory
- **Issues:** https://github.com/saas-factory-labs/Saas-Factory/issues
- **MudBlazor:** https://mudblazor.com/

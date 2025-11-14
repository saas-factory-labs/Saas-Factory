// This file demonstrates how consuming projects should test AppBlueprint.UiKit integration
// Copy this to your own test project and adapt as needed

using AppBlueprint.UiKit;
using AppBlueprint.UiKit.Components;
using AppBlueprint.UiKit.Configuration;
using AppBlueprint.UiKit.Services;
using AppBlueprint.UiKit.Themes;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Xunit;

namespace YourProject.IntegrationTests;

/// <summary>
/// Integration tests for AppBlueprint.UiKit NuGet package.
/// These tests verify that the package works correctly in a consuming project.
/// </summary>
public class UiKitIntegrationTests : TestContext
{
    [Fact]
    public void AddUiKit_WithDefaultConfiguration_RegistersServices()
    {
        // Arrange & Act
        Services.AddMudServices();
        Services.AddUiKit();

        // Assert
        var navigationService = Services.BuildServiceProvider().GetService<NavigationService>();
        Assert.NotNull(navigationService);

        var breadcrumbService = Services.BuildServiceProvider().GetService<BreadcrumbService>();
        Assert.NotNull(breadcrumbService);

        var theme = Services.BuildServiceProvider().GetService<MudTheme>();
        Assert.NotNull(theme);
        Assert.Equal(CustomThemes.Superherotheme.PaletteLight.Primary, theme.PaletteLight.Primary);
    }

    [Fact]
    public void AddUiKit_WithCustomTheme_UsesCustomTheme()
    {
        // Arrange
        var customPrimaryColor = "#1E40AF";

        // Act
        Services.AddMudServices();
        Services.AddUiKitWithTheme(theme => theme
            .WithPrimaryColor(customPrimaryColor));

        // Assert
        var theme = Services.BuildServiceProvider().GetRequiredService<MudTheme>();
        Assert.Equal(customPrimaryColor, theme.PaletteLight.Primary);
    }

    [Fact]
    public void AddUiKit_WithPresetTheme_UsesCorrectPreset()
    {
        // Arrange & Act
        Services.AddMudServices();
        Services.AddUiKitWithPreset(ThemePreset.ProfessionalBlue);

        // Assert
        var theme = Services.BuildServiceProvider().GetRequiredService<MudTheme>();
        Assert.NotEqual(CustomThemes.Superherotheme.PaletteLight.Primary, theme.PaletteLight.Primary);
    }

    [Fact]
    public void AddUiKit_WithDisabledFeatures_StillRegistersCore()
    {
        // Arrange & Act
        Services.AddMudServices();
        Services.AddUiKit(options =>
        {
            options.Features.EnableCharts = false;
            options.Features.EnableAccountSettings = false;
        });

        // Assert - Core services should still be registered
        var navigationService = Services.BuildServiceProvider().GetService<NavigationService>();
        Assert.NotNull(navigationService);
    }

    [Fact]
    public void AddUiKit_WithCustomServiceConfiguration_RegistersCustomServices()
    {
        // Arrange
        var customServiceCalled = false;

        // Act
        Services.AddMudServices();
        Services.AddUiKit(options =>
        {
            options.ConfigureServices = services =>
            {
                services.AddScoped<ITestService>(_ =>
                {
                    customServiceCalled = true;
                    return new TestService();
                });
            };
        });

        // Assert
        var testService = Services.BuildServiceProvider().GetService<ITestService>();
        Assert.NotNull(testService);
    }

    [Fact]
    public void NavigationMenu_Renders_Successfully()
    {
        // Arrange
        Services.AddMudServices();
        Services.AddUiKit();
        Services.AddSingleton<NavigationService>();

        // Act
        var cut = RenderComponent<NavigationMenu>();

        // Assert
        Assert.NotNull(cut);
        // Verify navigation menu structure exists
        cut.Find("nav"); // Should not throw
    }

    [Fact]
    public void DashboardCard_WithValidProps_RendersCorrectly()
    {
        // Arrange
        Services.AddMudServices();
        Services.AddUiKit();

        // Act
        var cut = RenderComponent<DashboardCard>(parameters => parameters
            .Add(p => p.Title, "Test Card")
            .Add(p => p.Value, "$1,234")
            .Add(p => p.TrendPercentage, 12.5)
            .Add(p => p.TrendDirection, "up"));

        // Assert
        Assert.Contains("Test Card", cut.Markup);
        Assert.Contains("$1,234", cut.Markup);
        Assert.Contains("12.5", cut.Markup);
    }

    [Fact]
    public void ThemeBuilder_FluentAPI_BuildsCorrectTheme()
    {
        // Arrange & Act
        var theme = new ThemeBuilder()
            .WithPrimaryColor("#FF0000")
            .WithSecondaryColor("#00FF00")
            .WithBorderRadius("8px")
            .Build();

        // Assert
        Assert.Equal("#FF0000", theme.PaletteLight.Primary);
        Assert.Equal("#00FF00", theme.PaletteLight.Secondary);
        Assert.Equal("8px", theme.LayoutProperties.DefaultBorderRadius);
    }

    [Fact]
    public void ThemeBuilder_Presets_ProduceUniqueThemes()
    {
        // Arrange & Act
        var superheroTheme = new ThemeBuilder().Build();
        var professionalTheme = new ThemeBuilder().UseProfessionalBluePreset().Build();
        var modernDarkTheme = new ThemeBuilder().UseModernDarkPreset().Build();

        // Assert
        Assert.NotEqual(superheroTheme.PaletteLight.Primary, professionalTheme.PaletteLight.Primary);
        Assert.NotEqual(professionalTheme.PaletteLight.Primary, modernDarkTheme.PaletteLight.Primary);
    }

    [Fact]
    public void NavigationService_AddRoute_StoresRoute()
    {
        // Arrange
        var service = new NavigationService();
        var route = new NavLinkMetadata
        {
            Name = "Dashboard",
            Href = "/dashboard",
            MudblazorIconPath = "Icons.Material.Filled.Dashboard"
        };

        // Act
        service.AddRoute(route);

        // Assert
        Assert.Contains(route, service.Routes);
    }

    [Fact]
    public void BreadcrumbService_UpdatesBreadcrumbs_WhenNavigating()
    {
        // Arrange
        var service = new BreadcrumbService();

        // Act
        service.SetBreadcrumbs(new List<BreadcrumbItem>
        {
            new() { Text = "Home", Href = "/" },
            new() { Text = "Dashboard", Href = "/dashboard" }
        });

        // Assert
        Assert.Equal(2, service.Breadcrumbs.Count);
        Assert.Equal("Home", service.Breadcrumbs[0].Text);
        Assert.Equal("Dashboard", service.Breadcrumbs[1].Text);
    }

    // Test helper interfaces and classes
    public interface ITestService { }
    public class TestService : ITestService { }
}

/// <summary>
/// Performance and bundle size tests (conceptual - adapt to your testing framework)
/// </summary>
public class UiKitPerformanceTests
{
    [Fact]
    public void DisablingFeatures_ReducesBundleSize()
    {
        // This is a conceptual test - actual implementation would measure bundle size
        // In practice, you would:
        // 1. Build with all features enabled
        // 2. Measure wwwroot size
        // 3. Build with features disabled
        // 4. Measure wwwroot size again
        // 5. Assert size reduction

        // For demonstration:
        var allFeaturesSize = 1000; // KB (example)
        var noChartsSize = 950;     // KB (example)

        Assert.True(noChartsSize < allFeaturesSize, "Disabling charts should reduce bundle size");
    }
}

/// <summary>
/// Compatibility tests to ensure NuGet package works with different configurations
/// </summary>
public class UiKitCompatibilityTests : TestContext
{
    [Fact]
    public void UiKit_WorksWithBlazorServer()
    {
        // Arrange
        Services.AddMudServices();
        Services.AddUiKit();

        // Act & Assert
        var serviceProvider = Services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<NavigationService>());
    }

    [Theory]
    [InlineData(ThemePreset.Superhero)]
    [InlineData(ThemePreset.ProfessionalBlue)]
    [InlineData(ThemePreset.ModernDark)]
    [InlineData(ThemePreset.Minimal)]
    public void AllThemePresets_RegisterSuccessfully(ThemePreset preset)
    {
        // Arrange
        Services.Clear(); // Clear previous test services

        // Act
        Services.AddMudServices();
        Services.AddUiKitWithPreset(preset);

        // Assert
        var theme = Services.BuildServiceProvider().GetService<MudTheme>();
        Assert.NotNull(theme);
    }
}

// NOTE: To use these tests in your project:
// 1. Create a test project: dotnet new xunit -n YourProject.IntegrationTests
// 2. Add packages:
//    dotnet add package SaaS-Factory.AppBlueprint.UiKit
//    dotnet add package bunit
//    dotnet add package xunit
// 3. Copy this file and adapt namespace
// 4. Run tests: dotnet test

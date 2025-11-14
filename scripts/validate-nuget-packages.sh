#!/bin/bash
set -e

# NuGet Package Validation Script
# This script validates that AppBlueprint NuGet packages can be consumed correctly
# Run this before publishing packages to NuGet.org

echo "=========================================="
echo "AppBlueprint NuGet Package Validator"
echo "=========================================="
echo ""

# Configuration
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
TEST_DIR="$REPO_ROOT/temp-nuget-validation"
PACKAGES_DIR="$REPO_ROOT/artifacts/packages"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Helper functions
log_info() {
    echo -e "${GREEN}✓${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}⚠${NC} $1"
}

log_error() {
    echo -e "${RED}✗${NC} $1"
}

cleanup() {
    if [ -d "$TEST_DIR" ]; then
        log_info "Cleaning up temporary test directory..."
        rm -rf "$TEST_DIR"
    fi
}

# Trap to ensure cleanup on exit
trap cleanup EXIT

# Step 1: Build packages
echo "Step 1: Building NuGet packages..."
echo "-----------------------------------"

cd "$REPO_ROOT"

if [ ! -d "$PACKAGES_DIR" ]; then
    log_warn "Packages directory not found. Building packages..."
    dotnet pack --configuration Release --output "$PACKAGES_DIR"
else
    log_info "Using existing packages in $PACKAGES_DIR"
fi

# Count packages
PACKAGE_COUNT=$(find "$PACKAGES_DIR" -name "SaaS-Factory.AppBlueprint.*.nupkg" ! -name "*.symbols.nupkg" | wc -l)
log_info "Found $PACKAGE_COUNT NuGet packages"

if [ "$PACKAGE_COUNT" -eq 0 ]; then
    log_error "No packages found. Build failed?"
    exit 1
fi

echo ""

# Step 2: Create temporary test project
echo "Step 2: Creating temporary test project..."
echo "-------------------------------------------"

cleanup # Remove any existing test directory
mkdir -p "$TEST_DIR"
cd "$TEST_DIR"

# Create Blazor Server test project
dotnet new blazorserver-empty -n TestConsumer --framework net10.0
cd TestConsumer

log_info "Created Blazor Server test project"
echo ""

# Step 3: Add package references
echo "Step 3: Adding package references..."
echo "-------------------------------------"

# Add local package source
dotnet nuget add source "$PACKAGES_DIR" --name LocalPackages

# Find the latest version of UiKit package
UIKIT_PACKAGE=$(find "$PACKAGES_DIR" -name "SaaS-Factory.AppBlueprint.UiKit.*.nupkg" ! -name "*.symbols.nupkg" | sort -V | tail -1)
UIKIT_VERSION=$(basename "$UIKIT_PACKAGE" | sed -E 's/SaaS-Factory\.AppBlueprint\.UiKit\.([0-9]+\.[0-9]+\.[0-9]+(-[a-z0-9.]+)?)\.nupkg/\1/')

log_info "Installing AppBlueprint.UiKit version $UIKIT_VERSION"

# Add packages
dotnet add package SaaS-Factory.AppBlueprint.UiKit --version "$UIKIT_VERSION" --source "$PACKAGES_DIR"
dotnet add package MudBlazor --version 8.14.0

log_info "Packages added successfully"
echo ""

# Step 4: Configure test project
echo "Step 4: Configuring test project..."
echo "------------------------------------"

# Update Program.cs to use UiKit
cat > Program.cs << 'EOF'
using AppBlueprint.UiKit;
using AppBlueprint.UiKit.Configuration;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddUiKit(options =>
{
    options.Features.EnableCharts = true;
    options.Navigation.SidebarWidth = 280;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public partial class Program { }
EOF

# Update _Imports.razor
cat > Components/_Imports.razor << 'EOF'
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using TestConsumer
@using TestConsumer.Components
@using MudBlazor
@using AppBlueprint.UiKit.Components
@using AppBlueprint.UiKit.Services
EOF

# Create a test page using UiKit components
mkdir -p Components/Pages
cat > Components/Pages/Dashboard.razor << 'EOF'
@page "/dashboard"
@using AppBlueprint.UiKit.Components

<PageTitle>Dashboard - Test</PageTitle>

<MudContainer MaxWidth="MaxWidth.Large" Class="mt-4">
    <MudText Typo="Typo.h4" Class="mb-4">Dashboard Test</MudText>

    <MudGrid>
        <MudItem xs="12" sm="6" md="3">
            <DashboardCard
                Title="Total Users"
                Value="1,234"
                TrendPercentage="12.5"
                TrendDirection="up" />
        </MudItem>

        <MudItem xs="12" sm="6" md="3">
            <DashboardCard
                Title="Revenue"
                Value="$45,678"
                TrendPercentage="8.3"
                TrendDirection="up" />
        </MudItem>
    </MudGrid>
</MudContainer>
EOF

log_info "Test project configured"
echo ""

# Step 5: Build test project
echo "Step 5: Building test project..."
echo "---------------------------------"

dotnet build --configuration Release

if [ $? -eq 0 ]; then
    log_info "Build successful ✓"
else
    log_error "Build failed ✗"
    exit 1
fi

echo ""

# Step 6: Create and run integration tests
echo "Step 6: Running integration tests..."
echo "-------------------------------------"

cd "$TEST_DIR"

# Create test project
dotnet new xunit -n IntegrationTests --framework net10.0
cd IntegrationTests

# Add references
dotnet add package SaaS-Factory.AppBlueprint.UiKit --version "$UIKIT_VERSION" --source "$PACKAGES_DIR"
dotnet add package MudBlazor --version 8.14.0
dotnet add package bunit --version 1.32.7
dotnet add reference "$TEST_DIR/TestConsumer/TestConsumer.csproj"

# Create test file
cat > UiKitIntegrationTests.cs << 'EOF'
using AppBlueprint.UiKit;
using AppBlueprint.UiKit.Configuration;
using AppBlueprint.UiKit.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Xunit;

namespace IntegrationTests;

public class UiKitIntegrationTests : TestContext
{
    [Fact]
    public void AddUiKit_RegistersServicesCorrectly()
    {
        // Arrange & Act
        Services.AddMudServices();
        Services.AddUiKit();

        // Assert
        var serviceProvider = Services.BuildServiceProvider();
        var navigationService = serviceProvider.GetService<NavigationService>();
        var breadcrumbService = serviceProvider.GetService<BreadcrumbService>();
        var theme = serviceProvider.GetService<MudTheme>();

        Assert.NotNull(navigationService);
        Assert.NotNull(breadcrumbService);
        Assert.NotNull(theme);
    }

    [Fact]
    public void AddUiKit_WithOptions_AppliesConfiguration()
    {
        // Arrange & Act
        Services.AddMudServices();
        Services.AddUiKit(options =>
        {
            options.Features.EnableCharts = false;
            options.Navigation.SidebarWidth = 300;
        });

        // Assert
        var serviceProvider = Services.BuildServiceProvider();
        var navOptions = serviceProvider.GetService<NavigationOptions>();

        Assert.NotNull(navOptions);
        Assert.Equal(300, navOptions.SidebarWidth);
    }
}
EOF

# Run tests
dotnet test --configuration Release

if [ $? -eq 0 ]; then
    log_info "Integration tests passed ✓"
else
    log_error "Integration tests failed ✗"
    exit 1
fi

echo ""

# Step 7: Validation summary
echo "=========================================="
echo "Validation Summary"
echo "=========================================="
echo ""

log_info "Package build: SUCCESS"
log_info "Package installation: SUCCESS"
log_info "Project build: SUCCESS"
log_info "Integration tests: SUCCESS"

echo ""
echo -e "${GREEN}✓ All validation checks passed!${NC}"
echo ""
echo "Packages are ready for publishing."
echo ""

# Optional: Show package details
echo "Package Details:"
echo "----------------"
find "$PACKAGES_DIR" -name "SaaS-Factory.AppBlueprint.*.nupkg" ! -name "*.symbols.nupkg" -exec basename {} \; | sort

echo ""
echo "To publish to NuGet.org, run:"
echo "  dotnet nuget push \"$PACKAGES_DIR/SaaS-Factory.AppBlueprint.*.nupkg\" --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY"
echo ""

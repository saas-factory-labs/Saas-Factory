---
title: Quick start
---

Get up and running in three simple steps:

1. **Install Prerequisites** - Set up your development environment
2. **Run the Application** - Start the AppHost with .NET Aspire
3. **Start Developing** - Access Swagger UI and begin building features

## Prerequisites

### Windows Development Setup

#### Step 1: Install WSL (Windows Subsystem for Linux)

Open PowerShell as Administrator and run:

```bash
wsl --install
```

**Important:** Restart your computer after installation.

#### Step 2: Install Required Development Tools

Run the following command in PowerShell (as Administrator):

```bash
@(
    "Microsoft.DotNet.SDK.9",
    "Git.Git",
    "Docker.DockerDesktop",
    "OpenJS.NodeJS",
    "GitHub.cli"
) | ForEach-Object { winget install --accept-package-agreements --accept-source-agreements --id $_ }
```

This will use Winget to install:
- **.NET 10 SDK** - Runtime and development tools
- **Git** - Version control
- **Docker Desktop** - Container runtime
- **Node.js** - JavaScript runtime for tooling
- **GitHub CLI** - GitHub integration

#### Step 3: Verify Installation

```bash
dotnet --version
git --version
docker --version
node --version
gh --version
```

### Recommended Optional Tools

```bash
@(
    "Microsoft.VisualStudioCode",
    "JetBrains.Rider",
    "Postman.Postman",
) | ForEach-Object { winget install --accept-package-agreements --accept-source-agreements --id $_ }
```

## Port Mappings

### Development Environment

| Service | HTTP | HTTPS | Purpose |
|---------|------|-------|---------|
| **AppHost** | - | https://localhost:17298 | .NET Aspire Dashboard |
| **Web (Blazor)** | http://localhost:8082 | https://localhost:8083 | Frontend Application |
| **API** | http://localhost:8080/api | https://localhost:8081/api | REST API Backend |
| **Gateway** | http://localhost:8084/gw/health | https://localhost:8085/gw/health | YARP Reverse Proxy |

### Port Configuration Precedence

ASP.NET Core determines port mappings in the following order (highest to lowest priority):

1. **Command-line arguments**
   ```bash
   dotnet run --urls "http://localhost:5000"
   ```

2. **Code-level configuration in Program.cs **
   ```csharp
   builder.WebHost.ConfigureKestrel(options => { ... })
   ```

3. **Launch Settings / Environment Variables**
   ```jsonc
   // launchSettings.json
   "environmentVariables": {
     "ASPNETCORE_URLS": "https://localhost:8081"
   }
   ```

4. **App Settings Files**
   ```jsonc
   // appsettings.json
   "Kestrel": {
     "Endpoints": { ... }
   }
   ```

## Running the Application

```bash
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.AppHost
dotnet run
```

The .NET Aspire dashboard will open automatically at `https://localhost:17298` and display the following services: Web, API, Gateway

### Individual Project Development

```bash
# Run API only
cd AppBlueprint.ApiService
dotnet watch

# Run Web only
cd AppBlueprint.Web
dotnet watch

# Run with hot reload
dotnet watch
```

## API Documentation

### Swagger UI

Access the interactive API documentation at:

**URL:** `https://localhost:8081/index.html`

![swagger-ui.png](/img/swagger-ui.png)

### OpenAPI Specification

Download the raw OpenAPI/Swagger JSON:

**URL:** `https://localhost:8081/swagger/v1/swagger.json`

![swagger-json.png](/img/swagger-json.png)

### Generating API Clients

The project uses **Kiota** to generate strongly-typed API clients:

```bash
# Regenerate API client SDK
cd Shared-Modules\AppBlueprint.Api.Client.Sdk
dotnet build
```

## Database Management

### Entity Framework Core Migrations

All commands should be executed from the `AppBlueprint.Infrastructure` project directory since the DbContext resides there.

```bash
cd Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

### Common EF Core Commands

#### View Database Context Information

```bash
dotnet ef dbcontext info --context "ApplicationDBContext"
```

#### Create a New Migration

```bash
dotnet ef migrations add <name> --context ApplicationDBContext
```

#### Apply Migrations to Database

```bash
dotnet ef database update --context ApplicationDBContext
```

#### Remove Last Migration

```bash
dotnet ef migrations remove --context ApplicationDBContext
```

#### Generate SQL Script

```bash
dotnet ef migrations script --context ApplicationDBContext --output migration.sql
```

### Database Connection Strings

Connection strings are configured via environment variables or `appsettings.json`:

```bash
DATABASE_CONNECTION_STRING="Host=localhost;Database=appblueprint;Username=postgres;Password=your_password"
```

## Key Configuration Files

#### Directory.Packages.props

Central package management for all C# projects. Packages are referenced in each C# project's `.csproj` file without a version, which is sourced from the `Directory.Packages.props` file:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" />
```

**Location:** `Code/AppBlueprint/Directory.Packages.props`

#### Directory.Build.props

Defines common MSBuild properties for all projects in the solution.

**Location:** `Code/AppBlueprint/Directory.Build.props`

```xml
<!-- This file is used to define the common properties for all projects in the solution -->
<Project>
    <PropertyGroup>
        <Targetframework>net9.0</Targetframework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <AnalysisLevel>latest</AnalysisLevel>
        <AnalysisMode>All</AnalysisMode>
    </PropertyGroup>
</Project>
```

#### NuGet.Config

Central NuGet package source configuration, including GitHub Packages integration.

**Location:** `Code/AppBlueprint/NuGet.Config`

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
        <add key="saas-factory-labs-github" value="https://nuget.pkg.github.com/saas-factory-labs/index.json" />
    </packageSources>
</configuration>
```

### Package Versioning (NuGet, Docker)

#### Automatic Versioning with GitVersion

Set this property in the `.csproj` file of a C# project to enable automatic versioning using the GitVersion tool:

```xml
<PropertyGroup>
    <Version>$(GITVERSION_SemVer)</Version>
</PropertyGroup>
```

## Docker Configuration

### Building Docker Images

Build a Docker image from the correct build context:

```bash
cd Code/AppBlueprint
docker build -f AppBlueprint.Web/Dockerfile . -t appblueprint-web:dev
```

> **Important:** The build context must be set to `Code/AppBlueprint` directory to ensure access to `Directory.Packages.props`, `NuGet.Config`, and dependent projects.

### GitHub Container Registry Login

To pull/push images from GitHub Container Registry (ghcr.io) on a VM or Coolify:

```bash
docker login ghcr.io -u saas-factory-labs
# Enter your GitHub Personal Access Token (PAT) when prompted
```

## Development Tools

### Running GitHub Actions Locally with Act

[Act](https://github.com/nektos/act) allows you to test GitHub Actions workflows locally using Docker.

#### Install Act

```bash
winget install nektos.act
```

#### Run All Workflows

```bash
act -P ubuntu-latest=ghcr.io/catthehacker/ubuntu:act-latest
```

#### Run Specific Workflow

```bash
act -P ubuntu-latest=ghcr.io/catthehacker/ubuntu:act-latest -j publish-packages
```

### Upgrading .NET Aspire

To upgrade to the latest .NET Aspire version:

#### Install Upgrade Assistant

```bash
dotnet tool install -g upgrade-assistant
```

#### Run Upgrade

```bash
upgrade-assistant upgrade
```

**Reference:** [Microsoft .NET Aspire Upgrade Guide](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/upgrade-to-aspire-9?pivots=dotnet-cli)

## Best Practices

### Null Reference Validation

Follow these patterns for null checking in the codebase:

```csharp
// ✅ Preferred: Throw if null with inline check
var user = await _dbContext.GetUser(userId) 
    ?? throw new ArgumentNullException(nameof(userId));

// ✅ Pattern: Explicit null check with is null
if (user is null)
{
    throw new ArgumentNullException(nameof(user));
}

// ✅ Pattern: Explicit not null check
if (user is not null)
{
    // Process user
}
```

### Recommended Packages

#### FluentRegex
Create human-readable regular expressions:

```csharp
// Instead of: @"^\d{3}-\d{2}-\d{4}$"
var pattern = Pattern.With
    .StartOfString()
    .Digit().Repeat.Exactly(3)
    .Literal("-")
    .Digit().Repeat.Exactly(2)
    .Literal("-")
    .Digit().Repeat.Exactly(4)
    .EndOfString()
    .ToString();
```

**Repository:** https://github.com/bcwood/FluentRegex

## Troubleshooting

### Common Issues

#### Port Already in Use

```bash
# Find process using port 8081
netstat -ano | findstr :8081

# Kill the process (replace <PID> with actual process ID)
taskkill /PID <PID> /F
```

#### Database Connection Errors

1. Verify PostgreSQL is running in Docker
2. Check connection string environment variables
3. Ensure migrations are applied: `dotnet ef database update --context ApplicationDBContext`

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Clean Architecture Guide](https://github.com/jasontaylordev/CleanArchitecture)
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)

## Getting Help

- **GitHub Issues:** [Report a bug or request a feature](https://github.com/saas-factory-labs/Saas-Factory/issues)
- **GitHub Discussions:** [Ask questions and share ideas](https://github.com/saas-factory-labs/Saas-Factory/discussions)


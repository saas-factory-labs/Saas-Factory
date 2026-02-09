
Development workflow

# Development Workflow

_Complete guide to setup, develop, and maintain the AppBlueprint project_

## Quick Start

Get up and running in three simple steps:

1. **Install Prerequisites** - Set up your development environment
2. **Run the Application** - Start the AppHost with .NET Aspire
3. **Start Developing** - Access Swagger UI and begin building features

## Prerequisites

### Windows Development Setup

#### Step 1: Install WSL (Windows Subsystem for Linux)

Open PowerShell as Administrator and run:

```powershell
wsl --install
```

**Important:** Restart your computer after installation.

#### Step 2: Install Required Development Tools

Run the following command in PowerShell (as Administrator):

```powershell
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

```powershell
dotnet --version
git --version
docker --version
node --version
gh --version
```

### Recommended Optional Tools

```powershell
@(
    "Microsoft.VisualStudioCode",
    "JetBrains.Rider",
    "Postman.Postman",
) | ForEach-Object { winget install --accept-package-agreements --accept-source-agreements --id $_ }
```

## Port Mappings

### Development Environment

<table>
    <tr>
        <th>Service</th>
        <th>HTTP</th>
        <th>HTTPS</th>
        <th>Purpose</th>
    </tr>
    <tr>
        <td><strong>AppHost</strong></td>
        <td>-</td>
        <td>https://localhost:17298</td>
        <td>.NET Aspire Dashboard</td>
    </tr>
    <tr>
        <td><strong>Web (Blazor)</strong></td>
        <td>http://localhost:8082</td>
        <td>https://localhost:8083</td>
        <td>Frontend Application</td>
    </tr>
    <tr>
        <td><strong>API</strong></td>
        <td>http://localhost:8080/api</td>
        <td>https://localhost:8081/api</td>
        <td>REST API Backend</td>
    </tr>
    <tr>
        <td><strong>Gateway</strong></td>
        <td>http://localhost:8084/gw/health</td>
        <td>https://localhost:8085/gw/health</td>
        <td>YARP Reverse Proxy</td>
    </tr>
</table>

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

```powershell
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.AppHost
dotnet run
```

The .NET Aspire dashboard will open automatically at `https://localhost:17298 and display the following services: Web, API, Gateway`

### Individual Project Development

```powershell
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

![swagger-ui.png](swagger-ui.png)

### OpenAPI Specification

Download the raw OpenAPI/Swagger JSON:

**URL:** `https://localhost:8081/swagger/v1/swagger.json`

![swagger-json.png](swagger-json.png)

### Generating API Clients

The project uses **Kiota** to generate strongly-typed API clients:

```powershell
# Regenerate API client SDK
cd Shared-Modules\AppBlueprint.Api.Client.Sdk
dotnet build
```

## Database Management

### Entity Framework Core Migrations

All commands should be executed from the `AppBlueprint.Infrastructure` project directory since the DbContext resides there.

```powershell
cd Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

### Common EF Core Commands

#### View Database Context Information

```powershell
dotnet ef dbcontext info --context "ApplicationDBContext"
```

#### Create a New Migration

```powershell
dotnet ef migrations add <name> --context ApplicationDBContext
```

#### Apply Migrations to Database

```powershell
dotnet ef database update --context ApplicationDBContext
```

#### Remove Last Migration

```powershell
dotnet ef migrations remove --context ApplicationDBContext
```

#### Generate SQL Script

```powershell
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

<details>
<summary>View Full Directory.Packages.props Configuration</summary>

```xml
<!-- This file is used to centrally define the versions of the packages used in the project -->
<Project>
    <PropertyGroup>
        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    </PropertyGroup>

    <!-- Entity Framework Core and Database Providers -->
    <ItemGroup>
        <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="9.0.1" />
        <PackageVersion Include="Microsoft.EntityFrameworkCore.Relational" Version="9.0.1" />
        <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.1">
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
            <PrivateAssets>all</PrivateAssets>
        </PackageVersion>
        <PackageVersion Include="Npgsql" Version="9.0.2" />
        <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.3" />
        <PackageVersion Include="AspNetCore.HealthChecks.NpgSql" Version="9.0.0" />
    </ItemGroup>

    <!-- Configuration and Compliance -->
    <ItemGroup>
        <PackageVersion Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.21.0" />
        <PackageVersion Include="Microsoft.Extensions.Http.Resilience" Version="9.1.0" />
        <PackageVersion Include="Microsoft.Extensions.ServiceDiscovery" Version="9.0.0" />
        <PackageVersion Include="Microsoft.Extensions.Http.Resiliency" Version="9.1.0" />
        <PackageVersion Include="Microsoft.Extensions.Hosting" Version="9.0.1" />
        <PackageVersion Include="Microsoft.Extensions.Configuration" Version="9.0.1" />
        <PackageVersion Include="Microsoft.Extensions.Configuration.Abstractions" Version="9.0.1" />
        <PackageVersion Include="Microsoft.Extensions.Configuration.UserSecrets" Version="9.0.1" />
        <PackageVersion Include="Microsoft.Extensions.Compliance.Abstractions" Version="9.1.0" />
        <PackageVersion Include="Microsoft.Extensions.Compliance.Redaction" Version="9.1.0" />
        <PackageVersion Include="Microsoft.Extensions.Compliance.Testing" Version="9.1.0" />
    </ItemGroup>

    <!-- ASP.NET Core and Middleware -->
    <ItemGroup>
        <PackageVersion Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.1" />
        <PackageVersion Include="Microsoft.IdentityModel.Tokens" Version="8.3.1" />
        <PackageVersion Include="System.IdentityModel.Tokens.Jwt" Version="8.3.1" />
        <PackageVersion Include="Microsoft.AspNetCore.Authorization" Version="9.0.0" />
        <PackageVersion Include="Microsoft.AspNetCore.DataProtection" Version="9.0.1" />
        <PackageVersion Include="Microsoft.AspNetCore.DataProtection.Abstractions" Version="9.0.1" />
        <PackageVersion Include="Microsoft.AspNetCore.OpenApi" Version="9.0.0" />
        <PackageVersion Include="Microsoft.AspNetCore.Components.Web" Version="9.0.1" />
        <PackageVersion Include="Microsoft.AspNetCore.App" Version="2.2.8" />
    </ItemGroup>

    <!-- OpenTelemetry Dependencies -->
    <ItemGroup>
        <PackageVersion Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.11.1" />
        <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.11.1" />
        <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.11.0" />
        <PackageVersion Include="OpenTelemetry.Instrumentation.Http" Version="1.11.0" />
        <PackageVersion Include="OpenTelemetry.Instrumentation.Runtime" Version="1.11.0" />
    </ItemGroup>

    <!-- Swashbuckle and OpenAPI -->
    <ItemGroup>
        <PackageVersion Include="Swashbuckle.AspNetCore" Version="7.2.0" />
        <PackageVersion Include="Swashbuckle.AspNetCore.Filters" Version="8.0.2" />
        <PackageVersion Include="Swashbuckle.AspNetCore.SwaggerGen" Version="7.2.0" />
        <PackageVersion Include="Swashbuckle.AspNetCore.SwaggerUI" Version="7.2.0" />
        <PackageVersion Include="Microsoft.OpenApi" Version="1.6.22" />
        <PackageVersion Include="Microsoft.OpenApi.Readers" Version="2.0.0-preview5" />
    </ItemGroup>

    <!-- Strawberry Shake (GraphQL) -->
    <ItemGroup>
        <PackageVersion Include="StrawberryShake" Version="14.3.0" />
        <PackageVersion Include="StrawberryShake.Transport.Http" Version="14.3.0" />
        <PackageVersion Include="StrawberryShake.Transport.WebSockets" Version="14.3.0" />
    </ItemGroup>

    <!-- MudBlazor and UI Components -->
    <ItemGroup>
        <PackageVersion Include="MudBlazor" Version="8.2.0" />
        <PackageVersion Include="MudBlazor.ThemeManager" Version="3.0.0" />
    </ItemGroup>

    <!-- Health Checks -->
    <ItemGroup>
        <PackageVersion Include="AspNetCore.HealthChecks.UI.Client" Version="9.0.0" />
        <PackageVersion Include="AspNetCore.HealthChecks.Redis" Version="9.0.0" />
        <PackageVersion Include="AspNetCore.HealthChecks.Uris" Version="9.0.0" />
    </ItemGroup>

    <!-- Testing Dependencies -->
    <ItemGroup>
        <PackageVersion Include="Testcontainers.PostgreSql" Version="4.1.0" />
        <PackageVersion Include="Testcontainers" Version="4.1.0" />
        <PackageVersion Include="TestContainers.Container.Abstractions" Version="1.5.4" />
        <PackageVersion Include="xunit" Version="2.9.3" />
        <PackageVersion Include="xunit.runner.visualstudio" Version="3.0.1">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageVersion>
        <PackageVersion Include="coverlet.collector" Version="6.0.3">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageVersion>
        <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
        <PackageVersion Include="TUnit" Version="0.6.89" />
        <PackageVersion Include="TUnit.Assertions" Version="0.6.89" />
    </ItemGroup>

    <!-- Miscellaneous Dependencies -->
    <ItemGroup>
        <PackageVersion Include="FluentValidation" Version="11.11.0" />
        <PackageVersion Include="AutoBogus" Version="2.13.1" />
        <PackageVersion Include="Bogus" Version="35.6.1" />
        <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
        <PackageVersion Include="System.Text.Json" Version="9.0.1" />
        <PackageVersion Include="Serilog" Version="4.2.0" />
        <PackageVersion Include="Stripe.net" Version="47.2.0" />
        <PackageVersion Include="Spectre.Console" Version="0.49.1" />
        <PackageVersion Include="FigletFontArt" Version="1.0.0" />
        <PackageVersion Include="Scalar.AspNetCore" Version="1.2.75" />
        <PackageVersion Include="System.CommandLine" Version="2.0.0-beta4.22272.1" />
        <PackageVersion Include="System.CommandLine.NamingConventionBinder" Version="2.0.0-beta4.22272.1" />
        <PackageVersion Include="System.Configuration.ConfigurationManager" Version="9.0.0" />
        <PackageVersion Include="NetArchTest.Rules" Version="1.3.2" />
        <PackageVersion Include="JetBrains.Annotations" Version="2024.3.0" />
        <PackageVersion Include="Karambolo.PO" Version="1.11.1" />
        <PackageVersion Include="MongoDB.Driver.Core" Version="2.30.0" />
        <PackageVersion Include="Yarp.ReverseProxy" Version="2.2.0" />
    </ItemGroup>

    <!-- Application Hosting -->
    <ItemGroup>
        <PackageVersion Include="Aspire.Hosting.AppHost" Version="9.0.0" />
        <PackageVersion Include="Aspire.Hosting.PostgreSQL" Version="9.0.0" />
        <PackageVersion Include="Aspire.Hosting.Redis" Version="9.0.0" />
        <PackageVersion Include="Aspire.Hosting.Testing" Version="9.0.0" />
    </ItemGroup>

    <!-- API Versioning -->
    <ItemGroup>
        <PackageVersion Include="Asp.Versioning.Mvc" Version="8.1.0" />
        <PackageVersion Include="Asp.Versioning.Mvc.ApiExplorer" Version="8.1.0" />
    </ItemGroup>
</Project>
```

</details>

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

        <!-- Enable source code analysis using sonar nuget package -->
        <AnalysisLevel>latest</AnalysisLevel>
        <AnalysisMode>All</AnalysisMode>
        <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
        <CodeAnalysisTreatWarningsAsErrors>false</CodeAnalysisTreatWarningsAsErrors>
        <EnforceCodeStyleInBuild>false</EnforceCodeStyleInBuild>
        
        <!-- Enable packaging the shared projects to Nuget package files -->        
        <IsPackable>true</IsPackable>
        <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
        <GeneratePackage>true</GeneratePackage>                
        <Authors>Casper Rubæk</Authors>
        <Company>SaaS Factory</Company>
        <Copyright>© SaaaS Factory</Copyright>
        <Description>Shared .nuget packages for Appblueprint template from Shared-Modules folder</Description>        
        <RepositoryUrl>https://github.com/saas-factory-labs/SaaS-Factory</RepositoryUrl>
        <RepositoryType>git</RepositoryType>        
    </PropertyGroup>

    <!-- Enable source code analysis using sonar nuget package across all projects except docker projects eg. .dcproj -->
    <ItemGroup Condition="'$(MSBuildProjectExtension)' != '.dcproj'">
<!--        Version="*"-->
<!--        <PackageReference Include="SonarAnalyzer.CSharp" >-->
<!--            <PrivateAssets>all</PrivateAssets>-->
<!--            <IncludeAssets>-->
<!--                runtime; build; native; contentfiles; analyzers; buildtransitive-->
<!--            </IncludeAssets>-->
<!--        </PackageReference>-->
    </ItemGroup>

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
        <!-- <add key="LocalNugetFeed" value="C:\Development\LocalNugetFeed" /> -->
        <add key="saas-factory-labs-github" value="https://nuget.pkg.github.com/saas-factory-labs/index.json" />
    </packageSources>

    <packageSourceMapping>
        <packageSource key="nuget.org">
            <package pattern="*" />            
        </packageSource>
        <!-- <packageSource key="LocalNugetFeed"> -->
        <!--     <package pattern="*" /> -->
        <!-- </packageSource> -->
        <packageSource key="saas-factory-labs-github">
            <package pattern="AppBlueprint.*" />
            <package pattern="SaaS-Factory.*" />
        </packageSource>
    </packageSourceMapping>

    <config>
        <add key="globalPackagesFolder" value="packages" />
    </config>

    <packageSourceCredentials>
        <saas-factory-labs-github>
            <add key="Username" value="saas-factory-labs" />
            <add key="ClearTextPassword" value="%GITHUB_saas-factory-labs_PAT%"  />
        </saas-factory-labs-github>
    </packageSourceCredentials>
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

#### GitHub Workflow Configuration

Add these steps in the workflow for publishing packages to GitHub at `.github/workflows/publish-nuget-packages-github-packages.yml`:

```yaml
- name: Install GitVersion
  run: dotnet tool install --global GitVersion.Tool

- name: Debug GitVersion Output
  run: |
    dotnet-gitversion /output json || echo "GitVersion failed!"

- name: Determine Version
  id: gitversion
  run: |
    RAW_VERSION=$(dotnet-gitversion /output json 2>/dev/null || echo '{}')
    echo "Raw GitVersion Output: $RAW_VERSION"
    VERSION=$(echo "$RAW_VERSION" | jq -r '.SemVer // empty')
    
    if [[ -z "$VERSION" ]]; then
      echo "GitVersion failed to generate a version. Check Git tags and branch naming."
      exit 1
    fi
    
    echo "VERSION=$VERSION" >> $GITHUB_ENV
  shell: bash
```

## Docker Configuration

### Building Docker Images

Build a Docker image from the correct build context:

```bash
cd Code/AppBlueprint
docker build -f AppBlueprint.Web/Dockerfile . -t appblueprint-web:dev
```

> [!IMPORTANT]
> The build context must be set to `Code/AppBlueprint` directory to ensure access to `Directory.Packages.props`, `NuGet.Config`, and dependent projects.

### Dockerfile

**Location:** `Code/AppBlueprint/AppBlueprint.Web/Dockerfile`

```dockerfile
# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GITHUB_saas-factory-labs_PAT

# Install SSL libraries for HTTPS
RUN apt-get update && apt-get install -y libssl-dev

WORKDIR /src

# Copy the central package management file and NuGet config from AppBlueprint directory
COPY Directory.Packages.props ./
COPY NuGet.Config /root/.nuget/NuGet.Config

# Replace the token placeholder in NuGet config
RUN sed -i "s|%GITHUB_saas-factory-labs_PAT%|$GITHUB_saas-factory-labs_PAT|g" /root/.nuget/NuGet.Config

# Copy project files
COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]

# Restore packages (Now using the updated NuGet config with correct GitHub token)
RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/AppBlueprint.Web"
RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
```

### Docker Compose

**Location:** `Code/AppBlueprint/docker-compose.yml`

The Docker Compose configuration orchestrates all services for local development.

### GitHub Container Registry Login

To pull/push images from GitHub Container Registry (ghcr.io) on a VM or Coolify:

```bash
docker login ghcr.io -u saas-factory-labs
# Enter your GitHub Personal Access Token (PAT) when prompted
```

> [!WARNING]
> Your password will be stored unencrypted in `/root/.docker/config.json`. Consider configuring a credential helper for production environments.

**Example output:**
```
WARNING! Your password will be stored unencrypted in /root/.docker/config.json.
Configure a credential helper to remove this warning. See
https://docs.docker.com/engine/reference/commandline/login/#credentials-store
Login Succeeded
```

## Development Tools

### Running GitHub Actions Locally with Act

[Act](https://github.com/nektos/act) allows you to test GitHub Actions workflows locally using Docker.

#### Install Act

```powershell
winget install nektos.act
```

#### Run All Workflows

```powershell
act -P ubuntu-latest=ghcr.io/catthehacker/ubuntu:act-latest
```

#### Run Specific Workflow

```powershell
act -P ubuntu-latest=ghcr.io/catthehacker/ubuntu:act-latest -j publish-packages
```

#### Important Notes

- **Agent Type:** Use `ubuntu-latest` instead of `self-hosted` when testing locally
- **Workflow Syntax:** All workflows must be valid YAML. A syntax error in any workflow file will prevent `act` from running, even if you're targeting a different workflow
- **Docker Required:** Act runs workflows in Docker containers

### Upgrading .NET Aspire

To upgrade to the latest .NET Aspire version:

#### Install Upgrade Assistant

```powershell
dotnet tool install -g upgrade-assistant
```

#### Run Upgrade

```powershell
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

// ❌ Avoid: Using ?? or ??= operators (per coding standards)
var user = await _dbContext.GetUser(userId) ?? defaultUser; // Don't use
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

```powershell
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


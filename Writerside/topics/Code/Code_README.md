# 👨‍💻 Development Workflow

_Complete guide to setup, develop, and maintain the AppBlueprint project_

## 🚀 Quick Start

Get up and running in three simple steps:

1. **Install Prerequisites** - Set up your development environment
2. **Run the Application** - Start the AppHost with .NET Aspire
3. **Start Developing** - Access Swagger UI and begin building features

## 🛠️ Prerequisites

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

## 🛜 Port Mappings

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
   ```json
   // launchSettings.json
   "environmentVariables": {
     "ASPNETCORE_URLS": "https://localhost:8081"
   }
   ```

4. **App Settings Files**
   ```json
   // appsettings.json
   "Kestrel": {
     "Endpoints": { ... }
   }
   ```

## ⚙️ Running the Application

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

## 📚 API Documentation

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

## 🗄️ Database Management

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

## 🔧 Development Tools

### Key Configuration Files

#### NuGet.Config
Central package source configuration for the solution.

#### Directory.Build.props
MSBuild properties shared across all projects (versioning, authors, repository info).

#### Directory.Packages.props
Central Package Management (CPM) for NuGet package versions.

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

## 📖 Best Practices

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

## 🔍 Troubleshooting

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

## 📚 Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Clean Architecture Guide](https://github.com/jasontaylordev/CleanArchitecture)
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)

## 🆘 Getting Help

- **GitHub Issues:** [Report a bug or request a feature](https://github.com/saas-factory-labs/Saas-Factory/issues)
- **GitHub Discussions:** [Ask questions and share ideas](https://github.com/saas-factory-labs/Saas-Factory/discussions)


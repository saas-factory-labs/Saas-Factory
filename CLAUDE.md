# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SaaS Factory is a B2B/B2C SaaS blueprint for massively accelerating development of new SaaS web applications. The project uses .NET 10, .NET Aspire for orchestration, Clean Architecture with Domain-Driven Design principles, and targets rapid deployment with minimal boilerplate.

**Current focus:** All development work is on the `AppBlueprint` directory (`Code/AppBlueprint/`).

## Development Environment

- **Target Framework:** .NET 10 (`net10.0`)
- **SDK Version:** 10.0.100 (see `global.json`)
- **.NET Aspire:** 13.0.0 (orchestration)
- **Database:** PostgreSQL (Railway cloud database)
- **Authentication:** Logto (cloud-based)
- **UI Framework:** Blazor Server with MudBlazor components
- **Platform:** Windows with PowerShell (primary development environment)

## Essential Commands

### Running the Application

**IMPORTANT:** The AppHost project is typically already running in watch mode. **DO NOT run it** unless explicitly needed. Only build projects to verify compilation.

```powershell
# Navigate to AppBlueprint directory
cd Code\AppBlueprint

# Run the Aspire AppHost (orchestrates all services)
# Only if not already running
cd AppBlueprint.AppHost
dotnet run

# Build a specific project to verify compilation
cd AppBlueprint.Web
dotnet build

cd ..\AppBlueprint.ApiService
dotnet build
```

**Port Configuration (Development):**
- Web Frontend (HTTPS): `https://localhost:8083`
- Web Frontend (HTTP): `http://localhost:8082`
- API Service: `http://localhost:8091`
- Gateway: `http://localhost:8087`
- Aspire Dashboard: `http://localhost:18889` (OTLP endpoint)

### Testing

**Test Framework:** TUnit (primary), xUnit (legacy), bUnit (Blazor UI tests), FluentAssertions

```powershell
# Run all tests
cd Code\AppBlueprint
dotnet test

# Run tests for specific project
cd AppBlueprint.Tests
dotnet test

# Run specific test class/method
dotnet test --filter "FullyQualifiedName~TokenStorageServiceTests"
```

### Database Migrations

```powershell
# Navigate to Infrastructure project
cd Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Add new migration
dotnet ef migrations add MigrationName --context AppBlueprintDbContext

# Update database (Railway cloud database via connection string in appsettings)
dotnet ef database update --context AppBlueprintDbContext

# Remove last migration (if not applied)
dotnet ef migrations remove --context AppBlueprintDbContext
```

**Database Context Location:** `Shared-Modules/AppBlueprint.Infrastructure/DatabaseContexts/`

### Code Formatting

```powershell
# Format code according to .editorconfig
cd Code\AppBlueprint
dotnet format
```

## Architecture

### High-Level Structure

The project follows **Clean Architecture** with **Domain-Driven Design** principles, organized in a monorepo structure:

```
Code/AppBlueprint/
├── AppBlueprint.AppHost/          # .NET Aspire orchestrator (entry point)
├── AppBlueprint.Web/              # Blazor Server UI (frontend)
├── AppBlueprint.ApiService/       # REST API backend
├── AppBlueprint.AppGateway/       # YARP reverse proxy/gateway
├── AppBlueprint.DeveloperCli/     # CLI tools for developers
├── AppBlueprint.ServiceDefaults/  # Shared service configuration
├── AppBlueprint.TodoAppKernel/    # Todo feature module
├── AppBlueprint.Tests/            # Test project
└── Shared-Modules/                # Clean Architecture layers
    ├── AppBlueprint.Domain/           # Entities, Value Objects, Aggregates
    ├── AppBlueprint.Application/      # Use Cases, Commands, Queries, DTOs
    ├── AppBlueprint.Infrastructure/   # EF Core, Repositories, External Services
    ├── AppBlueprint.Presentation.ApiModule/  # API endpoints (Minimal APIs)
    ├── AppBlueprint.Contracts/        # Shared contracts/interfaces
    ├── AppBlueprint.SharedKernel/     # Shared kernel code
    ├── AppBlueprint.UiKit/            # Reusable UI components
    └── AppBlueprint.Api.Client.Sdk/   # Kiota-generated API client
```

### Layer Responsibilities

**Domain Layer** (`AppBlueprint.Domain`):
- Core business entities and value objects
- Domain interfaces (repositories, services)
- Business rules and domain logic
- No dependencies on other layers

**Application Layer** (`AppBlueprint.Application`):
- Use case orchestration (Commands, Queries - CQRS pattern)
- DTOs and mapping
- Application services and interfaces
- Validation logic

**Infrastructure Layer** (`AppBlueprint.Infrastructure`):
- EF Core DbContext and migrations
- Repository implementations
- External service integrations (Logto, Stripe, Resend, etc.)
- Authentication and authorization implementations
- Data persistence

**Presentation Layer** (`AppBlueprint.Presentation.ApiModule`):
- Minimal API endpoints
- API versioning
- Swagger/OpenAPI documentation

### Key Architectural Patterns

1. **CQRS (Command Query Responsibility Segregation):**
   - Commands in `Application/Commands/` and `Application/CommandHandlers/`
   - Queries in `Application/Queries/`

2. **Repository Pattern:**
   - Interfaces in `Domain/Interfaces/`
   - Implementations in `Infrastructure/Repositories/`

3. **Unit of Work:**
   - Implementation in `Infrastructure/UnitOfWork/`

4. **.NET Aspire Orchestration:**
   - `AppHost` orchestrates all services with service discovery
   - Environment-specific configuration (Development vs Production/Railway)

5. **API Gateway Pattern:**
   - `AppGateway` uses YARP for reverse proxy functionality

### Authentication Architecture

**Provider:** Logto (cloud-based authentication service)

**Flow:**
1. User initiates login via Blazor UI
2. Redirects to Logto for authentication
3. Logto redirects back with authentication cookie
4. Cookie-based authentication for Blazor Server
5. JWT tokens (optional) for API calls via `ITokenStorageService`

**Key Components:**
- `Infrastructure/Authentication/` - Logto configuration and endpoints
- `Infrastructure/Authorization/` - Authorization handlers and policies
- `AspNetCoreKiotaAuthenticationProvider` - API client authentication
- `TokenStorageService` - JWT token storage (localStorage via JSInterop)

**Configuration:** Logto settings in `appsettings.json` and environment variables.

### Database Architecture

**Database:** PostgreSQL (Railway cloud-hosted in production, local via Docker optional)

**ORM:** Entity Framework Core 10.0 (RC)

**Key Points:**
- Connection strings configured via `appsettings.json`
- Railway cloud database used by default (not local PostgreSQL via Aspire)
- Migrations in `Infrastructure/Migrations/`
- DbContext: `AppBlueprintDbContext` in `Infrastructure/DatabaseContexts/`
- Data seeding via `DataSeeder.cs`

## Important Development Rules

### Code Quality Standards

1. **Null Checks:** Use `is null` or `is not null` (NOT `??` or `??=` operators). Use `ArgumentNullException.ThrowIfNull()` and `ArgumentException.ThrowIfNullOrEmpty()`.

2. **Error Handling:** Use exceptions for error handling, not null coalescing operators.

3. **Namespaces:** Set namespace in `.cs` files according to folder structure.

4. **Testing:**
   - Write tests FIRST before implementing changes (TDD approach)
   - Use TUnit for new tests, FluentAssertions for assertions
   - Use NSubstitute for mocking

5. **Build Verification:** Always run `dotnet build` after changes to verify compilation.

6. **No Over-engineering:** Stay within scope - do not implement or over-engineer features beyond the task requirements.

7. **Warnings vs Errors:** Focus on resolving errors, not warnings (unless warnings are relevant to the task).

8. **Using Statements:** Remove unnecessary using statements when editing files.

### File Operations

- **NEVER delete files** without explicit user confirmation
- **ALWAYS prefer editing existing files** over creating new ones
- **DO NOT create** `.md` documentation files proactively unless explicitly requested
- **DO NOT remove commented code** unless explicitly requested

### Package Management

**Central Package Management:** All package versions defined in `Directory.Packages.props`

To add/update packages:
1. Use Nuget MCP server to query correct versions (release versions without vulnerabilities preferred)
2. Ask user before upgrading/downgrading packages
3. Update version in `Directory.Packages.props`
4. Add `<PackageReference>` (without version) to project `.csproj`

### Git Workflow

When creating commits:
1. Run `git status` to see untracked/modified files
2. Run `git diff` to see changes
3. Run `git log` to understand commit message style
4. Create concise commit message (1-2 sentences) focusing on "why" not "what"
5. End commit message with:
   ```
   🤖 Generated with [Claude Code](https://claude.com/claude-code)

   Co-Authored-By: Claude <noreply@anthropic.com>
   ```

## Technology Stack

### Frontend
- **Framework:** Blazor Server (Interactive Server Mode)
- **UI Library:** MudBlazor 8.14.0
- **Components:** Razor Components with code-behind pattern

### Backend
- **Framework:** .NET 10 Minimal APIs
- **API Documentation:** Swagger, Scalar, NSwag
- **API Versioning:** Asp.Versioning

### Testing
- **Unit Tests:** TUnit, xUnit, NSubstitute, FluentAssertions
- **UI Tests:** bUnit
- **Integration Tests:** Testcontainers (PostgreSQL, Redis)
- **Test Data:** Bogus, AutoBogus

### Infrastructure
- **Hosting:** Railway (production), .NET Aspire (development)
- **Database:** PostgreSQL (Npgsql EF Core provider)
- **Caching:** Redis (Aspire.StackExchange.Redis)
- **Authentication:** Logto.AspNetCore.Authentication
- **Telemetry:** OpenTelemetry, Aspire Dashboard
- **Observability:** Serilog, OpenTelemetry

### External Services
- **Payment:** Stripe
- **Email:** Resend
- **Storage:** Cloudflare R2 (S3-compatible), Azure Blob Storage
- **Secrets:** Azure Key Vault

## Troubleshooting

### AppHost Already Running
If you see port conflicts, the AppHost is likely already running. Check running processes and terminate if needed.

### Database Connection Issues
- Verify Railway database connection string in `appsettings.json`
- Check environment variables for connection string overrides

### Authentication Issues
- Verify Logto configuration in `appsettings.json`
- Check Logto dashboard for redirect URI configuration
- Ensure cookies are enabled in browser

### Build Errors After .NET 10 Upgrade
- Recent upgrade to .NET 10 completed
- Some projects may have lingering .NET 9 references - update to .NET 10 as needed
- Check `Directory.Build.props` for target framework

## Additional Resources

- **Main Branch:** `main`
- **Repository:** https://github.com/saas-factory-labs/Saas-Factory
- **AI Rules:** `.github/.ai-rules/` contains detailed rules for backend, frontend, and infrastructure
- **Copilot Instructions:** `.github/copilot-instructions.md`

## Notes for Claude Code

1. **Read AI rules first:** Before implementing changes, consult relevant rules in `.github/.ai-rules/` (backend, frontend, infrastructure, etc.)

2. **Test-first approach:** Always write tests before implementation

3. **Build verification:** Run `dotnet build` after each change to verify compilation

4. **Don't assume AppHost status:** The AppHost is usually running - ask before starting it

5. **PowerShell commands only:** Use PowerShell commands (NOT bash) when running CLI commands

6. **Railway deployment:** Production deployments use Railway with environment-specific configuration

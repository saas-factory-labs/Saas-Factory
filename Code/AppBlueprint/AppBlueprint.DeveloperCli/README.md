# AppBlueprint.DeveloperCli

Developer CLI to facilitate easy streamlined development of the Appblueprint.

## Features

### Testing
- **`test`** - Run tests with comprehensive options:
  - `--watch` - Auto-rerun tests on file changes
  - `--coverage` - Generate code coverage reports  
  - `--filter` - Filter tests by name or category
  - `--project` - Run specific test project
  - `--verbosity` - Control logging detail (quiet, minimal, normal, detailed, diagnostic)
  - `--no-restore` - Skip dependency restoration
  - `--no-build` - Skip project build

### Environment Management
- **`env:info`** - Display comprehensive development environment information including:
  - Configuration settings (database, authentication, environment)
  - Service status (Docker, PostgreSQL, API, Web App)
  - System information (.NET SDK, OS, current directory)
  - Project paths and structure

### Database Operations
- Migrate or rollback database changes from EF Core migration code to the database

### Git & GitHub Integration
- Git commit and push to Github repository
- Generate Github Action workflow
- Create Github repository for new app project

### Code Generation
- Generate code file such as API controller or similar
- Generate .Net project
- Generate Visual Studio solution with the projects from Appblueprint
    - AppBlueprint.ApiService
    - AppBlueprint.AppHost
    - AppBlueprint.Web
    - AppBlueprint.Tests
    - AppBlueprint.AppGateway
    - AppBlueprint.ServiceDefaults
    - AppBlueprint Shared Modules

## Usage

```bash
# Display environment information
dotnet run -- env:info

# Run tests
dotnet run -- test
dotnet run -- test --watch
dotnet run -- test --coverage
dotnet run -- test --filter "FullyQualifiedName~UnitTests"
dotnet run -- test --project "path/to/TestProject.csproj"

# Other commands
dotnet run -- migrate-database
dotnet run -- jwt-token
# ... see available commands with --help
```

# Developer CLI Rules

When working with the AppBlueprint.DeveloperCli project, follow these rules very carefully.

## Implementation

IMPORTANT: Always follow these steps very carefully when implementing changes:

1. Consult any relevant rules files listed below and start by listing which rule files have been used to guide your response.
2. Always run `dotnet build` to verify the code compiles after each change.
3. Fix any compiler warnings or test failures before moving on to the next step.

## Developer CLI Overview

The Developer CLI (AppBlueprint.DeveloperCli) is a command-line tool that facilitates streamlined development of the AppBlueprint. It provides automation for common development tasks.

### Key Features

- **Database Migrations**: Migrate or rollback database changes from EF Core migration code
- **Git Operations**: Commit and push changes to GitHub repository
- **Code Generation**: Generate code files such as API controllers, .NET projects, and Visual Studio solutions
- **GitHub Integration**: Create GitHub repositories and generate GitHub Action workflows
- **Project Management**: Generate Visual Studio solutions with all AppBlueprint projects

### Project Structure

The CLI is built using:
- Spectre.Console for interactive terminal UI
- Command pattern for organizing CLI commands
- Menu-driven interface for ease of use

### Development Guidelines

- Use Spectre.Console for all console interactions
- Follow the command pattern for new CLI commands
- Ensure all commands are well-documented with help text
- Test CLI commands manually before committing changes
- Keep commands focused and single-purpose

## Related Projects

The Developer CLI interacts with:
- AppBlueprint.ApiService
- AppBlueprint.AppHost
- AppBlueprint.Web
- AppBlueprint.Tests
- AppBlueprint.AppGateway
- AppBlueprint.ServiceDefaults
- AppBlueprint Shared Modules

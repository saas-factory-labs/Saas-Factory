# Baseline Rules

These are fundamental rules that apply to all code in the AppBlueprint project, regardless of the specific area (backend, frontend, infrastructure, etc.).

## Core Principles

- Follow Domain-Driven Design (DDD) and Clean Architecture principles
- Maintain low technical debt through standardized implementations
- Focus on reliability and maintainability
- Keep code simple and readable
- Avoid over-engineering

## Rule Files

- [Code Style](./code-style.md) - C# coding standards and conventions
- [Entity Modeling](./entity-modeling.md) - DDD entity, aggregate, and value object patterns

## General Guidelines

### Null Handling

- Use `is null` or `is not null` checks instead of `== null` or `!= null`
- Do NOT use `??` or `??=` operators
- Use `ThrowIfNull` and `ThrowIfEmpty` methods to validate inputs

### Error Handling

- Handle errors using exceptions
- Do not use exceptions for control flow
- Provide meaningful exception messages with a period at the end
- Use `UnreachableException` for code paths that should never be reached

### Code Organization

- Set namespaces according to folder structure
- Remove unnecessary using statements
- Mark all C# types as sealed unless inheritance is required
- Use primary constructors where appropriate
- Use top-level namespaces

### Testing

- Use TUnit for unit and integration tests
- Use bUnit for Blazor UI tests
- Use FluentAssertions for test assertions
- Create tests when making additions or modifications

### Code Formatting

- Use `dotnet format` to format code according to `.editorconfig`
- Follow the conventions in `.editorconfig` strictly
- Use `var` when the type is obvious
- Prefer `List<T>` over `T[]` for collections

### Dependencies

- Query NuGet packages for versions without known vulnerabilities
- Prefer release versions over pre-release versions
- Ask before upgrading or downgrading packages
- Never introduce new NuGet dependencies without approval

### Documentation

- Add XML summary comments to describe methods and classes
- Make IntelliSense more useful with clear summaries
- Use clear names instead of relying on comments
- Never use acronyms in naming
- Only add comments when code cannot express the intent clearly

### Database

- Use Entity Framework Core for database access
- Use PostgreSQL as the database provider
- Verify database schema using PostgreSQL tools

### Version Control

- Do not remove commented-out code unless explicitly asked
- Always provide clear warnings when deleting files or folders
- Focus on resolving errors, not warnings

## Authentication

- Do not implement password hashing or encryption directly
- The system uses external authentication providers (e.g., Logto, Auth0)
- Authentication is handled by dedicated services

## Platform Considerations

- Development is primarily done on Windows using PowerShell
- Do not use bash commands or `&&` operators in PowerShell commands
- Run commands sequentially in PowerShell instead of chaining with `&&`

## Build and Run Guidelines

- The AppHost project is the entry point
- Do NOT RUN AppHost if it's already running in watch mode
- Only build projects to ensure they compile correctly
- Always verify builds succeed before committing changes

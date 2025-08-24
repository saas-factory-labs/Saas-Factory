# AppBlueprint.Application

[![NuGet Version](https://img.shields.io/nuget/v/SaaS-Factory.AppBlueprint.Application)](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.Application)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Overview

The AppBlueprint.Application package provides the application layer implementation for the AppBlueprint template. This package contains CQRS commands, queries, use cases, and application services following Clean Architecture and Domain-Driven Design principles.

## Features

### CQRS Implementation
- **Commands** - State-changing operations with command handlers
- **Queries** - Data retrieval operations with query handlers  
- **Command Handlers** - Business logic execution for commands
- **Query Handlers** - Data access logic for queries

### Application Services
- **Use Cases** - Business logic orchestration (CQRS + MediatR)
- **Application Services** - Cross-cutting application concerns
- **DTOs (Data Transfer Objects)** - Data contracts for API boundaries
- **Validators** - Input validation logic
- **Behaviors** - Cross-cutting concerns (logging, validation, exceptions)

### Clean Architecture Components
- **Entities** - Core business entities
- **Value Objects** - Immutable domain concepts
- **Domain Events** - Domain-driven event handling
- **Domain Services** - Business logic services
- **Interfaces** - Application abstractions/contracts
- **Exceptions** - Domain-specific exceptions
- **Enums** - Domain enumerations
- **Mappers** - Object mapping utilities
- **Specifications** - Complex business rule specifications

## Installation

```bash
dotnet add package SaaS-Factory.AppBlueprint.Application
```

## Dependencies

This package includes the following key dependencies:

- **AppBlueprint.Domain** - Domain entities and business rules
- **AppBlueprint.Contracts** - Shared contracts and interfaces
- **Fluent-Regex** - Regular expression utilities

## Usage

### Commands (State-Changing Operations)

```csharp
// Example command
public sealed record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName
) : ICommand<UserId>;

// Example command handler
public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserId>
{
    public async Task<UserId> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Business logic implementation
        var user = User.Create(command.Email, command.FirstName, command.LastName);
        // Save user via repository
        return user.Id;
    }
}
```

### Queries (Data Retrieval Operations)

```csharp
// Example query
public sealed record GetUserByIdQuery(UserId UserId) : IQuery<UserDto>;

// Example query handler
public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        // Data access logic
        var user = await _userRepository.GetByIdAsync(query.UserId);
        return UserDto.FromDomain(user);
    }
}
```

### Cross-Cutting Concerns

The application layer handles cross-cutting concerns through:

- **Logging** - Structured logging with dependency injection
- **Validation** - Input validation using FluentValidation
- **Exception Handling** - Domain-specific exception management
- **Dependency Injection** - Service registration and configuration

## Architecture

This application layer implements:

- **CQRS Pattern** - Separation of read and write operations
- **Mediator Pattern** - Decoupled request/response handling (MediatR)
- **Repository Pattern** - Data access abstraction
- **Specification Pattern** - Complex query logic encapsulation
- **Domain Event Pattern** - Cross-cutting concern handling

## Project Structure

```
AppBlueprint.Application/
├── Commands/           # State-changing operations
├── CommandHandlers/    # Command execution logic
├── Queries/           # Data retrieval operations
├── QueryHandlers/     # Query execution logic
├── DTOs/              # Data transfer objects
├── Validators/        # Input validation
├── Services/          # Application services
├── Behaviors/         # Cross-cutting concerns
├── Mappers/           # Object mapping
├── Specifications/    # Business rule specifications
└── Interfaces/        # Application abstractions
```

## Use Cases

The application defines use cases through CQRS + MediatR:

- **Business Logic** - Core application business rules
- **Use Cases** - Specific application operations
- **Application Services** - Orchestration of domain services
- **Cross-cutting Concerns** - Logging, validation, exception handling

## Contributing

This package is part of the SaaS Factory Labs AppBlueprint template. Contributions are welcome!

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/saas-factory-labs/Saas-Factory/blob/main/LICENSE) file for details.

## Support

- 📚 [Documentation](https://github.com/saas-factory-labs/Saas-Factory)
- 🐛 [Issues](https://github.com/saas-factory-labs/Saas-Factory/issues)
- 💬 [Discussions](https://github.com/saas-factory-labs/Saas-Factory/discussions)

## Links

- [Source Code](https://github.com/saas-factory-labs/Saas-Factory)
- [SaaS Factory Labs](https://github.com/saas-factory-labs)
- [NuGet Package](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.Application)
# AppBlueprint.Domain

[![NuGet Version](https://img.shields.io/nuget/v/SaaS-Factory.AppBlueprint.Domain)](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.Domain)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Overview

AppBlueprint.Domain is the Domain layer of the AppBlueprint framework, following Clean Architecture and Domain-Driven Design principles. It contains entities, domain interfaces, and business rules with no dependency on Infrastructure, Application, or any external service - only [AppBlueprint.SharedKernel](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.SharedKernel).

## Features

### Entities
- **Baseline** - Core account entities: identity, customers, auditing, API logging, data exports, notifications, security, integrations
- **B2B** - Team and tenant management (`TeamManagementService`, `TenantService`) for multi-tenant business accounts
- **Entities** - User, notification, and webhook entities

### Domain Interfaces
- **Repository interfaces** (`Interfaces/Repositories`) - Contracts implemented by the Infrastructure layer (e.g. `INotificationRepository`, `IEmailVerificationRepository`, `IPasswordResetRepository`)
- **Service interfaces** (`Interfaces/Services`) - Domain service contracts
- **Unit of Work** (`Interfaces/UnitOfWork`) - Transactional boundary contract

### Design Principles
- No dependency on EF Core, ASP.NET Core, or any other Infrastructure/Application concern
- Entities build on `BaseEntity` from `AppBlueprint.SharedKernel` for identity and audit fields
- Repository and service interfaces live here; their implementations live in `AppBlueprint.Infrastructure.*`

## Installation

```bash
dotnet add package SaaS-Factory.AppBlueprint.Domain
```

## Dependencies

- **AppBlueprint.SharedKernel** - Base entity, ID generation, and shared primitives

## Usage

```csharp
using AppBlueprint.Domain.Interfaces.Repositories;

public sealed class NotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<UserNotificationEntity>> GetUserNotificationsAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return await _notificationRepository.GetForUserAsync(userId, cancellationToken);
    }
}
```

## Related Packages

This package is part of the AppBlueprint ecosystem:
- **AppBlueprint.SharedKernel** - Base entity and shared primitives this package builds on
- **AppBlueprint.Contracts** - Request/response DTOs used at the API boundary
- **AppBlueprint.Application** - Use cases and command/query handlers that orchestrate these entities
- **AppBlueprint.Infrastructure** - EF Core repository implementations of the interfaces defined here

## Contributing

This package is part of the SaaS Factory Labs AppBlueprint template. Contributions are welcome - see the [Contributing Guide](https://github.com/saas-factory-labs/Saas-Factory/blob/main/docs/CONTRIBUTING.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/saas-factory-labs/Saas-Factory/blob/main/LICENSE) file for details.

## Support

- 📚 [Documentation](https://github.com/saas-factory-labs/Saas-Factory)
- 🐛 [Issues](https://github.com/saas-factory-labs/Saas-Factory/issues)
- 💬 [Discussions](https://github.com/saas-factory-labs/Saas-Factory/discussions)

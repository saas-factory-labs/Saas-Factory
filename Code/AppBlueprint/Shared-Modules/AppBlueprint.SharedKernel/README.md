# AppBlueprint.SharedKernel

[![NuGet Version](https://img.shields.io/nuget/v/SaaS-Factory.AppBlueprint.SharedKernel)](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.SharedKernel)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Overview

AppBlueprint.SharedKernel is the foundational package referenced by every other AppBlueprint module (Domain, Application, Infrastructure, Contracts, etc.). It has no dependencies on the rest of the framework, so it can also be pulled into unrelated .NET projects that just want its base entity, ID, and validation primitives.

## Features

- **`BaseEntity` / `IEntity`** - Base entity type with `Id`, `CreatedAt`, `LastUpdatedAt`, and soft-delete (`IsSoftDeleted`) tracking
- **`PrefixedUlid`** - Generates sortable, prefixed string identifiers (e.g. `user_...`, `tenant_...`) instead of raw GUIDs
- **`ITenantScoped`** - Marker interface for multi-tenant entity filtering
- **`Role`** - Shared role enumeration used across the auth and authorization modules
- **PII annotations** (`SharedModels/PII`) - Attributes for marking properties that require GDPR-aware handling (export/redaction)
- **Validation helpers** (`Validations/`) - Shared validation utilities used across layers

## Installation

```bash
dotnet add package SaaS-Factory.AppBlueprint.SharedKernel
```

## Dependencies

None. This is the innermost package in the dependency graph - every other AppBlueprint package depends on it, and it depends on nothing else in the framework.

## Usage

```csharp
using AppBlueprint.SharedKernel;

public sealed class UserEntity : BaseEntity, ITenantScoped
{
    public string TenantId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public static UserEntity Create(string email, string tenantId)
    {
        return new UserEntity
        {
            Id = PrefixedUlid.Generate("user"),
            Email = email,
            TenantId = tenantId
        };
    }
}
```

## Related Packages

This package is part of the AppBlueprint ecosystem:
- **AppBlueprint.Domain** - Entities, aggregates, and value objects built on top of `BaseEntity`
- **AppBlueprint.Contracts** - Request/response DTOs
- **AppBlueprint.Application** - Use cases and application services

## Contributing

This package is part of the SaaS Factory Labs AppBlueprint template. Contributions are welcome - see the [Contributing Guide](https://github.com/saas-factory-labs/Saas-Factory/blob/main/docs/CONTRIBUTING.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/saas-factory-labs/Saas-Factory/blob/main/LICENSE) file for details.

## Support

- 📚 [Documentation](https://github.com/saas-factory-labs/Saas-Factory)
- 🐛 [Issues](https://github.com/saas-factory-labs/Saas-Factory/issues)
- 💬 [Discussions](https://github.com/saas-factory-labs/Saas-Factory/discussions)

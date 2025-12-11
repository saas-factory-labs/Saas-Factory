# AppBlueprint.Contracts

[![NuGet Version](https://img.shields.io/nuget/v/SaaS-Factory.AppBlueprint.Contracts)](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.Contracts)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Overview

The AppBlueprint.Contracts package provides a shared library of Data Transfer Objects (DTOs) for client-server communication. This package contains request and response models that define the contract between the API and its clients, following Clean Architecture principles.

## Features

### Request/Response DTOs
- **Baseline Module** - Core user, profile, authentication, and authorization contracts
- **B2B Module** - Business-to-Business specific contracts (teams, tenants, subscriptions)
- **B2C Module** - Business-to-Consumer specific contracts (placeholder for future features)
- **Admin Module** - Administrative operation contracts (placeholder for future features)

### Contract Categories
- **Authentication & Authorization** - Login, logout, token management
- **User Management** - Profile creation, updates, user queries
- **Team Management** - Team creation, member management, invitations
- **Tenant Management** - Tenant operations and settings
- **Payment & Billing** - Subscription and payment processing

### Design Principles
- **API-first design** with clear request/response contracts
- **Immutable DTOs** for thread-safety
- **Validation-ready** properties with appropriate nullability
- **Serialization-friendly** for JSON, XML, and other formats
- **Framework-agnostic** - can be used with any .NET application
- **Strongly-typed** with proper C# typing

## Installation

```bash
dotnet add package SaaS-Factory.AppBlueprint.Contracts
```

## Dependencies

This package has minimal dependencies:
- **AppBlueprint.SharedKernel** - For shared types and base classes

## Usage

### Authentication Requests/Responses

```csharp
using AppBlueprint.Contracts.Baseline.Authentication;

// Login request
var loginRequest = new LoginRequest
{
    Email = "user@example.com",
    Password = "SecurePassword123!"
};

// Login response
public class LoginResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}
```

### Profile Management

```csharp
using AppBlueprint.Contracts.Baseline.Profile;

// Create profile request
var createProfileRequest = new CreateProfileRequest
{
    FirstName = "John",
    LastName = "Doe",
    DateOfBirth = new DateTime(1990, 1, 1),
    Gender = "Male",
    UserName = "johndoe",
    IsActive = true,
    Language = "en-US",
    Timezone = DateTime.UtcNow,
    Avatar = "https://example.com/avatar.jpg",
    Slug = "john-doe"
};

// Profile response
public class ProfileResponse
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? UserName { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastLogin { get; set; }
    public string? Language { get; set; }
    public DateTime Timezone { get; set; }
    public string? Avatar { get; set; }
    public string? Slug { get; set; }
}
```

### Team Management (B2B)

```csharp
using AppBlueprint.Contracts.B2B.Team;

// Create team request
var createTeamRequest = new CreateTeamRequest
{
    Name = "Development Team",
    Description = "Our awesome dev team",
    TenantId = tenantId
};

// Team member invitation
var inviteRequest = new InviteTeamMemberRequest
{
    Email = "newmember@example.com",
    Role = "Developer"
};
```

## Contract Structure

```
AppBlueprint.Contracts/
├── Baseline/
│   ├── Authentication/
│   │   ├── Requests/
│   │   │   ├── LoginRequest.cs
│   │   │   └── RegisterRequest.cs
│   │   └── Responses/
│   │       └── LoginResponse.cs
│   ├── Profile/
│   │   ├── Requests/
│   │   │   ├── CreateProfileRequest.cs
│   │   │   └── UpdateProfileRequest.cs
│   │   └── Responses/
│   │       └── ProfileResponse.cs
│   └── Payment/
│       └── Requests/
│           └── ProfileRequest.cs
├── B2B/
│   ├── Team/
│   ├── Tenant/
│   └── Subscription/
├── B2C/
│   └── (Future B2C-specific contracts)
└── Admin/
    └── (Future admin-specific contracts)
```

## Best Practices

### Request Objects
- Use `required` keyword for mandatory properties
- Provide clear, descriptive property names
- Include data annotations for validation when needed
- Keep requests focused and single-purpose

### Response Objects
- Make properties nullable where appropriate
- Include all relevant data for the client
- Use proper types (DateTimeOffset for timestamps, etc.)
- Keep responses lean and focused

### Naming Conventions
- Requests: `{Action}{Entity}Request` (e.g., `CreateUserRequest`)
- Responses: `{Entity}Response` or `{Action}{Entity}Response`
- Use PascalCase for all public members
- Prefix interfaces with `I`

## Validation

While this package doesn't include validation logic, it's designed to work with:
- **FluentValidation** for complex validation rules
- **Data Annotations** for simple validation
- **Custom validators** in the Application layer

```csharp
// Example with FluentValidation (in Application layer)
public class CreateProfileRequestValidator : AbstractValidator<CreateProfileRequest>
{
    public CreateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress();
    }
}
```

## Versioning

This package follows semantic versioning:
- **Major**: Breaking changes to existing contracts
- **Minor**: New contracts or non-breaking additions
- **Patch**: Bug fixes and documentation updates

⚠️ **Breaking Changes**: Adding required properties or removing properties are breaking changes.

## Migration Guide

When contracts change:

### Adding New Properties (Non-Breaking)
```csharp
// Before
public class UserResponse
{
    public string Name { get; set; }
}

// After - Add nullable property (non-breaking)
public class UserResponse
{
    public string Name { get; set; }
    public string? Email { get; set; } // New optional property
}
```

### Removing Properties (Breaking)
```csharp
// This requires a major version bump
// Deprecate first, remove in next major version
[Obsolete("Use NewProperty instead")]
public string OldProperty { get; set; }
```

## Related Packages

This package is part of the AppBlueprint ecosystem:
- **AppBlueprint.SharedKernel** - Base types and shared utilities
- **AppBlueprint.Api.Client.Sdk** - Type-safe API client using these contracts
- **AppBlueprint.Application** - Application services that use these contracts
- **AppBlueprint.Presentation.ApiModule** - API endpoints that implement these contracts

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
- [NuGet Package](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.Contracts)

# AppBlueprint.Infrastructure.Auth

Authentication and authorization infrastructure for the AppBlueprint SaaS platform.

## What this package provides

- Logto OIDC/OpenID Connect authentication (cookie-based for Blazor Server)
- Firebase authentication and push notification integration
- JWT Bearer token authentication
- ASP.NET Core Data Protection configuration
- Token storage service (localStorage via JS Interop)
- Authentication provider factory pattern (Logto, Firebase, Auth0, Mock)
- Kiota authentication provider adapter for API client SDK
- Authorization handlers (minimum age, document, API key)

## Usage

```csharp
builder.Services.AddAppBlueprintAuth(
    builder.Configuration,
    builder.Environment);
```

## NuGet packages included

- `Logto.AspNetCore.Authentication`
- `FirebaseAdmin`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.AspNetCore.Authorization`
- `Microsoft.AspNetCore.DataProtection`
- `Microsoft.AspNetCore.DataProtection.Abstractions`
- `Microsoft.Kiota.Abstractions`

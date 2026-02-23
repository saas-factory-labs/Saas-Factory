# AppBlueprint Stability & Integration Guide

This document outlines the architectural decisions and best practices required to ensure the AppBlueprint codebase remains stable, maintainable, and easy to integrate into consuming applications (e.g., Financial Tools, Dating Apps, Property Portals).

## 1. Pure Tailwind CSS UI Kit (Zero MudBlazor)

The `SaaS-Factory.AppBlueprint.UiKit` has been completely migrated away from MudBlazor to a **pure Tailwind CSS** implementation. 

### Why this improves stability:
*   **Zero Third-Party Component Lock-in:** Consuming apps are not forced to adopt a specific component library's lifecycle, bugs, or breaking changes.
*   **Bundle Size Reduction:** Removing heavy component libraries significantly reduces the initial load time for Blazor WebAssembly and Server apps.
*   **Universal Styling:** Tailwind CSS is an industry standard. Consuming apps can easily override styles using standard `tailwind.config.js` without fighting framework-specific CSS specificity (e.g., `.mud-button`).
*   **Dynamic Theming:** The `ThemeService` provides runtime theme switching (SaaS, Dating, CRM) by generating Tailwind utility classes dynamically.

### Integration Best Practice:
Consuming apps should ensure their `tailwind.config.js` includes the UiKit paths to prevent Tailwind from purging necessary classes:
```javascript
module.exports = {
  content: [
    './**/*.razor',
    '../Shared-Modules/AppBlueprint.UiKit/Components/**/*.razor'
  ]
}
```

## 2. Configuration Externalization (IOptions Pattern)

To make integration seamless, AppBlueprint strictly enforces the **Options Pattern** for all configuration.

### Why this improves stability:
*   **Fail-Fast Validation:** Configuration errors (missing API keys, invalid URLs) are caught at startup via `[Required]` data annotations, preventing runtime crashes.
*   **Type Safety:** Consuming apps get IntelliSense when configuring AppBlueprint services.
*   **Hot Reload:** Using `IOptionsSnapshot<T>` allows consuming apps to update configurations (like feature flags or theme colors) without restarting the application.

### Integration Best Practice:
Consuming apps should bind their configurations in `Program.cs` using the provided extension methods:
```csharp
builder.Services.AddAppBlueprintConfiguration(builder.Configuration, builder.Environment);
```

## 3. Flexible Database Contexts

AppBlueprint provides a tiered `DbContext` architecture to prevent consuming apps from inheriting tables they don't need.

### Why this improves stability:
*   **Separation of Concerns:** A microservice only needs `BaselineDbContext` (Auth, Webhooks), while a full platform might need `ApplicationDbContext`.
*   **Migration Safety:** Consuming apps can safely add their own `DbSet<T>` properties to their custom context without conflicting with AppBlueprint's core tables.

### Integration Best Practice:
Consuming apps should inherit from the lowest necessary tier:
```csharp
public class FinancialDbContext : B2BDbContext 
{
    public DbSet<Projection> Projections => Set<Projection>();
}
```

## 4. Strict Nullability & Guard Clauses

AppBlueprint enforces strict nullability (`<Nullable>enable</Nullable>`) and runtime guard clauses at trust boundaries.

### Why this improves stability:
*   **Predictable Failures:** `ArgumentNullException.ThrowIfNull()` ensures that invalid data passed from a consuming app fails immediately with a clear stack trace, rather than causing a `NullReferenceException` deep within the AppBlueprint logic.
*   **API Contracts:** Consuming apps know exactly which parameters are optional (`?`) and which are required.

## 5. Central Package Management

AppBlueprint uses `Directory.Packages.props` to manage NuGet versions centrally.

### Why this improves stability:
*   **Dependency Hell Prevention:** Consuming apps that reference multiple AppBlueprint modules (e.g., `Domain` and `Infrastructure`) are guaranteed to receive the exact same versions of transitive dependencies (like EF Core or OpenTelemetry).
*   **Security:** Vulnerable packages can be updated in one place, instantly securing all modules.

## 6. API Client SDK Generation (Kiota)

AppBlueprint provides a pre-generated SDK (`AppBlueprint.Api.Client.Sdk`) using Microsoft Kiota.

### Why this improves stability:
*   **Strongly Typed Contracts:** Consuming frontends (React, Vue, MAUI) or external microservices don't need to guess API routes or payload structures.
*   **Built-in Resilience:** The SDK automatically handles retries, timeouts, and token injection via `AspNetCoreKiotaAuthenticationProvider`.

### Integration Best Practice:
Consuming apps should always use the SDK rather than raw `HttpClient` calls:
```csharp
var user = await apiClient.Api.V1.Users["me"].GetAsync();
```
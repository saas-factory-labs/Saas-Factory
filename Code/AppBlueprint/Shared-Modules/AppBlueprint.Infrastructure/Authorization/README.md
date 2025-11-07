# Authentication Provider System

This system provides a pluggable authentication architecture that allows easy switching between different authentication providers while keeping the code clean, separate, and maintainable.

## Supported Providers

- **Mock Provider**: For development and testing
- **Auth0 Provider**: Integration with Auth0 service
- **Logto Provider**: Integration with Logto service

## Configuration

### 1. Basic Provider Selection

Set the authentication provider in your `appsettings.json`:

```json
{
  "Authentication": {
    "Provider": "Mock"  // Options: Mock, Auth0, Logto
  }
}
```

### 2. Auth0 Configuration

```json
{
  "Authentication": {
    "Provider": "Auth0",
    "Auth0": {
      "Domain": "https://your-domain.auth0.com",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "Audience": "your-api-identifier"
    }
  }
}
```

### 3. Logto Configuration

```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://your-logto-endpoint.com",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "Scope": "openid profile email"
    }
  }
}
```

### 4. Development/Mock Configuration

```json
{
  "Authentication": {
    "Provider": "Mock"
  }
}
```

## Usage

The system maintains compatibility with existing code through the `IUserAuthenticationProvider` interface:

```csharp
@inject IUserAuthenticationProvider AuthProvider

// Login
var success = await AuthProvider.LoginAsync(email, password);

// Check authentication status
var isAuthenticated = AuthProvider.IsAuthenticated();

// Logout
await AuthProvider.LogoutAsync();
```

## Adding a New Provider

To add a new authentication provider:

1. Create a new folder under `Providers/` (e.g., `Providers/Firebase/`)

2. Create a configuration class:
```csharp
public class FirebaseConfiguration
{
    public string ApiKey { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    // ... other properties
}
```

3. Create the provider class inheriting from `BaseAuthenticationProvider`:
```csharp
public class FirebaseProvider : BaseAuthenticationProvider
{
    // Implement required methods
    public override Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Implementation
    }
    
    // ... other methods
}
```

4. Update `AuthenticationProviderType` enum in `AuthenticationProviderFactory.cs`

5. Add the provider to the factory's switch statement

6. Update configuration documentation

## Architecture

- **IAuthenticationProvider**: Core interface for all providers
- **BaseAuthenticationProvider**: Abstract base class with common functionality
- **AuthenticationProviderFactory**: Factory for creating provider instances
- **UserAuthenticationProviderAdapter**: Adapter to maintain compatibility with existing interface
- **Provider-specific implementations**: Separate classes for each authentication service

## Benefits

- **Clean Separation**: Each provider is isolated in its own namespace
- **Easy Switching**: Change provider by updating configuration
- **Maintainable**: New providers can be added without affecting existing code
- **Testable**: Mock provider available for testing and development
- **Backwards Compatible**: Existing code continues to work unchanged
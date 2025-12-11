# AppBlueprint.Api.Client.Sdk

[![NuGet Version](https://img.shields.io/nuget/v/SaaS-Factory.AppBlueprint.Api.Client.Sdk)](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.Api.Client.Sdk)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Overview

The AppBlueprint.Api.Client.Sdk is a type-safe, auto-generated API client for consuming AppBlueprint REST APIs. Built with Microsoft Kiota, it provides a fluent, strongly-typed interface for all API endpoints with full IntelliSense support.

## Features

### Type-Safe API Client
- **Auto-generated** from OpenAPI specifications
- **Strongly-typed** requests and responses
- **Full IntelliSense** support in Visual Studio/VS Code
- **Async/await** pattern throughout
- **Automatic serialization** of requests and responses

### Built with Microsoft Kiota
- **Modern HTTP client** with built-in retry policies
- **Authentication** integration with multiple providers
- **Request builders** for fluent API composition
- **Middleware pipeline** for cross-cutting concerns
- **Error handling** with typed exceptions

### Blazor-Friendly
- **Works seamlessly** in Blazor Server and WebAssembly
- **HttpClient** dependency injection compatible
- **Authentication state** integration
- **Minimal bundle size** for WASM scenarios

### API Coverage
- ✅ Authentication & Authorization endpoints
- ✅ User and Profile management
- ✅ Team and Tenant operations (B2B)
- ✅ Payment and subscription APIs
- ✅ All baseline CRUD operations

## Installation

```bash
dotnet add package SaaS-Factory.AppBlueprint.Api.Client.Sdk
```

## Dependencies

This package includes:
- **Microsoft.Kiota.Abstractions** - Core Kiota types
- **Microsoft.Kiota.Http.HttpClientLibrary** - HTTP client implementation
- **Microsoft.Kiota.Serialization.Json** - JSON serialization
- **Microsoft.Kiota.Serialization.Form** - Form data serialization
- **Microsoft.Kiota.Serialization.Text** - Text serialization
- **Microsoft.Kiota.Serialization.Multipart** - Multipart form data

## Quick Start

### Basic Setup

```csharp
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

// 1. Create authentication provider
var authProvider = new AnonymousAuthenticationProvider();
// Or use API key authentication:
// var authProvider = new ApiKeyAuthenticationProvider("api-key", "X-API-Key", KeyLocation.Header);

// 2. Create request adapter
var adapter = new HttpClientRequestAdapter(authProvider)
{
    BaseUrl = "https://api.yourapp.com"
};

// 3. Create API client
var client = new ApiClient(adapter);

// 4. Make API calls
var users = await client.Users.GetAsync();
```

### With Dependency Injection (Blazor/ASP.NET Core)

```csharp
// Program.cs or Startup.cs
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

builder.Services.AddScoped<IAuthenticationProvider>(sp =>
    new AnonymousAuthenticationProvider());

builder.Services.AddScoped<IRequestAdapter>(sp =>
{
    var authProvider = sp.GetRequiredService<IAuthenticationProvider>();
    return new HttpClientRequestAdapter(authProvider)
    {
        BaseUrl = "https://api.yourapp.com"
    };
});

builder.Services.AddScoped<ApiClient>(sp =>
{
    var adapter = sp.GetRequiredService<IRequestAdapter>();
    return new ApiClient(adapter);
});
```

### With Authentication

```csharp
using Microsoft.Kiota.Abstractions.Authentication;

// Bearer token authentication
public class BearerTokenAuthenticationProvider : IAuthenticationProvider
{
    private readonly string _token;

    public BearerTokenAuthenticationProvider(string token)
    {
        _token = token;
    }

    public Task AuthenticateRequestAsync(RequestInformation request, 
        Dictionary<string, object>? additionalAuthenticationContext = null, 
        CancellationToken cancellationToken = default)
    {
        request.Headers.Add("Authorization", $"Bearer {_token}");
        return Task.CompletedTask;
    }
}

// Usage
var authProvider = new BearerTokenAuthenticationProvider("your-jwt-token");
var adapter = new HttpClientRequestAdapter(authProvider)
{
    BaseUrl = "https://api.yourapp.com"
};
var client = new ApiClient(adapter);
```

## Usage Examples

### User Management

```csharp
// Get all users
var users = await client.Users.GetAsync();

// Get user by ID
var user = await client.Users[userId].GetAsync();

// Create new user
var createRequest = new CreateUserRequest
{
    Email = "user@example.com",
    FirstName = "John",
    LastName = "Doe"
};
var newUser = await client.Users.PostAsync(createRequest);

// Update user
var updateRequest = new UpdateUserRequest
{
    FirstName = "Jane",
    LastName = "Smith"
};
await client.Users[userId].PatchAsync(updateRequest);

// Delete user
await client.Users[userId].DeleteAsync();
```

### Profile Management

```csharp
// Get current user profile
var profile = await client.Profile.GetAsync();

// Update profile
var updateProfile = new UpdateProfileRequest
{
    FirstName = "John",
    LastName = "Doe",
    DateOfBirth = new DateTime(1990, 1, 1),
    Gender = "Male"
};
await client.Profile.PatchAsync(updateProfile);
```

### Team Management (B2B)

```csharp
// Get teams
var teams = await client.Teams.GetAsync();

// Create team
var createTeam = new CreateTeamRequest
{
    Name = "Engineering",
    Description = "Engineering team"
};
var team = await client.Teams.PostAsync(createTeam);

// Add team member
var inviteMember = new InviteTeamMemberRequest
{
    Email = "member@example.com",
    Role = "Developer"
};
await client.Teams[teamId].Members.PostAsync(inviteMember);

// Get team members
var members = await client.Teams[teamId].Members.GetAsync();
```

### Authentication

```csharp
// Login
var loginRequest = new LoginRequest
{
    Email = "user@example.com",
    Password = "password123"
};
var loginResponse = await client.Auth.Login.PostAsync(loginRequest);
var accessToken = loginResponse.AccessToken;

// Logout
await client.Auth.Logout.PostAsync();

// Refresh token
var refreshRequest = new RefreshTokenRequest
{
    RefreshToken = "refresh-token-here"
};
var refreshResponse = await client.Auth.Refresh.PostAsync(refreshRequest);
```

### Query Parameters and Filtering

```csharp
// Get users with query parameters
var users = await client.Users.GetAsync(requestConfiguration =>
{
    requestConfiguration.QueryParameters.Skip = 0;
    requestConfiguration.QueryParameters.Take = 10;
    requestConfiguration.QueryParameters.Filter = "isActive eq true";
    requestConfiguration.QueryParameters.OrderBy = "createdAt desc";
});
```

### Error Handling

```csharp
using Microsoft.Kiota.Abstractions;

try
{
    var user = await client.Users[userId].GetAsync();
}
catch (ApiException ex) when (ex.ResponseStatusCode == 404)
{
    Console.WriteLine("User not found");
}
catch (ApiException ex) when (ex.ResponseStatusCode == 401)
{
    Console.WriteLine("Unauthorized - please login");
}
catch (ApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

### Custom Headers

```csharp
var users = await client.Users.GetAsync(requestConfiguration =>
{
    requestConfiguration.Headers.Add("X-Custom-Header", "value");
    requestConfiguration.Headers.Add("X-Tenant-Id", tenantId);
});
```

### Cancellation Tokens

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    var users = await client.Users.GetAsync(cancellationToken: cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request timed out");
}
```

## Blazor Integration

### Blazor Server

```razor
@page "/users"
@inject ApiClient ApiClient

<h3>Users</h3>

@if (users == null)
{
    <p>Loading...</p>
}
else
{
    <ul>
        @foreach (var user in users)
        {
            <li>@user.FirstName @user.LastName</li>
        }
    </ul>
}

@code {
    private List<UserResponse>? users;

    protected override async Task OnInitializedAsync()
    {
        users = await ApiClient.Users.GetAsync();
    }
}
```

### Blazor WebAssembly

```csharp
// Program.cs
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

builder.Services.AddScoped<IAuthenticationProvider, BlazorAuthenticationProvider>();

builder.Services.AddScoped<IRequestAdapter>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var authProvider = sp.GetRequiredService<IAuthenticationProvider>();
    
    return new HttpClientRequestAdapter(authProvider, httpClient: httpClient)
    {
        BaseUrl = "https://api.yourapp.com"
    };
});

builder.Services.AddScoped<ApiClient>();
```

## Advanced Configuration

### Custom HTTP Client

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(60)
};

httpClient.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");

var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient)
{
    BaseUrl = "https://api.yourapp.com"
};
```

### Middleware Pipeline

```csharp
// Add custom middleware to the request pipeline
public class LoggingMiddleware : IMiddleware
{
    public Task InvokeAsync(RequestContext context, Func<Task> next)
    {
        Console.WriteLine($"Request: {context.Request.Method} {context.Request.Uri}");
        return next();
    }
}

// Register middleware
adapter.Middleware.Add(new LoggingMiddleware());
```

### Retry Policy

```csharp
using Polly;

var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

// Apply to HTTP client
var handler = new PolicyHttpMessageHandler(retryPolicy);
var httpClient = new HttpClient(handler);
```

## API Documentation

For complete API documentation, visit the [OpenAPI specification](https://api.yourapp.com/swagger).

## Code Generation

This SDK is auto-generated using Microsoft Kiota:

```bash
# Regenerate SDK from OpenAPI spec
kiota generate -l CSharp -c ApiClient -n AppBlueprint.Api.Client.Sdk -d openapi.json -o ./Generated
```

## Performance Tips

1. **Reuse HttpClient** - Don't create new instances for each request
2. **Use cancellation tokens** - Allow long-running requests to be cancelled
3. **Implement caching** - Cache frequently accessed data
4. **Batch requests** - Group multiple operations when possible
5. **Configure timeouts** - Set appropriate timeouts for your use case

## Troubleshooting

### Common Issues

**401 Unauthorized**
- Ensure authentication token is valid
- Check token hasn't expired
- Verify bearer token format

**404 Not Found**
- Verify base URL is correct
- Check endpoint path
- Ensure resource exists

**Timeout Errors**
- Increase HTTP client timeout
- Use cancellation tokens
- Check network connectivity

## Migration Guide

### From REST API to SDK

**Before (Direct HTTP)**
```csharp
var httpClient = new HttpClient();
var response = await httpClient.GetAsync("https://api.yourapp.com/users");
var json = await response.Content.ReadAsStringAsync();
var users = JsonSerializer.Deserialize<List<User>>(json);
```

**After (Using SDK)**
```csharp
var users = await client.Users.GetAsync();
```

## Related Packages

- **AppBlueprint.Contracts** - Shared DTOs used by this SDK
- **AppBlueprint.Application** - Server-side application layer
- **AppBlueprint.Presentation.ApiModule** - API endpoints consumed by this SDK

## Contributing

This package is part of the SaaS Factory Labs AppBlueprint template. Contributions are welcome!

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Update OpenAPI spec if needed
5. Regenerate SDK
6. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/saas-factory-labs/Saas-Factory/blob/main/LICENSE) file for details.

## Support

- 📚 [Documentation](https://github.com/saas-factory-labs/Saas-Factory)
- 🐛 [Issues](https://github.com/saas-factory-labs/Saas-Factory/issues)
- 💬 [Discussions](https://github.com/saas-factory-labs/Saas-Factory/discussions)
- 📖 [Kiota Documentation](https://learn.microsoft.com/en-us/openapi/kiota/)

## Links

- [Source Code](https://github.com/saas-factory-labs/Saas-Factory)
- [SaaS Factory Labs](https://github.com/saas-factory-labs)
- [NuGet Package](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.Api.Client.Sdk)
- [Microsoft Kiota](https://github.com/microsoft/kiota)

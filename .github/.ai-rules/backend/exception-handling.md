# Exception Handling

**Philosophy**: Avoid try-catch blocks. Rely on global exception handling for consistency and simplicity.

## Core Principle

> **"Avoid try-catch unless we cannot fix the root cause—global exception handling covers unknown exceptions"**

Based on [PlatformPlatform's approach](https://github.com/platformplatform/PlatformPlatform/tree/main/application/shared-kernel/SharedKernel/Middleware/GlobalExceptionHandler.cs).

## Global Exception Handler

We have a `GlobalExceptionHandler` in `AppBlueprint.Presentation.ApiModule/Middleware/GlobalExceptionHandler.cs` that:
- Catches ALL unhandled exceptions at the middleware level
- Returns structured Problem Details (RFC 7807) responses
- Logs exceptions with trace IDs
- Handles specific exception types: `ArgumentNullException`, `ArgumentException`, `InvalidOperationException`, `TimeoutException`, `UnauthorizedAccessException`

## When to Use try-catch

**✅ ONLY use try-catch for:**

1. **External HTTP API calls** - to handle network failures gracefully
2. **Specific recoverable scenarios** - where you can provide a fallback value (e.g., return `null`)
3. **Never to suppress errors** - always log exceptions before returning fallback values

**❌ NEVER use try-catch for:**

1. **Business logic validation** - use guard clauses and return early instead
2. **General error handling** - rely on `GlobalExceptionHandler`
3. **Authentication/authorization errors** - let them bubble up to global handler
4. **Database operations** - let EF Core exceptions bubble up
5. **API controllers/endpoints** - global handler will catch everything

## Recommended Patterns

### ✅ Pattern 1: No try-catch - Let Global Handler Catch

```csharp
// ✅ Correct - No try-catch, global handler will catch any exceptions
public sealed class UpdateUserHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        
        if (user is null)
        {
            return Result.NotFound($"User with ID '{command.UserId}' not found.");
        }
        
        user.UpdateEmail(command.Email);
        await userRepository.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
```

### ✅ Pattern 2: External HTTP Calls - try-catch with Logging

```csharp
// ✅ Correct - External integration with try-catch for network failures
public sealed class GravatarClient(HttpClient httpClient, ILogger<GravatarClient> logger)
{
    public async Task<Gravatar?> GetGravatar(UserId userId, string email, CancellationToken cancellationToken)
    {
        try
        {
            var hash = Convert.ToHexString(MD5.HashData(Encoding.ASCII.GetBytes(email)));
            var response = await httpClient.GetAsync($"avatar/{hash.ToLowerInvariant()}?d=404", cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                logger.LogInformation("No Gravatar found for user {UserId}", userId);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Failed to fetch Gravatar for user {UserId}. Status Code: {StatusCode}", userId, response.StatusCode);
                return null;
            }

            return new Gravatar(await response.Content.ReadAsStreamAsync(cancellationToken), response.Content.Headers.ContentType?.MediaType ?? "image/png");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Network error fetching Gravatar for user {UserId}", userId);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            logger.LogWarning(ex, "Gravatar request timed out for user {UserId}", userId);
            return null;
        }
    }
}
```

### ❌ Anti-Pattern 1: Generic Catch in Business Logic

```csharp
// ❌ WRONG - Don't use try-catch for business logic
public async Task<TodoResponse?> GetTodoAsync(string tenantId)
{
    try
    {
        var response = await _httpClient.GetAsync(new Uri($"api/todos/{tenantId}", UriKind.Relative));
        
        if (!response.IsSuccessStatusCode)
        {
            return null; // ❌ Hiding errors instead of letting global handler log them
        }
        
        return await response.Content.ReadFromJsonAsync<TodoResponse>();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting todos"); // ❌ Redundant - global handler will log
        return null;
    }
}

// ✅ Better - Let exceptions bubble up (or catch specific exceptions only)
public async Task<TodoResponse?> GetTodoAsync(string tenantId)
{
    var response = await _httpClient.GetAsync(new Uri($"api/todos/{tenantId}", UriKind.Relative));
    
    if (!response.IsSuccessStatusCode)
    {
        _logger.LogWarning("Failed to get todos for tenant {TenantId}. Status: {StatusCode}", tenantId, response.StatusCode);
        return null;
    }
    
    return await response.Content.ReadFromJsonAsync<TodoResponse>();
}
```

### ❌ Anti-Pattern 2: Empty Catch Blocks

```csharp
// ❌ WRONG - Silently swallowing exceptions
public async Task<User?> GetUserAsync(string userId)
{
    try
    {
        return await _repository.GetByIdAsync(userId);
    }
    catch
    {
        return null; // ❌ Silent failure - no logging
    }
}

// ✅ Better - Remove try-catch or log before returning
public async Task<User?> GetUserAsync(string userId)
{
    return await _repository.GetByIdAsync(userId);
}
```

### ❌ Anti-Pattern 3: Catching to Re-throw

```csharp
// ❌ WRONG - Catching just to re-throw
public async Task DeleteUserAsync(string userId)
{
    try
    {
        await _repository.DeleteAsync(userId);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to delete user");
        throw; // ❌ Global handler will already log this
    }
}

// ✅ Better - Remove try-catch entirely
public async Task DeleteUserAsync(string userId)
{
    await _repository.DeleteAsync(userId);
}
```

## Exception Types We Handle Globally

The `GlobalExceptionHandler` specifically handles:

| Exception Type | HTTP Status | Description |
|---------------|-------------|-------------|
| `ArgumentNullException` | 400 Bad Request | Missing required parameter |
| `ArgumentException` | 400 Bad Request | Invalid parameter value |
| `InvalidOperationException` | 400 Bad Request | Invalid operation |
| `TimeoutException` | 504 Gateway Timeout | Request timeout |
| `UnauthorizedAccessException` | 401 Unauthorized | Authentication required |
| All others | 500 Internal Server Error | Unexpected errors |

## When try-catch IS Appropriate

### External Service Integrations

```csharp
// ✅ HTTP clients calling external APIs
public async Task<AuthToken?> ExchangeCodeAsync(string code)
{
    try
    {
        var response = await _httpClient.PostAsync(...);
        return await response.Content.ReadFromJsonAsync<AuthToken>();
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "Failed to exchange authorization code");
        return null;
    }
}
```

### Graceful Degradation

```csharp
// ✅ Optional feature that shouldn't break the main flow
public async Task<string?> GetUserAvatarUrl(string email)
{
    try
    {
        return await _gravatarClient.GetAvatarUrl(email);
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Failed to fetch Gravatar, using default avatar");
        return null; // Graceful fallback
    }
}
```

## Summary

1. **Default**: No try-catch - let `GlobalExceptionHandler` handle everything
2. **External HTTP calls**: Use try-catch with specific exception types (`HttpRequestException`, `TaskCanceledException`)
3. **Always log** before returning fallback values
4. **Never suppress** errors silently
5. **Trust the global handler** for consistency and proper logging

## Further Reading

- [PlatformPlatform Backend Rules](https://github.com/platformplatform/PlatformPlatform/blob/main/.agent/rules/backend/backend.md)
- [PlatformPlatform GlobalExceptionHandler](https://github.com/platformplatform/PlatformPlatform/blob/main/application/shared-kernel/SharedKernel/Middleware/GlobalExceptionHandler.cs)
- [Microsoft Exception Handling Best Practices](https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)

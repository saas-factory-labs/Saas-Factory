---
description: Mandatory SonarCloud quality rules to prevent code smells
globs:
  - "**/*.cs"
alwaysApply: true
---

# SonarCloud Quality Rules

These rules are mandatory to prevent SonarCloud code smells and security findings.

## S2933 — Mark fields as `readonly`

Always mark fields as `readonly` if they are only assigned in the constructor or field initializer.

```csharp
// ✅ Correct
private readonly ILogger<MyService> _logger;

// ❌ Incorrect
private ILogger<MyService> _logger;
```

## S2325 — Avoid duplicate method calls

Do not create methods that simply call identical methods. Extract shared logic to a common method.

```csharp
// ❌ Duplicate methods
public void MethodA() => SharedLogic();
public void MethodB() => SharedLogic();

// ✅ Single method or polymorphism
public void ExecuteLogic() => SharedLogic();
```

## S3260 — Seal private classes

Mark non-derived private classes as `sealed` for performance and to communicate they are not designed for inheritance.

```csharp
// ✅ Correct
private sealed class InternalHelper { }

// ❌ Incorrect
private class InternalHelper { }
```

## S2139 — Log or rethrow exceptions

Never swallow exceptions. Either log them with context or rethrow with additional information.

```csharp
// ✅ Correct
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to process user {UserId}", userId);
    throw new ProcessingException($"Failed to process user {userId}", ex);
}

// ❌ Swallows exception
catch (Exception ex) { }

// ❌ Rethrows without context
catch (Exception ex) { throw ex; }
```

## S1135 — Complete or remove TODO comments

Do not leave untracked TODO comments. Either implement immediately, create a GitHub issue and reference it, or remove.

```csharp
// ✅ Linked to issue
// TODO: Implement retry logic (tracked in #1234)

// ❌ Vague and untracked
// TODO: fix this later
```

## S1172 — Remove unused method parameters

If a method parameter is not used, remove it. If it must exist for interface compliance, prefix with underscore.

```csharp
// ✅ Parameter used
public void Process(string userId) => _repository.Get(userId);

// ✅ Intentionally unused (interface requirement)
public override void OnEvent(object _sender, EventArgs _e) => ProcessEvent();

// ❌ Unused parameter
public void Process(string userId) => _repository.GetAll();
```

## S1192 — Extract duplicate string literals to constants

When a string literal is used 3+ times, extract it to a named constant.

```csharp
// ✅ Correct
private const string UserNotFoundMessage = "User not found";
throw new NotFoundException(UserNotFoundMessage);
_logger.LogWarning(UserNotFoundMessage);

// ❌ Duplicated 3+ times
throw new NotFoundException("User not found");
_logger.LogWarning("User not found");
return new ErrorResult("User not found");
```

## S1144 — Remove unused private members

Delete unused private methods, properties, and fields. If keeping for future use, move to a separate branch or track with a GitHub issue reference.

## S6562 — Always specify `DateTimeKind`

When creating `DateTime` instances, always specify `DateTimeKind.Utc` or `DateTimeKind.Local`. Prefer UTC for storage and business logic.

```csharp
// ✅ Correct
var now = DateTime.UtcNow;
var specific = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

// ❌ Ambiguous kind
var now = DateTime.Now;
var specific = new DateTime(2024, 1, 1, 0, 0, 0);
```

## S3776 — Cognitive Complexity limit

Keep method cognitive complexity below 15. If exceeded, refactor by extracting complex conditions into named methods, breaking into smaller methods, or simplifying logic.

## S107 — Method parameter limit

Limit methods to 7 or fewer parameters. If more are needed, use a DTO or builder pattern.

```csharp
// ✅ Correct — use DTO
public void SendNotification(NotificationRequest request) { }

// ❌ Too many parameters
public void SendNotification(string to, string from, string subject, string body, bool isHtml, int priority, DateTime scheduledTime, string templateId) { }
```

## ASP0015 — Use typed header properties

Use typed header properties instead of string literals for common HTTP headers.

```csharp
// ✅ Correct
context.Response.Headers.ContentType = "application/json";
context.Response.Headers.CacheControl = "no-cache";

// ❌ String literals
context.Response.Headers["Content-Type"] = "application/json";
context.Response.Headers["Cache-Control"] = "no-cache";
```

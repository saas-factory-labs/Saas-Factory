---
description: C# coding standards and conventions
globs:
  - "**/*.cs"
alwaysApply: true
---

# Code Style

## C# Language Features

- Always use these features:
  - Top-level namespaces.
  - Primary constructors.
  - Array initializers.
  - Pattern matching with `is null` and `is not null` instead of `== null` and `!= null`.
- Records for immutable types.
- Mark all C# types as sealed.
- Use `var` when possible, **except when using null-coalescing operators (`??`)** — in those cases, always use explicit types (e.g., `string userId = User.FindFirst("sub")?.Value ?? "unknown"` instead of `var userId = ...`) for better readability and type clarity.
- Use List collection types like `List<UserId>` instead of `UserId[]` whenever possible.
- Set namespace in `.cs` files according to the folder structure of the file.
- Remove unnecessary using statements when adding or editing a file.

## Naming and Comments

- Use clear names instead of making comments.
- Never use acronyms.
- Don't add comments unless the code is truly not expressing the intent.
- Add Summary comments to describe methods and classes and their intent to make intellisense more useful.
- When using `FluentRegex`, always include the standard regular expression string as a comment just above the definition for easier comparison and testing.

## Formatting

- JetBrains/VS Code tooling is used for automatically formatting code, but automatic line breaking has been disabled for more readable code:
  - Wrap lines if "new language" constructs are started after 120 characters.
- Run `dotnet format` to format code according to the `.editorconfig` file.
- Read the `.editorconfig` file to understand the coding style that must be followed.

## Null Checks and Guard Clauses

Use `ArgumentNullException.ThrowIfNull()` for guard clauses and `is null`/`is not null` for logic checks. Do **not** use `??` or `??=` operators for null checks.

**Add runtime guard clauses for:**
- `public`/`protected` methods (externally visible)
- Constructor parameters — especially for DI
- After deserialization from external sources
- Repository method parameters (trust boundary)
- Extension method `this` parameters
- Critical operations where null causes data corruption or security issues

**Skip runtime guard clauses for:**
- `private` methods (compile-time nullable reference types already apply)
- `internal` methods when `[InternalsVisibleTo]` controls all callers
- Parameters explicitly marked `?` (null is valid by design)
- Private methods called only from already-validated public methods

```csharp
// ✅ Public API boundary — requires runtime validation
public async Task UpdateProfileAsync(string userId, string firstName)
{
    ArgumentNullException.ThrowIfNull(userId);
    ArgumentNullException.ThrowIfNull(firstName);
    var user = await LoadUserAsync(userId);
    SaveChanges(user, firstName);
}

// ✅ Private helper — no runtime check needed
private async Task<UserEntity> LoadUserAsync(string userId)
    => await _db.Users.FindAsync(userId);

// ✅ Database trust boundary
public async Task<UserEntity?> GetUserByIdAsync(string userId)
{
    ArgumentNullException.ThrowIfNull(userId);
    return await _dbContext.Users.FindAsync(userId);
}
```

## Async/Await

- **Do NOT use `ConfigureAwait(false)`** in ASP.NET Core or Blazor Server code. ASP.NET Core has no `SynchronizationContext` since .NET Core 2.0, so there is no deadlock risk. Blazor Server UI code needs to return to the synchronization context to update the UI.

## String Operations

**Always specify `StringComparison` for string operations (CA1307/CA1310)** on technical/non-user-facing strings:

- Use `StringComparison.Ordinal` for: cookie names, header names, config keys, error messages, token strings, file paths, URLs
- Use `StringComparison.OrdinalIgnoreCase` for: file extensions, protocol names, case-insensitive config values

```csharp
// ✅ Correct
if (errorMessage.Contains("JavaScript interop", StringComparison.Ordinal))
if (key.StartsWith(".Token.", StringComparison.Ordinal))
string cleaned = key.Replace(".Token.", "", StringComparison.Ordinal);

// ❌ Incorrect
if (errorMessage.Contains("JavaScript interop"))
```

**Use `AsSpan` for temporary string slices to avoid heap allocations (CA1845):**

```csharp
// ✅ Zero-allocation
string preview = string.Concat(token.AsSpan(0, Math.Min(20, token.Length)), "...");

// ❌ Allocates substring on heap
string preview = token.Substring(0, Math.Min(20, token.Length)) + "...";
```

## Extracting Complex Conditions

When an `if` condition has more than one `&&` or `||` clause, or combines unrelated checks, extract it into a private method with a name that expresses the intent:

```csharp
// ❌ Inline complex condition — intent unclear
if (!string.IsNullOrEmpty(value) && value.Split('.').Length == 3 && value.StartsWith("ey", StringComparison.Ordinal))

// ✅ Extracted to named method
if (LooksLikeJwt(value))

private static bool LooksLikeJwt(string? value)
    => !string.IsNullOrEmpty(value)
        && value.Split('.').Length == 3
        && value.StartsWith("ey", StringComparison.Ordinal);
```

## Exceptions and Logging

- Avoid using exceptions for control flow.
- When exceptions are thrown, always use meaningful exceptions following .NET conventions.
- Use `UnreachableException` to signal code that cannot be reached by tests.
- Exception messages should include a period.
- Log only meaningful events at appropriate severity levels.
- Logging messages should not include a period.
- Use structured logging.
- Avoid try-catch unless we cannot fix the root cause — we have Global Exception handling for unknown exceptions.

## Environment Variables

All environment variables (Doppler secrets, `.env` files, AppHost configuration) **MUST** use UPPERCASE naming with single underscores (`_`) for word separation:

```
✅ LOGTO_RESOURCE, LOGTO_ENDPOINT, DATABASE_CONNECTION_STRING
✅ AUTHENTICATION_PROVIDER, API_BASE_URL
❌ Logto__Resource  (mixed case, double underscore)
❌ logto_resource   (lowercase)
❌ LOGTO__RESOURCE  (double underscores)
```

## SonarCloud Quality Rules

See [sonarcloud-quality.md](./sonarcloud-quality.md) for the full list of mandatory SonarCloud rules.

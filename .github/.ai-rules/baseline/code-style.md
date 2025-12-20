# Code Style

- Always use these features:
  - Top-level namespaces. 
  - Primary constructors.
  - Array initializers.
  - Pattern matching with `is null` and `is not null` instead of `== null` and `!= null`.
- Records for immutable types.
- Mark all C# types as sealed.
- Use `var` when possible, **except when using null-coalescing operators (`??`)** - in those cases, always use explicit types (e.g., `string userId = User.FindFirst("sub")?.Value ?? "unknown"` instead of `var userId = ...`) for better readability and type clarity.
- Use List collection types like `List<UserId>` instead of `UserId[]` when ever possible.
- JetBrain/VS Code tooling is used for automatically formatting code, but automatic line breaking has been disabled for more readable code:
  - Wrap lines if "new language" constructs are started after 120 characters. This ensures that no important code is hidden after the 120 character mark.
- Use clear names instead of making comments.
- Never use acronyms.
- Avoid using exceptions for control flow:
  - When exceptions are thrown, always use meaningful exceptions following .NET conventions.
  - Use `UnreachableException` to signal unreachable code, that cannot be reached by tests.
  - Exception messages should include a period.
- Log only meaningful events at appropriate severity levels.
  -Logging messages should not include a period.
  - Use structured logging.
- Never introduce new NuGet dependencies.
- Don't do defensive coding (e.g. do not add exception handling to handle situations we don't know will happen).
- Avoid try-catch, unless we cannot fix the reason. We have Global Exception handling to handle unknown exceptions.
# - Use SharedInfrastructureConfiguration.IsRunningInAzure to determine if we are running in Azure.
- Don't add comments unless the code is truly not expressing the intent.
- Add Summary comments to describe methods and classes and their intent to make intellisense more useful.

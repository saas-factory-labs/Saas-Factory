# Backend

When working with C# code, follow these rules very carefully.

## Code Style

- Always use these features:
  - Top-level namespaces. 
  - Primary constructors.
  - Array initializers.
  - Pattern matching with `is null` and `is not null` instead of `== null` and `!= null`.
- Records for immutable types.
- Mark all C# types as sealed.
- Use `var` when possible.
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

## Implementation

IMPORTANT: Always follow these steps very carefully when implementing changes:

1. Consult any relevant rules files listed below and start by listing which rule files have been used to guide your response (e.g., `Rules consulted: commands.md, unit-and-integration-tests.md`).
2. Always start new changes by writing new test cases (or change existing tests). Remember to consult [Unit and Integration Tests](./unit-and-integration-tests.md) for details.
3. Always run `dotnet build` or `dotnet test` to verify the code compiles after each change.
4. Fix any compiler warnings or test failures before moving on to the next step.

<!-- ## Backend Rules Files

- [API Endpoints](./api-endpoints.md) - Guidelines for minimal API endpoints.  
- [Commands](./commands.md) - Implementation of state-changing operations using CQRS commands.
- [Domain Modeling](./domain-modeling.md) - Implementation of DDD aggregates, entities, and value objects.
- [External Integrations](./external-integrations.md) - Implementation of integration to external services.
- [Queries](./queries.md) - Implementation of data retrieval operations using CQRS queries.
- [Repositories](./repositories.md) - Persistence abstractions for aggregates.
- [Strongly Typed IDs](./strongly-typed-ids.md) - Type-safe DDD identifiers for domain entities.
- [Telemetry Events](./telemetry-events.md) - Standardized observability event patterns.
- [Unit and Integration Tests](./unit-and-integration-tests.md) - Test suite patterns for commands, queries, and domain logic. -->

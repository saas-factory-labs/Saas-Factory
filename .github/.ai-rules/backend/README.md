# Backend

When working with C# code, follow these rules very carefully.

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

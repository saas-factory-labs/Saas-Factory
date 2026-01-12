# Frontend

When working with C# Blazor razor and code behind code as well as HTML, CSS and Javascript, follow these rules very carefully. 

## Implementation

IMPORTANT: Always follow these steps very carefully when implementing changes:

1. Consult any relevant rules files listed below and start by listing which rule files have been used to guide your response (e.g., `Rules consulted: commands.md, unit-and-integration-tests.md`).
2. Always start new changes by writing new test cases (or change existing tests). Remember to consult [Unit and Integration Tests](./unit-and-integration-tests.md) for details.
3. Always run `dotnet build` or `dotnet test` to verify the code compiles after each change.
4. Fix any compiler warnings or test failures before moving on to the next step.

## Frontend rules files

- **[Blazor JavaScript Interop](./blazor-javascript-interop.md) - CRITICAL: Prevents navigation hanging, JavaScript crashes, and render mode issues.**
- [Design review](./design-review.md) - Inspection of the live website using Playwright MCP
- [Design specification](./design-specification.md) - Specifications to adhere to for frontend design




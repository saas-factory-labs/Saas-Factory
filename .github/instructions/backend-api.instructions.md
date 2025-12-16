---
applies_to:
  - "Code/AppBlueprint/AppBlueprint.ApiService/**"
  - "Code/AppBlueprint/**/Controllers/**"
  - "Code/AppBlueprint/**/Endpoints/**"
---

# Backend API Development Instructions

When working with API controllers, endpoints, or the ApiService project:

## Quick Reference

- Consult [Backend Rules](../.ai-rules/backend/README.md) first
- Follow [API Controllers](../.ai-rules/backend/api-controllers.md) guidelines
- Review [Database Repositories](../.ai-rules/backend/database-repositories.md) for data access

## Key Principles

1. **Minimal API First**: Prefer minimal APIs over controller-based APIs
2. **CQRS Pattern**: Separate commands (writes) from queries (reads)
3. **Domain-Driven Design**: Keep business logic in the domain layer
4. **Validation**: Always validate input at the API boundary
5. **Error Handling**: Return appropriate HTTP status codes

## Testing

- Write integration tests for all API endpoints
- Test both success and error scenarios
- Verify response schemas and status codes
- Use TUnit and FluentAssertions

## Common Patterns

```csharp
// Minimal API endpoint example
app.MapPost("/api/users", async (CreateUserRequest request, IMediator mediator) =>
{
    var command = new CreateUserCommand(request.Email, request.Name);
    var result = await mediator.Send(command);
    return Results.Created($"/api/users/{result.Id}", result);
});
```

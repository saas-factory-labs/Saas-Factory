# Testing Rules

When writing tests for the AppBlueprint project, follow these rules very carefully.

## Testing Framework

The AppBlueprint project uses the following testing frameworks:

- **TUnit**: For unit tests and integration tests
- **bUnit**: For Blazor component/UI tests
- **FluentAssertions**: For test assertions
- **NSubstitute**: For mocking and substitutes

**IMPORTANT**: Do NOT use xUnit, NUnit, MSTest, or other testing frameworks unless explicitly requested.

## Test Organization

Tests are organized in the `AppBlueprint.Tests` project with the following structure:

- Unit tests for domain logic and business rules
- Integration tests for database operations and API endpoints
- Architecture tests to enforce architectural constraints
- Blazor UI tests for component behavior

## Test Files

- [Unit Tests](./unit-tests.md) - Guidelines for writing unit tests
- [Integration Tests](./integration-tests.md) - Guidelines for writing integration tests
- [Blazor Unit Tests](./blazor-unit-tests.md) - Guidelines for testing Blazor components with bUnit
- [Architecture Tests](./architecture-tests.md) - Guidelines for architecture constraint tests

## General Testing Guidelines

### Test Naming

- Use descriptive test method names that clearly state what is being tested
- Follow the pattern: `MethodName_Scenario_ExpectedBehavior`
- Example: `CreateUser_WithValidEmail_ReturnsNewUser`

### Test Structure

Follow the Arrange-Act-Assert (AAA) pattern:

```csharp
[Test]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var sut = CreateSystemUnderTest();
    var input = "test data";
    
    // Act
    var result = await sut.MethodName(input);
    
    // Assert
    result.Should().NotBeNull();
    result.Value.Should().Be("expected");
}
```

### FluentAssertions

Always use FluentAssertions for assertions:

```csharp
// Good
result.Should().NotBeNull();
result.Value.Should().Be(expected);
list.Should().HaveCount(3);
action.Should().Throw<InvalidOperationException>();

// Bad - do not use
Assert.NotNull(result);
Assert.Equal(expected, result.Value);
Assert.Equal(3, list.Count);
```

### Test Data

- Use meaningful test data that represents realistic scenarios
- Use Bogus library for generating fake data when appropriate
- Keep test data simple and focused on the scenario being tested
- Avoid magic numbers and strings - use constants or variables with clear names

### Mocking

- Use NSubstitute for creating mocks and substitutes
- Only mock dependencies, not the system under test
- Verify important interactions with mocks
- Keep mocking minimal - only mock what's necessary

### Test Independence

- Each test should be independent and isolated
- Tests should not depend on execution order
- Use proper setup and teardown for test fixtures
- Clean up test data after tests complete

### Integration Tests

- Use TestContainers or similar for database dependencies
- Ensure integration tests clean up after themselves
- Use separate test databases from development databases
- Test realistic scenarios that exercise multiple components

### Test Coverage

- Write tests for new code and modifications
- Focus on testing business logic and critical paths
- Don't aim for 100% coverage - focus on meaningful tests
- Test edge cases and error conditions

### Running Tests

- Run `dotnet test` to execute all tests
- Ensure all tests pass before committing changes
- Fix failing tests immediately - don't commit broken tests
- Pay attention to test output for warnings or issues

## What to Test

### Do Test

- Business logic and domain rules
- Error handling and validation
- Edge cases and boundary conditions
- Integration points between components
- Critical user workflows
- API endpoints and responses

### Don't Test

- Framework functionality (e.g., ASP.NET Core, EF Core internals)
- Third-party library behavior
- Trivial getters and setters
- Auto-generated code

## Test Quality

- Tests should be readable and easy to understand
- Tests should run quickly (especially unit tests)
- Tests should be deterministic (no flaky tests)
- Tests should provide clear failure messages
- Tests should be maintainable and easy to update

## Continuous Testing

- Run tests frequently during development
- Run tests before committing code
- Run tests in CI/CD pipeline
- Monitor test execution time and optimize slow tests

---
applies_to:
  - "Code/AppBlueprint/AppBlueprint.Tests/**"
  - "Code/AppBlueprint/**/*Tests.cs"
  - "Code/AppBlueprint/**/*.Tests/**"
---

# Testing Instructions

When writing or modifying tests:

## Quick Reference

- Consult [Testing Rules](../.ai-rules/tests/README.md) first
- Follow specific test type guidelines:
  - [Unit Tests](../.ai-rules/tests/unit-tests.md)
  - [Integration Tests](../.ai-rules/tests/integration-tests.md)
  - [Blazor Unit Tests](../.ai-rules/tests/blazor-unit-tests.md)
  - [Architecture Tests](../.ai-rules/tests/architecture-tests.md)

## Testing Framework

**REQUIRED**: Use TUnit, bUnit, and FluentAssertions ONLY.

❌ Do NOT use: xUnit, NUnit, MSTest

## Test Structure

```csharp
using TUnit.Core;
using FluentAssertions;

public class UserServiceTests
{
    [Test]
    public async Task CreateUser_WithValidData_ReturnsNewUser()
    {
        // Arrange
        var sut = new UserService();
        var email = "test@example.com";
        
        // Act
        var result = await sut.CreateUser(email);
        
        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
    }
}
```

## Best Practices

1. **Naming**: Use descriptive test names: `MethodName_Scenario_ExpectedBehavior`
2. **AAA Pattern**: Always use Arrange-Act-Assert structure
3. **Independence**: Each test should be isolated and independent
4. **Coverage**: Focus on testing behavior, not implementation details
5. **Speed**: Keep unit tests fast; use integration tests for slower operations

## FluentAssertions Examples

```csharp
// Value assertions
result.Should().Be(expected);
result.Should().NotBeNull();

// Collection assertions
list.Should().HaveCount(3);
list.Should().Contain(item);

// Exception assertions
action.Should().Throw<InvalidOperationException>()
      .WithMessage("Expected error message");

// Async assertions
await action.Should().ThrowAsync<ArgumentException>();
```

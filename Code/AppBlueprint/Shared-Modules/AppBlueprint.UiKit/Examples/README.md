# AppBlueprint.UiKit - Integration Test Examples

This directory contains **example code** demonstrating how to write integration tests for the AppBlueprint.UiKit NuGet package in consuming projects.

## Important Note

These files are **excluded from compilation** in the UiKit project itself. They are provided as reference/documentation for developers who want to test the UiKit package in their own projects.

## Usage

To use these examples in your test project:

1. Create a test project using TUnit:
   ```bash
   dotnet new tunit -n YourProject.IntegrationTests
   ```

2. Add required package references to your test project:
   ```xml
   <ItemGroup>
     <PackageReference Include="TUnit" />
     <PackageReference Include="TUnit.Assertions" />
     <PackageReference Include="bunit" />
     <PackageReference Include="FluentAssertions" />
   </ItemGroup>
   ```

3. Copy the example test file to your test project and adapt as needed.

## Test Framework

These examples use:
- **TUnit** - Modern, fast testing framework (replaces Xunit)
- **bUnit** - Blazor component testing library
- **FluentAssertions** - Fluent assertion syntax

## Example Files

- `IntegrationTest.Example.cs` - Comprehensive integration tests demonstrating:
  - Service registration verification
  - Theme configuration testing
  - Component rendering tests
  - Navigation and breadcrumb service tests
  - Performance considerations

## Reference

For more information about testing Blazor components with bUnit and TUnit, see:
- [bUnit Documentation](https://bunit.dev/)
- [TUnit Documentation](https://github.com/thomhurst/TUnit)
- [FluentAssertions Documentation](https://fluentassertions.com/)


using System.Reflection;
using AppBlueprint.Application.Services;
using AppBlueprint.Domain.Entities.User;
using FluentAssertions;
using NetArchTest.Rules;

namespace AppBlueprint.Tests.Layers;

/// <summary>
/// Architecture tests that enforce framework coupling boundaries.
/// Inner layers (Domain, Application) must not take hard dependencies on
/// infrastructure frameworks like EF Core or ASP.NET Core.
/// </summary>
internal sealed class FrameworkDependencyTests
{
    private static readonly Assembly DomainAssembly = typeof(UserEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(SignupService).Assembly;

    // -------------------------------------------------------------------------
    // Entity Framework Core — only Infrastructure should reference EF Core
    // -------------------------------------------------------------------------

    [Test]
    public void DomainLayer_ShouldNotDependOn_EntityFramework()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain must not reference EF Core — use SharedKernel base types only, " +
                     $"but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void DomainLayer_ShouldNotDependOn_NpgsqlEntityFramework()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("Npgsql.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain must not reference Npgsql EF provider, " +
                     $"but violations found: {FormatViolations(result)}");
    }

    // -------------------------------------------------------------------------
    // ASP.NET Core — Domain and Application must be framework-agnostic
    // -------------------------------------------------------------------------

    [Test]
    public void DomainLayer_ShouldNotDependOn_AspNetCore()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.AspNetCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain must be framework-agnostic and not reference ASP.NET Core, " +
                     $"but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void ApplicationLayer_ShouldNotDependOn_AspNetCore()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.AspNetCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Application must be framework-agnostic and not reference ASP.NET Core, " +
                     $"but violations found: {FormatViolations(result)}");
    }

    // -------------------------------------------------------------------------
    // External service SDKs — Domain should not couple to third-party SDKs
    // -------------------------------------------------------------------------

    [Test]
    public void DomainLayer_ShouldNotDependOn_Stripe()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("Stripe")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain must not reference the Stripe SDK directly, " +
                     $"but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void ApplicationLayer_ShouldNotDependOn_Stripe()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn("Stripe")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Application must not reference the Stripe SDK directly — use an interface, " +
                     $"but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void DomainLayer_ShouldNotDependOn_Newtonsoft()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("Newtonsoft.Json")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain must not reference Newtonsoft.Json, " +
                     $"but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void ApplicationLayer_ShouldNotDependOn_Newtonsoft()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn("Newtonsoft.Json")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Application must not reference Newtonsoft.Json — use System.Text.Json, " +
                     $"but violations found: {FormatViolations(result)}");
    }

    private static string FormatViolations(NetArchTest.Rules.TestResult result)
    {
        if (result.FailingTypes is null || result.FailingTypes.Count == 0)
            return "(none)";

        return string.Join(", ", result.FailingTypes.Select(t => t.Name));
    }
}

using System.Reflection;
using AppBlueprint.Application.Services;
using AppBlueprint.Domain.Entities.User;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;

namespace AppBlueprint.Tests.Layers;

/// <summary>
/// Architecture tests that enforce design rules across layers:
/// - FluentValidation validators belong in Application only
/// - Middleware belongs in Presentation only
/// - Controllers must be public and inherit ControllerBase
/// </summary>
internal sealed class DesignRuleTests
{
    private static readonly Assembly DomainAssembly = typeof(UserEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(SignupService).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(UserRepository).Assembly;
    private static readonly Assembly PresentationAssembly = typeof(AuthenticationController).Assembly;

    // -------------------------------------------------------------------------
    // Validator placement — validators belong exclusively in Application
    // -------------------------------------------------------------------------

    [Test]
    public void Validators_ShouldOnlyResideIn_ApplicationLayer()
    {
        IEnumerable<Type> outsideValidators = Types
            .InAssemblies([DomainAssembly, InfrastructureAssembly, PresentationAssembly])
            .That()
            .HaveNameEndingWith("Validator", StringComparison.Ordinal)
            .GetTypes();

        outsideValidators.Should().BeEmpty(
            because: "FluentValidation validators must only reside in the Application layer");
    }

    [Test]
    public void Validators_InApplicationLayer_ShouldEndWithValidator()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceContaining("Validations")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Validator", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all validator classes must end with 'Validator', but violations found: {FormatViolations(result)}");
    }

    // -------------------------------------------------------------------------
    // Middleware placement — middleware belongs exclusively in Presentation
    // -------------------------------------------------------------------------

    [Test]
    public void Middleware_ShouldOnlyResideIn_PresentationLayer()
    {
        IEnumerable<Type> outsideMiddleware = Types
            .InAssemblies([DomainAssembly, ApplicationAssembly, InfrastructureAssembly])
            .That()
            .HaveNameEndingWith("Middleware", StringComparison.Ordinal)
            .GetTypes();

        outsideMiddleware.Should().BeEmpty(
            because: "middleware classes must only reside in the Presentation layer");
    }

    // -------------------------------------------------------------------------
    // Controller design rules
    // -------------------------------------------------------------------------

    [Test]
    public void Controllers_ShouldBePublic()
    {
        var result = Types.InAssembly(PresentationAssembly)
            .That()
            .HaveNameEndingWith("Controller", StringComparison.Ordinal)
            .Should()
            .BePublic()
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all controllers must be public, but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void Controllers_ShouldInheritFrom_ControllerBase()
    {
        var result = Types.InAssembly(PresentationAssembly)
            .That()
            .HaveNameEndingWith("Controller", StringComparison.Ordinal)
            .Should()
            .Inherit(typeof(ControllerBase))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all controllers must inherit from ControllerBase, but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void Controllers_ShouldOnlyResideIn_PresentationLayer()
    {
        IEnumerable<Type> outsideControllers = Types
            .InAssemblies([DomainAssembly, ApplicationAssembly, InfrastructureAssembly])
            .That()
            .HaveNameEndingWith("Controller", StringComparison.Ordinal)
            .GetTypes();

        outsideControllers.Should().BeEmpty(
            because: "controller classes must only reside in the Presentation layer");
    }

    // -------------------------------------------------------------------------
    // Exception handler placement
    // -------------------------------------------------------------------------

    [Test]
    public void ExceptionHandlers_ShouldOnlyResideIn_PresentationLayer()
    {
        IEnumerable<Type> outsideHandlers = Types
            .InAssemblies([DomainAssembly, ApplicationAssembly, InfrastructureAssembly])
            .That()
            .HaveNameEndingWith("ExceptionHandler", StringComparison.Ordinal)
            .GetTypes();

        outsideHandlers.Should().BeEmpty(
            because: "exception handler classes must only reside in the Presentation layer");
    }

    private static string FormatViolations(NetArchTest.Rules.TestResult result)
    {
        if (result.FailingTypes is null || result.FailingTypes.Count == 0)
            return "(none)";

        return string.Join(", ", result.FailingTypes.Select(t => t.Name));
    }
}

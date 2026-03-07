using System.Reflection;
using AppBlueprint.Application.Services;
using AppBlueprint.Domain.Entities.User;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
using FluentAssertions;
using NetArchTest.Rules;

namespace AppBlueprint.Tests.Layers;

/// <summary>
/// Architecture tests that enforce naming conventions across all layers.
/// </summary>
internal sealed class NamingConventionTests
{
    private static readonly Assembly DomainAssembly = typeof(UserEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(SignupService).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(UserRepository).Assembly;
    private static readonly Assembly PresentationAssembly = typeof(AuthenticationController).Assembly;

    // -------------------------------------------------------------------------
    // Interfaces
    // -------------------------------------------------------------------------

    [Test]
    public void Interfaces_InDomainAssembly_ShouldStartWithI()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all Domain interfaces must start with 'I', but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void Interfaces_InApplicationAssembly_ShouldStartWithI()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all Application interfaces must start with 'I', but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void Interfaces_InInfrastructureAssembly_ShouldStartWithI()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all Infrastructure interfaces must start with 'I', but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void Interfaces_InPresentationAssembly_ShouldStartWithI()
    {
        var result = Types.InAssembly(PresentationAssembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all Presentation interfaces must start with 'I', but violations found: {FormatViolations(result)}");
    }

    // -------------------------------------------------------------------------
    // Repositories
    // -------------------------------------------------------------------------

    [Test]
    public void Classes_InRepositoriesNamespace_ShouldEndWithRepository()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .ResideInNamespaceContaining("Repositories")
            .And()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("Repository", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all repository classes must end with 'Repository', but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void Interfaces_InDomainRepositoriesNamespace_ShouldEndWithRepository()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespaceContaining("Repositories")
            .And()
            .AreInterfaces()
            .Should()
            .HaveNameEndingWith("Repository", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all domain repository interfaces must end with 'Repository', but violations found: {FormatViolations(result)}");
    }

    // -------------------------------------------------------------------------
    // Services
    // -------------------------------------------------------------------------

    [Test]
    public void Classes_InServicesNamespace_ShouldEndWithService()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceContaining("Services")
            .And()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("Service", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all service classes must end with 'Service', but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void Interfaces_InServicesNamespace_ShouldEndWithService()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceContaining("Services")
            .And()
            .AreInterfaces()
            .Should()
            .HaveNameEndingWith("Service", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all service interfaces must end with 'Service', but violations found: {FormatViolations(result)}");
    }

    // -------------------------------------------------------------------------
    // Controllers
    // -------------------------------------------------------------------------

    [Test]
    public void Classes_InControllersNamespace_ShouldEndWithController()
    {
        var result = Types.InAssembly(PresentationAssembly)
            .That()
            .ResideInNamespaceContaining("Controllers")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Controller", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all controller classes must end with 'Controller', but violations found: {FormatViolations(result)}");
    }

    // -------------------------------------------------------------------------
    // Entities
    // -------------------------------------------------------------------------

    [Test]
    public void Classes_InDomainEntitiesNamespace_ShouldEndWithEntity()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespaceContaining("Entities")
            .And()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("Entity", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all domain entity classes must end with 'Entity', but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void Classes_InInfrastructureEntitiesNamespace_ShouldEndWithEntity()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .ResideInNamespaceContaining("Entities")
            .And()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .And()
            .DoNotHaveNameEndingWith("EntityConfiguration", StringComparison.Ordinal)
            .Should()
            .HaveNameEndingWith("Entity", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all infrastructure entity classes must end with 'Entity', but violations found: {FormatViolations(result)}");
    }

    // -------------------------------------------------------------------------
    // Entity Configurations
    // -------------------------------------------------------------------------

    [Test]
    public void EntityConfigurations_ShouldEndWithEntityConfiguration()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .ImplementInterface(typeof(Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<>))
            .Should()
            .HaveNameEndingWith("EntityConfiguration", StringComparison.Ordinal)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"all EF entity configuration classes must end with 'EntityConfiguration', but violations found: {FormatViolations(result)}");
    }

    // -------------------------------------------------------------------------
    // Helper
    // -------------------------------------------------------------------------

    private static string FormatViolations(NetArchTest.Rules.TestResult result)
    {
        if (result.FailingTypes is null || result.FailingTypes.Count == 0)
            return "(none)";

        return string.Join(", ", result.FailingTypes.Select(t => t.Name));
    }
}

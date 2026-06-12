using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;

namespace AppBlueprint.Tests.Layers;

/// <summary>
/// Architecture tests that enforce clean architecture layer dependency rules.
/// Domain must not depend on Application, Infrastructure, or Presentation.
/// Application must not depend on Infrastructure or Presentation.
/// Infrastructure must not depend on Presentation.
/// SharedKernel must not depend on any other project layer.
/// </summary>
internal sealed class LayerTests
{
    private static readonly Assembly DomainAssembly = ArchitectureAssemblies.Domain;
    private static readonly Assembly ApplicationAssembly = ArchitectureAssemblies.Application;
    private static readonly Assembly PresentationAssembly = ArchitectureAssemblies.Presentation;
    private static readonly Assembly SharedKernelAssembly = ArchitectureAssemblies.SharedKernel;

    [Test]
    public void DomainLayer_ShouldNotHaveDependencyOn_ApplicationLayer()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain must not depend on Application, but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(ArchitectureAssemblies.InfrastructureNames)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain must not depend on any Infrastructure assembly, but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void DomainLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Domain must not depend on Presentation, but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAny(ArchitectureAssemblies.InfrastructureNames)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Application must not depend on any Infrastructure assembly, but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Application must not depend on Presentation, but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void InfrastructureLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        var result = Types.InAssemblies(ArchitectureAssemblies.Infrastructure)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Infrastructure must not depend on Presentation, but violations found: {FormatViolations(result)}");
    }

    // -------------------------------------------------------------------------
    // SharedKernel isolation — must not depend on any project layer
    // -------------------------------------------------------------------------

    [Test]
    public void SharedKernelLayer_ShouldNotHaveDependencyOn_DomainLayer()
    {
        var result = Types.InAssembly(SharedKernelAssembly)
            .Should()
            .NotHaveDependencyOn(DomainAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"SharedKernel must not depend on Domain, but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void SharedKernelLayer_ShouldNotHaveDependencyOn_ApplicationLayer()
    {
        var result = Types.InAssembly(SharedKernelAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"SharedKernel must not depend on Application, but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void SharedKernelLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        var result = Types.InAssembly(SharedKernelAssembly)
            .Should()
            .NotHaveDependencyOnAny(ArchitectureAssemblies.InfrastructureNames)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"SharedKernel must not depend on any Infrastructure assembly, but violations found: {FormatViolations(result)}");
    }

    [Test]
    public void SharedKernelLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        var result = Types.InAssembly(SharedKernelAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"SharedKernel must not depend on Presentation, but violations found: {FormatViolations(result)}");
    }

    private static string FormatViolations(NetArchTest.Rules.TestResult result)
    {
        if (result.FailingTypes is null || result.FailingTypes.Count == 0)
            return "(none)";

        return string.Join(", ", result.FailingTypes.Select(t => t.Name));
    }
}

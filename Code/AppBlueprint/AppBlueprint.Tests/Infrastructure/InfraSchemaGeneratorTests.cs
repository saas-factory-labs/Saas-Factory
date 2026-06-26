using System.Reflection;
using FluentAssertions;

namespace AppBlueprint.Tests.Infrastructure;

/// <summary>
/// <c>InfraSchemaGenerator</c> is internal to the CLI, so it is exercised via reflection — matching
/// the existing convention used by <c>SystemCommandLineCompatibilityTests</c>.
/// </summary>
internal sealed class InfraSchemaGeneratorTests
{
    [Test]
    public void GenerateJson_ProducesSchemaWithExpectedDefinitions()
    {
        System.Reflection.Assembly cli = System.Reflection.Assembly.Load("AppBlueprint.DeveloperCli");
        Type generator = cli.GetType("AppBlueprint.DeveloperCli.Commands.InfraSchemaGenerator")!;
        MethodInfo generate = generator.GetMethod("GenerateJson", BindingFlags.Public | BindingFlags.Static)!;

        string json = (string)generate.Invoke(null, null)!;

        json.Should().Contain("AppInfrastructureConfig");
        json.Should().Contain("CloudProvider");
        json.Should().Contain("AppId");
        json.Should().Contain("Cloudflare");
    }
}

using AppBlueprint.Infrastructure.Core.Configuration;
using NJsonSchema;
using NJsonSchema.Generation;

namespace AppBlueprint.DeveloperCli.Commands;

/// <summary>
/// Generates the JSON Schema for <see cref="AppInfrastructureConfig"/> so that a SaaS app's
/// <c>infra.json</c> gets full IDE IntelliSense by referencing it via <c>$schema</c>.
/// </summary>
internal static class InfraSchemaGenerator
{
    /// <summary>Conventional file name for the generated schema.</summary>
    public const string DefaultSchemaFileName = "app-infra-schema.json";

    /// <summary>Reflects the config contract and returns the JSON Schema document as a string.</summary>
    public static string GenerateJson()
    {
        var settings = new SystemTextJsonSchemaGeneratorSettings();
        JsonSchema schema = JsonSchema.FromType<AppInfrastructureConfig>(settings);
        return schema.ToJson();
    }
}

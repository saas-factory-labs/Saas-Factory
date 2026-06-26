using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace AppBlueprint.Infrastructure.Core.Configuration;

/// <summary>
/// Loads and validates a SaaS application's <c>infra.json</c> into an <see cref="AppInfrastructureConfig"/>.
/// Deserialization is case-insensitive (PascalCase is the schema-canonical form), tolerates comments
/// and trailing commas, and surfaces clear, actionable errors for malformed or incomplete documents.
/// </summary>
public static class AppInfrastructureConfigLoader
{
    /// <summary>Conventional file name an app declares its infrastructure in.</summary>
    public const string DefaultFileName = "infra.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        // Explicit reflection-based resolver so the loader works regardless of the host application's
        // JsonSerializerIsReflectionEnabledByDefault setting (e.g. trimmed/single-file consumers).
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    /// <summary>Parses and validates <c>infra.json</c> content.</summary>
    /// <exception cref="InvalidOperationException">The JSON is malformed, missing required fields, or invalid.</exception>
    public static AppInfrastructureConfig Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        AppInfrastructureConfig? config;
        try
        {
            config = JsonSerializer.Deserialize<AppInfrastructureConfig>(json, SerializerOptions);
        }
        catch (JsonException ex)
        {
            // Covers malformed JSON, missing 'required' members (AppId/Provider), and invalid enum values.
            throw new InvalidOperationException($"{DefaultFileName} could not be parsed: {ex.Message}", ex);
        }

        if (config is null)
        {
            throw new InvalidOperationException($"{DefaultFileName} deserialized to null.");
        }

        Validate(config);
        return config;
    }

    /// <summary>Reads, parses and validates an <c>infra.json</c> file at <paramref name="path"/>.</summary>
    /// <exception cref="FileNotFoundException">No file exists at <paramref name="path"/>.</exception>
    public static AppInfrastructureConfig LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"No {DefaultFileName} found at '{path}'.", path);
        }

        return Parse(File.ReadAllText(path));
    }

    /// <summary>Loads <c>infra.json</c> from <paramref name="directory"/> (typically the app's root).</summary>
    public static AppInfrastructureConfig LoadFromDirectory(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        return LoadFromFile(Path.Combine(directory, DefaultFileName));
    }

    private static void Validate(AppInfrastructureConfig config)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(config.AppId))
        {
            errors.Add("'AppId' is required and cannot be empty.");
        }

        if (config.Compute is null || config.Compute.Count == 0)
        {
            errors.Add("At least one 'Compute' workload must be declared.");
        }
        else
        {
            foreach (ComputeArgs workload in config.Compute)
            {
                if (string.IsNullOrWhiteSpace(workload.Name))
                {
                    errors.Add("A 'Compute' workload is missing its 'Name'.");
                }

                if (string.IsNullOrWhiteSpace(workload.Image))
                {
                    string label = string.IsNullOrWhiteSpace(workload.Name) ? "(unnamed)" : workload.Name;
                    errors.Add($"Compute workload '{label}' is missing its 'Image'.");
                }
            }
        }

        if (config.Storage is not null)
        {
            foreach (string bucket in config.Storage.Buckets)
            {
                if (string.IsNullOrWhiteSpace(bucket))
                {
                    errors.Add("A 'Storage.Buckets' entry is empty.");
                }
            }
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                $"{DefaultFileName} is invalid:{Environment.NewLine} - " +
                string.Join($"{Environment.NewLine} - ", errors));
        }
    }
}

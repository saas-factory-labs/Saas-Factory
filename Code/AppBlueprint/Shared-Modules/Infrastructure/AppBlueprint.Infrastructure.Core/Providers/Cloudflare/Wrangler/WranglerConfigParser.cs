using Tomlyn;
using Tomlyn.Model;

namespace AppBlueprint.Infrastructure.Core.Providers.Cloudflare.Wrangler;

/// <summary>
/// Parses an existing <c>wrangler.toml</c> into a <see cref="WranglerConfig"/>. Reads into Tomlyn's
/// native <see cref="TomlTable"/> model (no source-generated context required), pulling only the
/// keys the infrastructure engine bridges.
/// </summary>
public static class WranglerConfigParser
{
    /// <summary>Conventional Cloudflare worker config file name.</summary>
    public const string DefaultFileName = "wrangler.toml";

    /// <summary>Parses <c>wrangler.toml</c> content into a <see cref="WranglerConfig"/>.</summary>
    /// <exception cref="InvalidOperationException">The TOML is malformed.</exception>
    public static WranglerConfig Parse(string toml)
    {
        ArgumentNullException.ThrowIfNull(toml);

        TomlTable? model;
        try
        {
            model = TomlSerializer.Deserialize<TomlTable>(toml, new TomlSerializerOptions());
        }
        catch (TomlException ex)
        {
            throw new InvalidOperationException($"{DefaultFileName} could not be parsed: {ex.Message}", ex);
        }

        if (model is null)
        {
            throw new InvalidOperationException($"{DefaultFileName} could not be parsed to a table.");
        }

        return new WranglerConfig
        {
            Name = GetString(model, "name"),
            Main = GetString(model, "main"),
            AccountId = GetString(model, "account_id"),
            CompatibilityDate = GetString(model, "compatibility_date"),
            CompatibilityFlags = GetStringArray(model, "compatibility_flags"),
            R2Buckets = GetTableArray(model, "r2_buckets")
                .Select(t => new WranglerR2Binding(
                    GetString(t, "binding") ?? string.Empty,
                    GetString(t, "bucket_name") ?? string.Empty))
                .ToList(),
            HyperdriveBindings = GetTableArray(model, "hyperdrive")
                .Select(t => new WranglerHyperdriveBinding(
                    GetString(t, "binding") ?? string.Empty,
                    GetString(t, "id") ?? string.Empty))
                .ToList(),
            Vars = GetVars(model)
        };
    }

    /// <summary>Parses <c>wrangler.toml</c> from <paramref name="directory"/>, or returns null if absent.</summary>
    public static WranglerConfig? TryLoadFromDirectory(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);

        string path = Path.Combine(directory, DefaultFileName);
        return File.Exists(path) ? Parse(File.ReadAllText(path)) : null;
    }

    private static string? GetString(TomlTable table, string key)
        => table.TryGetValue(key, out object? value) && value is string s ? s : null;

    private static IReadOnlyList<string> GetStringArray(TomlTable table, string key)
        => table.TryGetValue(key, out object? value) && value is TomlArray array
            ? array.OfType<string>().ToList()
            : [];

    private static IEnumerable<TomlTable> GetTableArray(TomlTable table, string key)
        => table.TryGetValue(key, out object? value) && value is TomlTableArray array
            ? array.OfType<TomlTable>()
            : [];

    private static IReadOnlyDictionary<string, string> GetVars(TomlTable model)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        if (model.TryGetValue("vars", out object? value) && value is TomlTable vars)
        {
            foreach (KeyValuePair<string, object> entry in vars)
            {
                if (entry.Value is string s)
                {
                    result[entry.Key] = s;
                }
            }
        }

        return result;
    }
}

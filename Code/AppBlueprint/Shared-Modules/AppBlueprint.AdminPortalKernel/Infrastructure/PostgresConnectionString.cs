using System.Text;
using System.Text.RegularExpressions;

namespace AppBlueprint.AdminPortalKernel.Infrastructure;

/// <summary>
/// Normalizes PostgreSQL connection strings. Npgsql's connection-string builder only
/// understands keyword=value form, but cloud providers (Neon, Railway) hand out
/// <c>postgresql://user:pass@host/db?sslmode=require&amp;channel_binding=require</c> URIs.
/// Per-app module connection strings are typically copied straight from those providers,
/// so the kernel accepts both forms.
/// </summary>
internal static class PostgresConnectionString
{
    /// <summary>
    /// Redacts the password from a connection string for safe display, handling both the
    /// <c>postgresql://user:pass@host</c> URI form and the <c>...;Password=pass;...</c>
    /// keyword form. Returns "(not configured)" for empty input.
    /// </summary>
    public static string Mask(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return "(not configured)";
        }

        // URI userinfo: scheme://user:password@host -> scheme://user:***@host
        string masked = Regex.Replace(
            connectionString,
            "(?<=://)([^:/@\\s]+):([^@\\s]+)@",
            "$1:***@",
            RegexOptions.None,
            TimeSpan.FromSeconds(1));

        // Keyword form: Password=... / Pwd=... -> ***
        masked = Regex.Replace(
            masked,
            "(?i)\\b(password|pwd)\\s*=\\s*[^;]*",
            "$1=***",
            RegexOptions.None,
            TimeSpan.FromSeconds(1));

        return masked;
    }

    public static string Normalize(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        if (!connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
            && !connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            // Already keyword=value form — pass through unchanged.
            return connectionString;
        }

        var uri = new Uri(connectionString);
        string[] userInfo = uri.UserInfo.Split(':', 2);

        var builder = new StringBuilder();
        builder.Append($"Host={uri.Host}");
        if (uri.Port > 0)
        {
            builder.Append($";Port={uri.Port}");
        }

        if (userInfo.Length > 0 && !string.IsNullOrEmpty(userInfo[0]))
        {
            builder.Append($";Username={Uri.UnescapeDataString(userInfo[0])}");
        }

        if (userInfo.Length > 1)
        {
            builder.Append($";Password={Uri.UnescapeDataString(userInfo[1])}");
        }

        string database = uri.AbsolutePath.TrimStart('/');
        if (!string.IsNullOrEmpty(database))
        {
            builder.Append($";Database={database}");
        }

        if (!string.IsNullOrEmpty(uri.Query))
        {
            foreach (string pair in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = pair.Split('=', 2);
                if (parts.Length != 2)
                {
                    continue;
                }

                // Only translate keywords Npgsql recognises; e.g. channel_binding is not one, so skip it.
                if (parts[0].Equals("sslmode", StringComparison.OrdinalIgnoreCase))
                {
                    string sslMode = Uri.UnescapeDataString(parts[1]).ToLowerInvariant() switch
                    {
                        "require" => "Require",
                        "verify-ca" => "VerifyCA",
                        "verify-full" => "VerifyFull",
                        "prefer" => "Prefer",
                        "allow" => "Allow",
                        "disable" => "Disable",
                        var other => other
                    };
                    builder.Append($";SSL Mode={sslMode}");
                }
            }
        }

        return builder.ToString();
    }
}

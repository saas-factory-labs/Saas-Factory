namespace DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context;

internal static class PostgresConnectionStringHelper
{
    /// <summary>
    /// Npgsql 10 does not parse postgresql:// URIs — converts to keyword=value format.
    /// Passes keyword=value strings through unchanged.
    /// </summary>
    internal static string Normalize(string connectionString)
    {
        if (!connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            return connectionString;

        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':', 2);

        var sb = new System.Text.StringBuilder();
        sb.Append($"Host={uri.Host}");
        if (uri.Port > 0) sb.Append($";Port={uri.Port}");
        if (userInfo.Length > 0 && !string.IsNullOrEmpty(userInfo[0]))
            sb.Append($";Username={Uri.UnescapeDataString(userInfo[0])}");
        if (userInfo.Length > 1)
            sb.Append($";Password={Uri.UnescapeDataString(userInfo[1])}");

        var database = uri.AbsolutePath.TrimStart('/');
        if (!string.IsNullOrEmpty(database))
            sb.Append($";Database={database}");

        if (!string.IsNullOrEmpty(uri.Query))
        {
            foreach (var pair in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = pair.Split('=', 2);
                if (parts.Length != 2) continue;
                switch (parts[0].ToLowerInvariant())
                {
                    case "sslmode":
                        // Npgsql requires specific capitalisation — "require" won't be recognised
                        var sslMode = Uri.UnescapeDataString(parts[1]).ToLowerInvariant() switch
                        {
                            "require"     => "Require",
                            "verify-ca"   => "VerifyCA",
                            "verify-full" => "VerifyFull",
                            "prefer"      => "Prefer",
                            "allow"       => "Allow",
                            "disable"     => "Disable",
                            var other     => other
                        };
                        sb.Append($";SSL Mode={sslMode}");
                        break;
                    // channel_binding is not a recognised Npgsql keyword — skip it
                }
            }
        }

        return sb.ToString();
    }
}

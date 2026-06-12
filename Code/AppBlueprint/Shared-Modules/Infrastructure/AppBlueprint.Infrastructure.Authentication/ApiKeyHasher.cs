using System.Security.Cryptography;
using System.Text;

namespace AppBlueprint.Infrastructure.Authentication;

/// <summary>
/// Generates and hashes API keys. Generation and verification MUST share the same hashing
/// routine, so both the issuing controller and <see cref="ApiKeyAuthenticationHandler"/>
/// depend on this single helper.
/// </summary>
public static class ApiKeyHasher
{
    /// <summary>Human-readable prefix that identifies AppBlueprint API keys.</summary>
    public const string KeyPrefix = "abk_";

    private const int SecretByteLength = 32; // 256 bits of entropy

    /// <summary>
    /// Generates a new cryptographically random raw API key. The raw value is shown to the
    /// caller exactly once and is never stored; only its hash is persisted.
    /// </summary>
    public static string GenerateRawKey()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(SecretByteLength);
        return KeyPrefix + Base64UrlEncode(randomBytes);
    }

    /// <summary>Computes a lowercase hex SHA-256 hash of <paramref name="input"/>.</summary>
    public static string ComputeSha256Hex(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}

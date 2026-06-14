using System.Security.Cryptography;
using System.Text;

namespace AppBlueprint.AdminPortalKernel.Security;

/// <summary>Device characteristics captured from the request used to fingerprint an admin session.</summary>
public sealed record DeviceSignals(string? UserAgent, string? IpAddress, string? AcceptLanguage, string? SecChUa);

/// <summary>An opaque, stable hash identifying the device/connection an admin acts from.</summary>
public sealed record DeviceFingerprint(string Value);

/// <summary>
/// Produces a SHA256 fingerprint from request characteristics (User-Agent + IP + browser hints),
/// per the admin-access-security-hardening reference. Used to detect when an admin session is
/// resumed from a different device/connection.
/// </summary>
public interface IDeviceFingerprintService
{
    DeviceFingerprint Compute(DeviceSignals signals);
}

/// <inheritdoc />
public sealed class DeviceFingerprintService : IDeviceFingerprintService
{
    public DeviceFingerprint Compute(DeviceSignals signals)
    {
        ArgumentNullException.ThrowIfNull(signals);

        string combined = string.Join('|',
            Normalize(signals.UserAgent),
            Normalize(signals.IpAddress),
            Normalize(signals.AcceptLanguage),
            Normalize(signals.SecChUa));

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(combined));
        return new DeviceFingerprint(Convert.ToBase64String(hash));
    }

    private static string Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "unknown" : value.Trim();
}

using System.Net;
using System.Net.Sockets;

namespace AppBlueprint.Application.Validations;

/// <summary>
/// Validates user-supplied webhook target URLs to prevent Server-Side Request Forgery (SSRF).
/// Only absolute http/https URLs that resolve to public IP addresses are permitted; loopback,
/// private, link-local (incl. cloud metadata 169.254.169.254), and unique-local targets are rejected.
/// </summary>
public static class WebhookUrlValidator
{
    /// <summary>
    /// Attempts to validate <paramref name="url"/> as a safe outbound webhook target.
    /// Returns false and sets <paramref name="error"/> when the URL is malformed, uses a
    /// disallowed scheme, or points at an internal/metadata address.
    /// </summary>
    public static bool TryValidate(string? url, out string? error)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            error = "Webhook URL is required.";
            return false;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            error = "Webhook URL must be an absolute URL.";
            return false;
        }

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            error = "Webhook URL must use http or https.";
            return false;
        }

        // Resolve the host to one or more IP addresses and reject any that are non-public.
        // A literal IP host resolves to itself; a DNS name is resolved so rebinding to an
        // internal address is also blocked.
        IPAddress[] addresses;
        try
        {
            addresses = uri.HostNameType is UriHostNameType.IPv4 or UriHostNameType.IPv6
                ? [IPAddress.Parse(uri.Host)]
                : Dns.GetHostAddresses(uri.Host);
        }
        catch (SocketException)
        {
            error = "Webhook URL host could not be resolved.";
            return false;
        }
        catch (ArgumentException)
        {
            error = "Webhook URL host is invalid.";
            return false;
        }

        if (addresses.Length == 0)
        {
            error = "Webhook URL host could not be resolved.";
            return false;
        }

        foreach (IPAddress address in addresses)
        {
            if (IsBlockedIpAddress(address))
            {
                error = "Webhook URL must not target an internal, loopback, or metadata address.";
                return false;
            }
        }

        error = null;
        return true;
    }

    /// <summary>
    /// Returns true when <paramref name="address"/> falls in a range that must not be reachable
    /// from a webhook: loopback, private, link-local, unique-local, or otherwise non-public.
    /// </summary>
    public static bool IsBlockedIpAddress(IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (IPAddress.IsLoopback(address)) return true;

        // Normalise IPv4-mapped IPv6 (::ffff:a.b.c.d) to IPv4 before range checks.
        if (address.IsIPv4MappedToIPv6) address = address.MapToIPv4();

        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            byte[] bytes = address.GetAddressBytes();

            // 10.0.0.0/8
            if (bytes[0] == 10) return true;
            // 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return true;
            // 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168) return true;
            // 169.254.0.0/16 (link-local, includes cloud metadata 169.254.169.254)
            if (bytes[0] == 169 && bytes[1] == 254) return true;
            // 100.64.0.0/10 (carrier-grade NAT)
            if (bytes[0] == 100 && bytes[1] >= 64 && bytes[1] <= 127) return true;
            // 0.0.0.0/8
            if (bytes[0] == 0) return true;

            return false;
        }

        if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            if (address.IsIPv6LinkLocal || address.IsIPv6SiteLocal || address.IsIPv6UniqueLocal) return true;

            // fc00::/7 unique-local (covered by IsIPv6UniqueLocal) and ::/128 unspecified.
            if (address.Equals(IPAddress.IPv6Any)) return true;

            return false;
        }

        // Unknown address families are treated as unsafe.
        return true;
    }
}

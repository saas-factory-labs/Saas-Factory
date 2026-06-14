using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace AppBlueprint.AdminPortalKernel.Security;

/// <summary>
/// Issues single-use, short-lived cryptographic nonces per administrative transaction and
/// rejects replays, per the token-replay mitigation in the security reference.
/// </summary>
public interface IAdminNonceService
{
    /// <summary>Generates a fresh base64 nonce (256 bits of entropy).</summary>
    string GenerateNonce();

    /// <summary>
    /// Atomically consumes a nonce for the given admin. Returns true on first use within the
    /// TTL; false if the nonce was already used or has expired (replay / stale).
    /// </summary>
    bool TryConsume(string adminUserId, string nonce);
}

/// <summary>
/// In-memory implementation. State is per-process: replay protection holds within a single
/// instance and resets on restart. Swap for a distributed cache when running multi-instance.
/// </summary>
public sealed class AdminNonceService : IAdminNonceService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
    private readonly ConcurrentDictionary<string, DateTimeOffset> _consumed = new(StringComparer.Ordinal);
    private readonly TimeProvider _timeProvider;

    public AdminNonceService(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
    }

    public string GenerateNonce() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    public bool TryConsume(string adminUserId, string nonce)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(adminUserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(nonce);

        DateTimeOffset now = _timeProvider.GetUtcNow();
        EvictExpired(now);

        string key = $"{adminUserId}:{nonce}";
        // GetOrAdd is not sufficient on its own (it cannot tell us whether we added or matched),
        // so use TryAdd: it returns false when the key already exists, i.e. a replay.
        return _consumed.TryAdd(key, now.Add(Ttl));
    }

    private void EvictExpired(DateTimeOffset now)
    {
        foreach (KeyValuePair<string, DateTimeOffset> entry in _consumed)
        {
            if (entry.Value <= now)
            {
                _consumed.TryRemove(entry.Key, out _);
            }
        }
    }
}

using System.Collections.Concurrent;

namespace AppBlueprint.AdminPortalKernel.Security;

/// <summary>
/// Progressive lockout for repeated failed admin authentication attempts, per the
/// credential-stuffing/brute-force mitigation: 5 failures = 15 min, 10 = 1 hour, 15 = 24 hours.
/// Recording attempts is driven by the authentication flow; the access guard additionally
/// refuses data access while an identity is locked out (defense in depth).
/// </summary>
public interface IAdminAccountLockoutService
{
    /// <summary>True when <paramref name="identifier"/> is currently locked out; sets the release time.</summary>
    bool IsLockedOut(string identifier, out DateTimeOffset lockoutUntil);

    /// <summary>Records a failed attempt and escalates the lockout window when thresholds are crossed.</summary>
    void RecordFailedAttempt(string identifier);

    /// <summary>Clears all failure state for an identifier after a successful authentication.</summary>
    void Reset(string identifier);
}

/// <summary>
/// In-memory implementation keyed by email/username. State is per-process and resets on restart;
/// swap for a distributed store when running multi-instance.
/// </summary>
public sealed class AdminAccountLockoutService : IAdminAccountLockoutService
{
    private readonly ConcurrentDictionary<string, FailureState> _state = new(StringComparer.OrdinalIgnoreCase);
    private readonly TimeProvider _timeProvider;

    public AdminAccountLockoutService(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
    }

    public bool IsLockedOut(string identifier, out DateTimeOffset lockoutUntil)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        lockoutUntil = DateTimeOffset.MinValue;

        if (_state.TryGetValue(identifier, out FailureState? state))
        {
            lock (state)
            {
                if (state.LockoutUntil > _timeProvider.GetUtcNow())
                {
                    lockoutUntil = state.LockoutUntil;
                    return true;
                }
            }
        }

        return false;
    }

    public void RecordFailedAttempt(string identifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        FailureState state = _state.GetOrAdd(identifier, static _ => new FailureState());

        lock (state)
        {
            state.Count++;
            DateTimeOffset now = _timeProvider.GetUtcNow();

            if (state.Count >= 15)
            {
                state.LockoutUntil = now.AddHours(24);
            }
            else if (state.Count >= 10)
            {
                state.LockoutUntil = now.AddHours(1);
            }
            else if (state.Count >= 5)
            {
                state.LockoutUntil = now.AddMinutes(15);
            }
        }
    }

    public void Reset(string identifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        _state.TryRemove(identifier, out _);
    }

    private sealed class FailureState
    {
        public int Count { get; set; }
        public DateTimeOffset LockoutUntil { get; set; }
    }
}

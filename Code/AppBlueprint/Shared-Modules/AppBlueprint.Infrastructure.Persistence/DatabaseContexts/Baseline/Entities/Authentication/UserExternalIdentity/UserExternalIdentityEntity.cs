using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authentication.AuthenticationProvider;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authentication.UserExternalIdentity;

/// <summary>
/// Links a user to an external authentication provider identity.
/// Enables multi-provider support — a single user can log in via Logto, Auth0, Firebase, etc.
/// </summary>
public class UserExternalIdentityEntity
{
    public int Id { get; set; }

    /// <summary>
    /// The internal user this identity belongs to.
    /// </summary>
    public required string UserId { get; set; }
    public UserEntity? User { get; set; }

    /// <summary>
    /// The authentication provider (Logto, Auth0, Firebase, etc.)
    /// </summary>
    public int AuthenticationProviderId { get; set; }
    public AuthenticationProviderEntity? AuthenticationProvider { get; set; }

    /// <summary>
    /// The user's unique ID in the external provider (e.g., Logto 'sub', Auth0 user_id, Firebase localId).
    /// </summary>
    public required string ExternalUserId { get; set; }

    /// <summary>
    /// The email associated with this external identity (may differ from the user's primary email).
    /// </summary>
    public string? ExternalEmail { get; set; }

    /// <summary>
    /// Display name from the external provider.
    /// </summary>
    public string? ExternalDisplayName { get; set; }

    public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}

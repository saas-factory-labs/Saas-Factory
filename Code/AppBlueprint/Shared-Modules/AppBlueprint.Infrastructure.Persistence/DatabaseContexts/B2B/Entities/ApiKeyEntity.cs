using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;

public sealed class ApiKeyEntity : BaseEntity, ITenantScoped
{
    public ApiKeyEntity()
    {
        Id = PrefixedUlid.Generate("api");
    }

    public required UserEntity Owner { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    public required string SecretRef { get; set; } // saved in azure keyvault

    /// <summary>SHA-256 hex hash of the raw API key for fast, safe database lookup.</summary>
    public string? KeyHash { get; set; }

    /// <summary>Optional expiry. Null means the key never expires.</summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>When true the key is revoked and must be rejected during authentication.</summary>
    public bool IsRevoked { get; set; }

    public string UserId { get; set; } = string.Empty;

    public required string TenantId { get; set; }
}

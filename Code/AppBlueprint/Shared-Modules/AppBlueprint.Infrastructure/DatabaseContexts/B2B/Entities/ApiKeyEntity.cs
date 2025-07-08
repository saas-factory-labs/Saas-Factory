using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
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

    public string UserId { get; set; } = string.Empty;

    public required string TenantId { get; set; }
}

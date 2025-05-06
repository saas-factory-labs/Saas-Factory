using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;

public class ApiKeyEntity
{
    public ApiKeyEntity()
    {
        CreatedAt = DateTime.Now;
    }

    public int Id { get; set; }

    public required UserEntity Owner { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string SecretRef { get; set; } // saved in azure keyvault

    public int UserId { get; set; }


    public DateTime CreatedAt { get; set; }
}

using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Affiliate.Models;

public class ReferralModel
{
    public ReferralModel()
    {
        Owner = new UserEntity
        {
            Id = PrefixedUlid.Generate("user"),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            UserName = "johndoe",
            Profile = new ProfileEntity()
        };
    }

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public UserEntity Owner { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}

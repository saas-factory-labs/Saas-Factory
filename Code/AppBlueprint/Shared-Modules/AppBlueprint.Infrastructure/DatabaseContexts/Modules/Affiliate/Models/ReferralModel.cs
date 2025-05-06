using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

// using Shared.Models;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Affiliate.Models;

public class ReferralModel
{
    public ReferralModel()
    {
        Owner = new UserEntity
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            UserName = "johndoe",
            Profile = new ProfileEntity()
        };
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public UserEntity Owner { get; set; }
    public int OwnerId { get; set; }
}

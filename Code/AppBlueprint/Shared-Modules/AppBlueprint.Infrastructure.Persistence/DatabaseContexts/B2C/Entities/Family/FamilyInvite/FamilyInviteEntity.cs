using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.Profile;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyInvite;

// Represents an invitation to join a family
public class FamilyInviteEntity : BaseEntity
{
    public FamilyInviteEntity()
    {
        Id = PrefixedUlid.Generate("finv");
        Family = new FamilyEntity();
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

    // Family the user is invited to
    public string FamilyId { get; set; } = string.Empty;
    public FamilyEntity Family { get; set; }

    // User receiving the invitation
    public string UserId { get; set; } = string.Empty; public UserEntity Owner { get; set; }

    public DateTime ExpireAt { get; set; }
    public bool IsActive { get; set; }
}

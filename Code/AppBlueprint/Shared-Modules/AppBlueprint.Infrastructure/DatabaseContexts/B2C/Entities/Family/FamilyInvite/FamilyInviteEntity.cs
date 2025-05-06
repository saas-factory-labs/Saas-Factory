using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyInvite;

// Represents an invitation to join a family
public class FamilyInviteEntity
{
    public FamilyInviteEntity()
    {
        Family = new FamilyEntity();
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

    // Family the user is invited to
    public int FamilyId { get; set; }
    public FamilyEntity Family { get; set; }

    // User receiving the invitation
    public int UserId { get; set; }
    public UserEntity Owner { get; set; }

    public DateTime ExpireAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

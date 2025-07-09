using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class NotificationEntity : BaseEntity
{
    public NotificationEntity()
    {
        Id = PrefixedUlid.Generate("notif");
        User = new UserEntity
        {
            FirstName = "FirstName",
            LastName = "LastName",
            UserName = "UserName",
            Email = "Email",
            Profile = new ProfileEntity()
        };
    }

    public required string OwnerId { get; set; }

    public required string Title { get; set; }
    public required string Message { get; set; }
    public bool IsRead { get; set; }

    // Navigation properties
    public required string UserId { get; set; }
    public UserEntity User { get; set; }
}

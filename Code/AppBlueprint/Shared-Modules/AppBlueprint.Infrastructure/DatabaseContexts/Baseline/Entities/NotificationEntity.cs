using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class NotificationEntity
{
    public NotificationEntity()
    {
        User = new UserEntity
        {
            FirstName = "FirstName",
            LastName = "LastName",
            UserName = "UserName",
            Email = "Email",
            Profile = new ProfileEntity()
        };
    }

    public int Id { get; set; }
    public int OwnerId { get; set; }

    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }

    // Navigation properties
    public int UserId { get; set; }
    public UserEntity User { get; set; }
}

using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

// using Shared.Models;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Chat.Models;

public class ChatMessageModel
{
    public ChatMessageModel()
    {
        Sender = new UserEntity
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Id = 1,
            CreatedAt = DateTime.Now,
            IsActive = true,
            UserName = "johndoe",
            Profile = new ProfileEntity()
        };

        Chat = new ChatModel();
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public ChatModel Chat { get; set; }

    public UserEntity Sender { get; set; }

    public UserEntity Owner { get; set; }
    public int OwnerId { get; set; }
}

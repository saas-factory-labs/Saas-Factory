using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

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
            Id = PrefixedUlid.Generate("user"),
            CreatedAt = DateTime.Now,
            IsActive = true,
            UserName = "johndoe",
            Profile = new ProfileEntity()
        };

        Chat = new ChatModel();
    }

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public ChatModel Chat { get; set; }

    public UserEntity Sender { get; set; }

    public UserEntity Owner { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}

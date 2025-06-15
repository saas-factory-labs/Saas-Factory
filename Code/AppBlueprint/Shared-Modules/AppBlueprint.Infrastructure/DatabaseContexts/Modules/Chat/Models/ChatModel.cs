using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

// using Shared.Models;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Chat.Models;

public class ChatModel
{
    public ChatModel()    {
        Owner = new UserEntity
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
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

    public List<ChatMessageModel> ChatMessages { get; set; }
}

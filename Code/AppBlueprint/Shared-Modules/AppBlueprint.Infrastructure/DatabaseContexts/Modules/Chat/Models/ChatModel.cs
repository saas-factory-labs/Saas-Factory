using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Chat.Models;

public class ChatModel: BaseEntity
{
    public ChatModel()
    {
        Owner = new UserEntity
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            UserName = "johndoe",
            Profile = new ProfileEntity()
        };
    }

    public required string Name { get; set; }

    public required bool IsActive { get; set; }
    public string? Description { get; set; }
    
    public required UserEntity Owner { get; set; }
    public required string OwnerId { get; set; } 

    public List<ChatMessageModel>? ChatMessages { get; set; }
}

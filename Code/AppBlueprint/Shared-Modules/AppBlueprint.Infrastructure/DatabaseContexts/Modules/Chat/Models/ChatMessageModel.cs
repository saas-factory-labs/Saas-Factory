using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Chat.Models;

public class ChatMessageModel: BaseEntity
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

        Chat = new ChatModel
        {
            Name = "Default Chat",
            IsActive = true,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now,
            Owner = new UserEntity
            {
                Email = "chat-owner@test.com",
                FirstName = "ChatOwner",
                LastName = "User",
                UserName = "chatowner",
                Profile = new ProfileEntity()
            },
            OwnerId = PrefixedUlid.Generate("user")
        };

        Owner = new UserEntity
        {
            Email = "owner@test.com",
            FirstName = "OwnerFirstName",
            LastName = "OwnerLastName",
            Id = PrefixedUlid.Generate("user"),
            CreatedAt = DateTime.Now,
            IsActive = true,
            UserName = "owneruser",
            Profile = new ProfileEntity()
        };
    }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public required ChatModel Chat { get; set; }

    public required UserEntity Sender { get; set; }

    public required UserEntity Owner { get; set; }
    public required string OwnerId { get; set; } 
    public required string SenderId { get; set; } 
    public required string ChatId { get; set; } 
}

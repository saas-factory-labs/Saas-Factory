using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailVerification;

public class EmailVerificationEntity : BaseEntity
{
    public EmailVerificationEntity()
    {
        Id = PrefixedUlid.Generate("email_verif");
    }

    public required string Token { get; set; }
    public DateTime ExpireAt { get; set; }
    public bool HasBeenOpened { get; set; }
    public bool HasBeenVerified { get; set; }

    public string? UserEntityId { get; set; }
    public UserEntity? User { get; set; }
}

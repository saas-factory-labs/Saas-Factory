namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.EmailVerification;

public class EmailVerificationEntity
{
    public int Id { get; set; }

    public required string Token { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public DateTime ExpireAt { get; set; }

    public bool HasBeenOpened { get; set; }
    public bool HasBeenVerified { get; set; }
}

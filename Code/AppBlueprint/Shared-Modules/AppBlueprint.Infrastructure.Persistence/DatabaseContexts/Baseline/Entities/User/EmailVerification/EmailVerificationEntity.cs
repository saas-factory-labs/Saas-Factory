namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.EmailVerification;

public sealed class EmailVerificationEntity
{
    public int Id { get; set; }

    public required string Token { get; init; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public DateTime ExpireAt { get; set; }

    public bool HasBeenOpened { get; set; }
    public bool HasBeenVerified { get; set; }
}

namespace AppBlueprint.Contracts.Baseline.Profile.Requests;

public class CreateProfileRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public required string UserName { get; set; }
    public bool IsActive { get; set; }

    public string? Language { get; set; }

    public DateTime Timezone { get; set; }

    public string? Avatar { get; set; }

    public string? Slug { get; set; }
}

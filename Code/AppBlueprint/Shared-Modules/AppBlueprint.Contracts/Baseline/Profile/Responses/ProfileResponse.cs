namespace AppBlueprint.Contracts.Baseline.Profile.Responses;

public class ProfileResponse
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? UserName { get; set; }
    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastLogin { get; set; }

    public string? Language { get; set; }

    public DateTime Timezone { get; set; }

    public string? Avatar { get; set; }

    public string? Slug { get; set; }
}

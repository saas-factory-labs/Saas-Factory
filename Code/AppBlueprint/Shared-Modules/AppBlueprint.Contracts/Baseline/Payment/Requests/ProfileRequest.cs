namespace AppBlueprint.Contracts.Baseline.Payment.Requests;

public class ProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; } // Middle name is optional but can be in LastName field as well
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Language { get; set; }
    public string? Avatar { get; set; }
    public DateTime Timezone { get; set; }
}

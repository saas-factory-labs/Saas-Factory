using Microsoft.AspNetCore.Authorization;

namespace AppBlueprint.Infrastructure.Authorization;

public class MinimumAgeRequirement(int minimumAge) : IAuthorizationRequirement
{
    public int MinimumAge { get; } = minimumAge;
}

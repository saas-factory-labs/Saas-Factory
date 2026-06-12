using Microsoft.AspNetCore.Authorization;

namespace AppBlueprint.Infrastructure.Authentication.Authorization;

public class MinimumAgeRequirement(int minimumAge) : IAuthorizationRequirement
{
    public int MinimumAge { get; } = minimumAge;
}

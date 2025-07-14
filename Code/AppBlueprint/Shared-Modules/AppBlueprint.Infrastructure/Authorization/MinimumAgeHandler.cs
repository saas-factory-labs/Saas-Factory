using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AppBlueprint.Infrastructure.Authorization;

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MinimumAgeRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);
        
        if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth)) return Task.CompletedTask;

        var birthDate = DateTime.Parse(context.User.FindFirst(ClaimTypes.DateOfBirth)!.Value, CultureInfo.InvariantCulture);
        int age = DateTime.Today.Year - birthDate.Year;

        if (age >= requirement.MinimumAge) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

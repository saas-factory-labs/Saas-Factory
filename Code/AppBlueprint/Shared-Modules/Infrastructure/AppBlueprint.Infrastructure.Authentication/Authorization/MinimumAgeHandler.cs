using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AppBlueprint.Infrastructure.Authentication.Authorization;

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MinimumAgeRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth)) return Task.CompletedTask;

        var birthDate = DateTime.Parse(context.User.FindFirst(ClaimTypes.DateOfBirth)!.Value, CultureInfo.InvariantCulture);
        int age = CalculateAge(birthDate, DateTime.Today);

        if (age >= requirement.MinimumAge) context.Succeed(requirement);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Calculates a person's age in whole years as of <paramref name="today"/>.
    /// SECURITY: subtracts a year when the birthday has not yet occurred this year, so an
    /// age gate cannot be bypassed by users who only turn the required age later in the year.
    /// </summary>
    public static int CalculateAge(DateTime birthDate, DateTime today)
    {
        int age = today.Year - birthDate.Year;
        if (birthDate.Date > today.Date.AddYears(-age)) age--;
        return age;
    }
}

using System.Security.Claims;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext? context)
    {
        if (context is null) return null;
        Claim? userId = context.User.Claims.SingleOrDefault(x => x.Type == "userid");

        if (Guid.TryParse(userId?.Value, out Guid parsedId)) return parsedId;

        return null;
    }
}

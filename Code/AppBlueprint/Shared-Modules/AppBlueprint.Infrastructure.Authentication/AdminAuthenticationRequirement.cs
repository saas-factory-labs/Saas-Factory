using System.Security.Claims;
using AppBlueprint.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AppBlueprint.Infrastructure.Authentication;

public class AdminAuthenticationRequirement(string apiKey) : IAuthorizationHandler, IAuthorizationRequirement
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.User.HasClaim("Admin", "true"))
        {
            context.Succeed(this);
            return Task.CompletedTask;
        }

        if (context.Resource is not HttpContext httpContext) return Task.CompletedTask;

        if (!httpContext.Request.Headers.TryGetValue(ApiHeaderNames.ApiKeyHeaderName, out
                StringValues extractedApiKey))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (apiKey != extractedApiKey)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var identity = (ClaimsIdentity)httpContext.User.Identity!;
        identity.AddClaim(new Claim("userid", Guid.Parse("74e20de1-8dd0-4bc2-a9f5-8aa3203ad209").ToString()));
        context.Succeed(this);
        return Task.CompletedTask;
    }
}

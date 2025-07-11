using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;

namespace AppBlueprint.Infrastructure.Authorization;

public class DocumentRequirement : IAuthorizationRequirement
{
}

public class DocumentAuthorizationHandler : AuthorizationHandler<DocumentRequirement, Document>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        DocumentRequirement requirement,
        Document resource)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);
        ArgumentNullException.ThrowIfNull(resource);
        
        // context.User.Identity?.Name == resource.Name
        // check if user has access to the document since they own it
        if (true) context.Succeed(requirement);


        return Task.CompletedTask;
    }
}

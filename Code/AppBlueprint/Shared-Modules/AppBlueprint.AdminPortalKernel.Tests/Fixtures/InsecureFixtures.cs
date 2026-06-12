using AppBlueprint.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Route = Microsoft.AspNetCore.Components.RouteAttribute;

namespace AppBlueprint.AdminPortalKernel.Tests.Fixtures;

// Deliberately insecure types used to prove the security inspector catches every
// violation class. They are never registered in a real host; the test assembly doubles
// as a hostile plugin in the fail-fast tests.

/// <summary>Controller without any [Authorize] attribute.</summary>
public sealed class UnprotectedFixtureController : ControllerBase
{
    public IActionResult Get() => Ok();
}

/// <summary>[Authorize] without the DeploymentManagerAdmin role is still a violation.</summary>
[Authorize]
public sealed class RolelessFixtureController : ControllerBase
{
    public IActionResult Get() => Ok();
}

/// <summary>Properly protected controller, but with an [AllowAnonymous] leak on an action.</summary>
[Authorize(Roles = Roles.DeploymentManagerAdmin)]
public sealed class AnonymousLeakFixtureController : ControllerBase
{
    [AllowAnonymous]
    public IActionResult Leak() => Ok();

    public IActionResult Safe() => Ok();
}

/// <summary>Fully compliant component: role-gated and routed under its module slug.</summary>
[Route("/apps/fixture-app/admin/good")]
[Authorize(Roles = Roles.DeploymentManagerAdmin)]
public sealed class GoodFixtureComponent : ComponentBase;

/// <summary>Routable component without authorization.</summary>
[Route("/apps/fixture-app/admin/open")]
public sealed class UnprotectedFixtureComponent : ComponentBase;

/// <summary>Role-gated component that escapes its module's /apps/{slug}/ route prefix.</summary>
[Route("/totally/elsewhere")]
[Authorize(Roles = Roles.DeploymentManagerAdmin)]
public sealed class EscapedRouteFixtureComponent : ComponentBase;

using System.Reflection;
using AppBlueprint.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentManager.Tests;

/// <summary>
/// Security regression tests (OWASP A01 - Broken Access Control).
/// The DeploymentManager API manages deployed SaaS apps across all customers,
/// so every controller must require the DeploymentManagerAdmin role.
/// </summary>
internal sealed class ControllerAuthorizationTests
{
    private static List<Type> GetApiControllers()
    {
        Assembly apiAssembly = typeof(global::DeploymentManager.ApiService.Api.Controllers.Pulumi.ProjectController).Assembly;

        return apiAssembly.GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type)
                           && !type.IsAbstract)
            .ToList();
    }

    [Test]
    public async Task AllControllers_MustRequireDeploymentManagerAdminRole()
    {
        List<Type> controllers = GetApiControllers();

        await Assert.That(controllers.Count).IsGreaterThan(0);

        List<string> unprotected = new();

        foreach (Type controller in controllers)
        {
            AuthorizeAttribute? authorize = controller.GetCustomAttributes<AuthorizeAttribute>(inherit: true)
                .FirstOrDefault();

            if (authorize is null || authorize.Roles is null
                || !authorize.Roles.Contains(Roles.DeploymentManagerAdmin, StringComparison.Ordinal))
            {
                unprotected.Add(controller.FullName ?? controller.Name);
            }
        }

        string unprotectedControllers = string.Join(", ", unprotected);
        await Assert.That(unprotectedControllers).IsEmpty();
    }

    [Test]
    public async Task NoControllerAction_MayAllowAnonymousAccess()
    {
        List<string> anonymousActions = new();

        foreach (Type controller in GetApiControllers())
        {
            if (controller.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).Any())
            {
                anonymousActions.Add(controller.FullName ?? controller.Name);
            }

            foreach (MethodInfo action in controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (action.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).Any())
                {
                    anonymousActions.Add($"{controller.FullName}.{action.Name}");
                }
            }
        }

        string anonymousEndpoints = string.Join(", ", anonymousActions);
        await Assert.That(anonymousEndpoints).IsEmpty();
    }
}

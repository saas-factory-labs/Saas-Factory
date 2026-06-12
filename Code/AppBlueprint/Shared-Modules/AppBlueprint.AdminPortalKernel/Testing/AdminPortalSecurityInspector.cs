using System.Reflection;
using AppBlueprint.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ComponentRouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace AppBlueprint.AdminPortalKernel.Testing;

/// <summary>
/// Reflection-based security inspection of admin portal assemblies (OWASP A01 - Broken
/// Access Control), generalizing DeploymentManager's ControllerAuthorizationTests:
/// every controller and routable component must require the DeploymentManagerAdmin role,
/// nothing may be [AllowAnonymous], and module routes must stay under /apps/{slug}/.
/// Framework-agnostic (returns violation strings) so it can run inside any test framework
/// in this repo or in external app repos - and at host startup, where the plugin loader
/// refuses to boot with a violating plugin.
/// </summary>
public static class AdminPortalSecurityInspector
{
    /// <summary>Controllers lacking [Authorize(Roles = DeploymentManagerAdmin)].</summary>
    public static IReadOnlyList<string> FindUnprotectedControllers(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        List<string> violations = new();

        foreach (Type controller in GetLoadableTypes(assembly)
                     .Where(type => typeof(ControllerBase).IsAssignableFrom(type)
                                    && type is { IsClass: true, IsAbstract: false }))
        {
            AuthorizeAttribute? authorize = controller.GetCustomAttributes<AuthorizeAttribute>(inherit: true)
                .FirstOrDefault(attribute => attribute.Roles is not null
                                             && attribute.Roles.Contains(Roles.DeploymentManagerAdmin, StringComparison.Ordinal));

            if (authorize is null)
            {
                violations.Add(
                    $"Controller '{controller.FullName}' must be protected with [Authorize(Roles = Roles.DeploymentManagerAdmin)].");
            }
        }

        return violations;
    }

    /// <summary>[AllowAnonymous] anywhere (controller types, controller actions, routable components).</summary>
    public static IReadOnlyList<string> FindAllowAnonymousUsages(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        List<string> violations = new();

        foreach (Type type in GetLoadableTypes(assembly).Where(type => type is { IsClass: true, IsAbstract: false }))
        {
            bool isController = typeof(ControllerBase).IsAssignableFrom(type);
            bool isRoutableComponent = type.GetCustomAttributes<ComponentRouteAttribute>(inherit: false).Any();

            if (!isController && !isRoutableComponent)
            {
                continue;
            }

            if (type.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).Any())
            {
                violations.Add($"'{type.FullName}' must not be [AllowAnonymous].");
            }

            if (!isController)
            {
                continue;
            }

            foreach (MethodInfo action in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (action.GetCustomAttributes<AllowAnonymousAttribute>(inherit: true).Any())
                {
                    violations.Add($"Action '{type.FullName}.{action.Name}' must not be [AllowAnonymous].");
                }
            }
        }

        return violations;
    }

    /// <summary>Routable components (RouteAttribute) lacking [Authorize(Roles = DeploymentManagerAdmin)].</summary>
    public static IReadOnlyList<string> FindUnprotectedRoutableComponents(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        List<string> violations = new();

        foreach (Type component in GetLoadableTypes(assembly)
                     .Where(type => type is { IsClass: true, IsAbstract: false }
                                    && type.GetCustomAttributes<ComponentRouteAttribute>(inherit: false).Any()))
        {
            bool isProtected = component.GetCustomAttributes<AuthorizeAttribute>(inherit: true)
                .Any(attribute => attribute.Roles is not null
                                  && attribute.Roles.Contains(Roles.DeploymentManagerAdmin, StringComparison.Ordinal));

            if (!isProtected)
            {
                violations.Add(
                    $"Routable component '{component.FullName}' must be protected with " +
                    "[Authorize(Roles = Roles.DeploymentManagerAdmin)] (add it to the module's Components/Pages/_Imports.razor).");
            }
        }

        return violations;
    }

    /// <summary>
    /// Routes that escape the module's namespace: every route template in a module
    /// assembly must start with /apps/{slug}/ for one of the assembly's module slugs.
    /// </summary>
    public static IReadOnlyList<string> FindRoutesOutsideSlugPrefixes(Assembly assembly, IReadOnlyCollection<string> slugs)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(slugs);

        List<string> allowedPrefixes = slugs.Select(slug => $"/apps/{slug}/").ToList();
        List<string> allowedExactRoutes = slugs.Select(slug => $"/apps/{slug}/admin").ToList();

        List<string> violations = new();

        foreach (Type component in GetLoadableTypes(assembly)
                     .Where(type => type is { IsClass: true, IsAbstract: false }))
        {
            foreach (ComponentRouteAttribute route in component.GetCustomAttributes<ComponentRouteAttribute>(inherit: false))
            {
                bool allowed = allowedPrefixes.Any(prefix => route.Template.StartsWith(prefix, StringComparison.Ordinal))
                               || allowedExactRoutes.Contains(route.Template, StringComparer.Ordinal);

                if (!allowed)
                {
                    violations.Add(
                        $"Route '{route.Template}' on '{component.FullName}' is outside the module's " +
                        $"allowed prefix(es): {string.Join(", ", allowedPrefixes)}.");
                }
            }
        }

        return violations;
    }

    /// <summary>
    /// Full inspection of a plugin assembly hosting the given module slugs.
    /// Used by the plugin loader at startup (fail fast) and by host regression tests.
    /// </summary>
    public static IReadOnlyList<string> InspectModuleAssembly(Assembly assembly, IReadOnlyCollection<string> slugs)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(slugs);

        return
        [
            .. FindUnprotectedControllers(assembly),
            .. FindAllowAnonymousUsages(assembly),
            .. FindUnprotectedRoutableComponents(assembly),
            .. FindRoutesOutsideSlugPrefixes(assembly, slugs)
        ];
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(type => type is not null)!;
        }
    }
}

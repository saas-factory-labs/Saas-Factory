using System.Reflection;
using AppBlueprint.UiKit.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace AppBlueprint.UiKit.Services;

public class NavigationService(IConfiguration configuration)
{
    public static List<NavLinkMetadata> GetNavLinks()
    {
        var links = new List<NavLinkMetadata>();

        // 1. Load Pages via Reflection (Pages already in the UiKit Library)
        var assembly = Assembly.GetExecutingAssembly(); // Blazor Web App Assembly
        IEnumerable<Type> pageTypes = assembly.ExportedTypes
            .Where(type => type.IsSubclassOf(typeof(ComponentBase)) &&
                           type.GetCustomAttributes(typeof(RouteAttribute), false).Length > 0);

        foreach (Type pageType in pageTypes)
        {
            if (pageType is null)
                continue;

            IEnumerable<RouteAttribute> routeAttributes = pageType
                .GetCustomAttributes(typeof(RouteAttribute), false)
                .Cast<RouteAttribute>();

            RouteAttribute? routeAttribute = routeAttributes.FirstOrDefault();
            if (routeAttribute is not null)
                links.Add(new NavLinkMetadata
                {
                    Name = pageType.Name,
                    Href = routeAttribute.Template,
                    Icon = "home"
                });
        }

        return links;
    }
}


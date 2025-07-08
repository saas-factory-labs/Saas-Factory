using System.Reflection;
using AppBlueprint.UiKit.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace AppBlueprint.UiKit.Services;

public class NavigationService
{
    private readonly IConfiguration _configuration;

    public NavigationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<NavLinkMetadata> GetNavLinks()
    {
        var links = new List<NavLinkMetadata>();

        // 1. Load Pages via Reflection (Pages already in the UiKit Library)
        var assembly = Assembly.GetExecutingAssembly(); // Blazor Web App Assembly
        IEnumerable<Type>? pageTypes = assembly.ExportedTypes
            .Where(type => type.IsSubclassOf(typeof(ComponentBase)) &&
                           type.GetCustomAttributes(typeof(RouteAttribute), false).Any());

        foreach (Type? pageType in pageTypes)
        {
            IEnumerable<RouteAttribute>? routeAttributes = pageType
                .GetCustomAttributes(typeof(RouteAttribute), false)
                .Cast<RouteAttribute>();

            RouteAttribute? routeAttribute = routeAttributes.FirstOrDefault();
            if (routeAttribute is not null)
                links.Add(new NavLinkMetadata
                {
                    Name = pageType.Name,
                    Href = routeAttribute.Template,
                    MudblazorIconPath = "@Icons.Material.Filled.Home"
                });
        }

        return links;
    }
}


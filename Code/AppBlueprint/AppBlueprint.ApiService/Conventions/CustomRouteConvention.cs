using System.Globalization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AppBlueprint.ApiService.Conventions;

internal class CustomRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        CultureInfo culture = CultureInfo.InvariantCulture;

        foreach (ActionModel action in controller.Actions)
        foreach (SelectorModel selector in action.Selectors)
        {
            if (selector.AttributeRouteModel is null) selector.AttributeRouteModel = new AttributeRouteModel();

            // Get the controller and action names
            string controllerName = controller.ControllerName.ToLower(culture);
            string actionName = action.ActionName.ToLower(culture);

            string guid = Guid.NewGuid().ToString();

            // Define the route template
            string routeTemplate = $"api/{controllerName}/{actionName}/{guid}";

            selector.AttributeRouteModel.Template = routeTemplate;
        }
    }
}

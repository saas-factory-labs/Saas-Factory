using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppBlueprint.Presentation.ApiModule.OpenApi;

public class OpenApiVersionFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(swaggerDoc);

        // Set OpenAPI version explicitly at the root level using compatible version
        if (!swaggerDoc.Extensions.ContainsKey("openapi"))
            swaggerDoc.Extensions.Add("openapi", new OpenApiString("3.0.0"));

        // Ensure API version is set
        swaggerDoc.Info.Version = "1.0";

        // Add server URL if not present, without /api prefix
        if (swaggerDoc.Servers is null || swaggerDoc.Servers.Count == 0)
            swaggerDoc.Servers = new List<OpenApiServer>
            {
                new() { Url = "/" }
            };
    }
}

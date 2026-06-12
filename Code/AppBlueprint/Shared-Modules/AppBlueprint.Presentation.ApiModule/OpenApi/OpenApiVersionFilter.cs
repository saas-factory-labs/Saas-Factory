using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppBlueprint.Presentation.ApiModule.OpenApi;

public class OpenApiVersionFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(swaggerDoc);

        // Set OpenAPI version explicitly at the root level using compatible version
        if (swaggerDoc.Extensions is null)
            swaggerDoc.Extensions = new Dictionary<string, IOpenApiExtension>();

        if (!swaggerDoc.Extensions.ContainsKey("openapi"))
            swaggerDoc.Extensions.Add("openapi", new JsonNodeExtension("3.0.0"));

        // Ensure API version is set
        if (swaggerDoc.Info is not null)
            swaggerDoc.Info.Version = "1.0";

        // Add server URL if not present, without /api prefix
        if (swaggerDoc.Servers is null || swaggerDoc.Servers.Count == 0)
            swaggerDoc.Servers =
            [
                new OpenApiServer { Url = "/" }
            ];
    }
}

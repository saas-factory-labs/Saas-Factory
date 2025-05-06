using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppBlueprint.Presentation.ApiModule.OpenApi;

public class SchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        // Ensure all schemas are compatible with OpenAPI 3.0
        if (schema.Type == "object" && schema.AdditionalProperties == null)
            schema.AdditionalProperties = new OpenApiSchema { Type = "object" };
    }
}

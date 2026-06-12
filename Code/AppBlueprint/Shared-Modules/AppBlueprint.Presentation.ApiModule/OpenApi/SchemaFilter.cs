using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppBlueprint.Presentation.ApiModule.OpenApi;

public class SchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        // Ensure all schemas are compatible with OpenAPI 3.0
        if (schema is OpenApiSchema concreteSchema &&
            concreteSchema.Type == JsonSchemaType.Object &&
            concreteSchema.AdditionalProperties is null)
        {
            concreteSchema.AdditionalProperties = new OpenApiSchema { Type = JsonSchemaType.Object };
        }
    }
}

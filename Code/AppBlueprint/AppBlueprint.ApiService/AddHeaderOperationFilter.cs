using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppBlueprint.ApiService;

internal class AddHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (operation.Parameters is null)
            operation.Parameters = [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Description = "Access token (Bearer <token>)",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String
            }
        });
    }
}

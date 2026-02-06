using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppBlueprint.ApiService;

internal class AddHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Description = "Access token (Bearer <token>)",
            Required = false,
            Schema = new OpenApiSchema
            {
                // Type = "string"
                Type = "bearer"
            }
        });
    }
}

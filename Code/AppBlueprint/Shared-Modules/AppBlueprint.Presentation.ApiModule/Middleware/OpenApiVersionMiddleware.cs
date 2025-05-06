using System.Text;

namespace AppBlueprint.Presentation.ApiModule.Middleware;

public class OpenApiVersionMiddleware
{
    private readonly RequestDelegate _next;

    public OpenApiVersionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            Stream originalBodyStream = context.Response.Body;
            using MemoryStream memoryStream = new();
            context.Response.Body = memoryStream;

            await _next(context);

            memoryStream.Position = 0;
            string responseBody;
            using (StreamReader reader = new(memoryStream))
            {
                responseBody = await reader.ReadToEndAsync();
            }

            // Check if this is a Swagger JSON response
            if (context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
                // Add OpenAPI version if not present
                if (!responseBody.Contains("\"openapi\"", StringComparison.OrdinalIgnoreCase))
                    responseBody = responseBody.Insert(1, "\"openapi\":\"3.0.0\",");

            byte[] modifiedBody = Encoding.UTF8.GetBytes(responseBody);
            context.Response.ContentLength = modifiedBody.Length;

            memoryStream.Position = 0;
            memoryStream.SetLength(0);
            await memoryStream.WriteAsync(modifiedBody);

            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
        else
        {
            await _next(context);
        }
    }
}

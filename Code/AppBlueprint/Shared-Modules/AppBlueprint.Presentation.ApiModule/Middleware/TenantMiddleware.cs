namespace AppBlueprint.Presentation.ApiModule.Middleware;

public class TenantMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext? context)
    {
        if (context is not null)
        {
            string? tenantId = context.Request.Headers["tenant-id"].FirstOrDefault();
            if (string.IsNullOrEmpty(tenantId))
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Tenant ID is required.");
                return;
            }

            context.Items["TenantId"] = tenantId;
        }

        await next(context);
    }
}

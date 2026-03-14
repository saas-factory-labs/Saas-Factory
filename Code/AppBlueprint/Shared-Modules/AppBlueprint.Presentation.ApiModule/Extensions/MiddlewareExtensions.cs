namespace AppBlueprint.Presentation.ApiModule.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseStaticFiles();

        // Note: OpenAPI/Swagger middleware is configured in Program.cs using NSwag
        // app.UseOpenApi() and app.UseSwaggerUi() should be called in Program.cs instead

        // Remove real headers and replace with deceptive ones to mislead attackers
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Remove("X-AspNet-Version");

            // Deception headers - mislead attackers with false server technology information
            context.Response.Headers.Append("Server", "Apache/2.4.62 (Ubuntu)");
            context.Response.Headers.Append("X-Powered-By", "PHP/8.2.28");

            await next();
        });

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}

// public static class MiddlewareExtensions
// {
//     public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
//     {
//         app.UseStaticFiles();
//
//         app.UseSwagger(options => {
//             options.RouteTemplate = "swagger/{documentName}/swagger.json";
//         });
//
//         app.UseSwaggerUI(options =>
//         {
//             options.SwaggerEndpoint("/api/swagger/v3.1/swagger.json", "API v3.1");
//             options.RoutePrefix = "swagger"; // Makes it available at `/api/swagger`
//             options.DocumentTitle = "AppBlueprint API - OpenAPI 3.1";
//             options.DisplayRequestDuration();
//         });
//
//         app.UseAuthentication();
//         app.UseAuthorization();
//
//         app.Use(async (context, next) =>
//         {
//             context.Response.Headers.Add("Content-Security-Policy",
//                 "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval' https://trusted.cdn.com; " +
//                 "style-src 'self' 'unsafe-inline' https://trusted.cdn.com; " +
//                 "img-src 'self' data: https://trusted.cdn.com; " +
//                 "font-src 'self' https://trusted.cdn.com; " +
//                 "connect-src 'self' https://api.trusted.com; " +
//                 "frame-src 'self' https://trusted.cdn.com");
//             await next();
//         });
//
//         return app;
//     }
// }

// public static class MiddlewareExtensions
// {
//     public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
//     {
//         app.UseStaticFiles();
//         
//         app.UseSwagger(options => {
//             options.RouteTemplate = "swagger/{documentName}/swagger.json";
//             // Ensure consistent OpenAPI spec version
//             options.SerializeAsV2 = false;
//         });
//
//         app.UseAuthentication();
//         app.UseAuthorization();
//
//         app.Use(async (context, next) =>
//         {
//             context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval' https://trusted.cdn.com; style-src 'self' 'unsafe-inline' https://trusted.cdn.com; img-src 'self' data: https://trusted.cdn.com; font-src 'self' https://trusted.cdn.com; connect-src 'self' https://api.trusted.com; frame-src 'self' https://trusted.cdn.com");
//             await next();
//         });
//
//         return app;
//     }
// }

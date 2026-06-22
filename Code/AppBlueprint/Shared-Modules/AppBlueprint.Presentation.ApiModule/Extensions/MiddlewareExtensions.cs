namespace AppBlueprint.Presentation.ApiModule.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseStaticFiles();

        // Note: OpenAPI/Swagger middleware is configured in Program.cs using NSwag
        // app.UseOpenApi() and app.UseSwaggerUi() should be called in Program.cs instead

        IWebHostEnvironment environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

        // Strip technology-disclosure headers as late as possible so downstream middleware
        // can't reintroduce them before the response is flushed.
        app.Use(async (context, next) =>
        {
            context.Response.OnStarting(static state =>
            {
                HttpResponse response = (HttpResponse)state;
                response.Headers.Remove("Server");
                response.Headers.Remove("X-Powered-By");
                response.Headers.Remove("X-AspNet-Version");

                return Task.CompletedTask;
            }, context.Response);

            // Prevent MIME-sniffing attacks
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            // Prevent clickjacking attacks by disallowing embedding in iframes
            context.Response.Headers["X-Frame-Options"] = "DENY";

            // Enable XSS protection in older browsers
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

            // Control referrer information sent with requests
            context.Response.Headers["Referrer-Policy"] = "no-referrer";

            // Permissions Policy - control browser features
            context.Response.Headers["Permissions-Policy"] =
                "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";

            // Restrict cross-domain policy files (Flash, PDF, etc.)
            context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";

            if (!environment.IsDevelopment())
            {
                // HSTS - only in production (Railway terminates SSL at the proxy level, browser enforces HTTPS)
                context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";

                // CSP - strict policy for production API (no browser resources served; Swagger is dev-only)
                context.Response.Headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";
            }

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

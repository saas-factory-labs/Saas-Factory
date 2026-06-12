using AppBlueprint.Presentation.ApiModule.Extensions;
using AppBlueprint.ServiceDefaults;
using DeploymentManager.ApiService;
using DeploymentManager.ApiService.Application.Services;
using DeploymentManager.ApiService.Domain.Interfaces;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.UnitOfWork;
using DeploymentManager.ApiService.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using static DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context.PostgresConnectionStringHelper;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddControllers()
    .ConfigureApplicationPartManager(partManager =>
    {
        // AppBlueprint.Presentation.ApiModule is referenced only for its JWT authentication
        // extensions. Remove it from controller discovery so AppBlueprint's own controllers
        // (users, tenants, files, ...) are not exposed by the DeploymentManager API.
        ApplicationPart? appBlueprintPart = partManager.ApplicationParts
            .FirstOrDefault(part => part.Name == "AppBlueprint.Presentation.ApiModule");
        if (appBlueprintPart is not null)
        {
            partManager.ApplicationParts.Remove(appBlueprintPart);
        }
    });

// SECURITY (OWASP A01): the DeploymentManager API manages deployed SaaS apps across all
// customers. Reuse AppBlueprint's JWT bearer authentication (Logto); every controller is
// additionally gated to the DeploymentManagerAdmin role via [Authorize] attributes.
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);

string connectionString = Normalize(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["DATABASE_URL"]
    ?? throw new InvalidOperationException("No database connection string configured."));

builder.Services.AddDbContext<DeploymentManagerDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IInfrastructureCodeProvider, NullInfrastructureCodeProvider>();

builder.Services.AddHttpClient<IEmailService, MailGunEmailService>();

WebApplication? app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    DeploymentManagerDbContext db = scope.ServiceProvider.GetRequiredService<DeploymentManagerDbContext>();
    await db.Database.MigrateAsync();
}

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

// Health checks remain anonymous (mapped by MapDefaultEndpoints); all controllers
// require the DeploymentManagerAdmin role via their [Authorize] attributes.
app.MapControllers();

app.MapDefaultEndpoints();

string[]? summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    WeatherForecast[]? forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
        .ToArray();
    return forecast;
});

await app.RunAsync();

namespace DeploymentManager.ApiService
{
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}

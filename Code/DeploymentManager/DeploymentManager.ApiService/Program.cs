using AppBlueprint.ServiceDefaults;
using DeploymentManager.ApiService;
using DeploymentManager.ApiService.Application.Services;
using DeploymentManager.ApiService.Domain.Interfaces;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.UnitOfWork;
using DeploymentManager.ApiService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using static DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context.PostgresConnectionStringHelper;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddControllers();

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

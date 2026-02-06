using DeploymentManager.ApiService;
using DeploymentManager.ApiService.Domain.Interfaces;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.Repositories;
using DeploymentManager.ApiService.Infrastructure.Services;

// using Infrastructure.Persistence.Data.Context;


WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
// builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

// builder.Services.AddDbContext<DeploymentManagerContext>(options =>
// {
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
// });

// builder.Services.AddScoped<IUnitOfWork, Infrastructure.Persistence.Data.UnitOfWork>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// builder.Services.AddScoped<PulumiAzureService, PulumiAzureService>();

// builder.Services.AddScoped<IInfrastructureCodeProvider, PulumiAutomationApiService>();

builder.Services.AddScoped<IEmailService, MailGunEmailService>();

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

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

// app.MapDefaultEndpoints();

app.Run();

namespace DeploymentManager.ApiService
{
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}

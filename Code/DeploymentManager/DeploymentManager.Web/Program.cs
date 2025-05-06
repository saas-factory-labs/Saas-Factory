using DeploymentManager.Web.Components;
using MudBlazor.Services;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
// builder.AddServiceDefaults();
// builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

WebApplication? app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// app.MapDefaultEndpoints();

app.Run();

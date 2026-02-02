using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

// Using Railway cloud database - connection string from environment variable
var railwayConnectionString = Environment.GetEnvironmentVariable("APPBLUEPRINT_RAILWAY_CONNECTIONSTRING") 
    ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        "Railway database connection string not found. Set APPBLUEPRINT_RAILWAY_CONNECTIONSTRING or DATABASE_CONNECTION_STRING environment variable.");

builder.AddProject<Projects.AppBlueprint_AppGateway>("appgw")
    .WithHttpEndpoint(port: 9000, name: "gateway", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:9000");
    
var apiService = builder.AddProject<Projects.AppBlueprint_ApiService>("apiservice")
    .WithHttpEndpoint(port: 9100, name: "api", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:9100")
    .WithEnvironment("SwaggerPath", "/swagger")
    .WithEnvironment("DATABASE_CONNECTION_STRING", railwayConnectionString)
    .WithEnvironment("Authentication__Provider", "Logto")
    .WithEnvironment("Authentication__Logto__Endpoint", "https://32nkyp.logto.app")
    .WithEnvironment("Authentication__Logto__ClientId", "uovd1gg5ef7i1c4w46mt6")
    .WithEnvironment("Authentication__Logto__ApiResource", "https://api.appblueprint.local"); // Validate JWT audience

builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
    .WithHttpEndpoint(port: 9200, name: "web-http", isProxied: false)
    .WithReference(apiService)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:9200")
    .WithEnvironment("API_BASE_URL", "http://localhost:9100") // Explicit API URL to avoid service discovery issues
    .WithEnvironment("DATABASE_CONNECTION_STRING", railwayConnectionString)
    .WithEnvironment("Logto__Endpoint", "https://32nkyp.logto.app/oidc")
    .WithEnvironment("Logto__AppId", "uovd1gg5ef7i1c4w46mt6")
    .WithEnvironment("Logto__AppSecret", Environment.GetEnvironmentVariable("LOGTO__APPSECRET") ?? "")
    .WithEnvironment("Logto__Resource", "https://api.appblueprint.local"); // Request JWT access token for API

string[] keys = new[]
{
    "ASPIRE_ALLOW_UNSECURED_TRANSPORT",
    "ASPIRE_DASHBOARD_PORT",
    "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL",
    "OTEL_EXPORTER_OTLP_ENDPOINT",
    "OTEL_EXPORTER_OTLP_HEADERS",
    "OTEL_EXPORTER_OTLP_PROTOCOL"
};

foreach (var key in keys)
{
    Console.WriteLine($"{key} = {Environment.GetEnvironmentVariable(key)}");
}

builder.Build().Run();
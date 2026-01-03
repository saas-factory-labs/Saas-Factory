using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

// Using Railway cloud database - connection string from environment variable
var railwayConnectionString = Environment.GetEnvironmentVariable("APPBLUEPRINT_RAILWAY_CONNECTIONSTRING") 
    ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        "Railway database connection string not found. Set APPBLUEPRINT_RAILWAY_CONNECTIONSTRING or DATABASE_CONNECTION_STRING environment variable.");

builder.AddProject<Projects.AppBlueprint_AppGateway>("appgw")
    .WithHttpEndpoint(port: 8087, name: "gateway");
    
var apiService = builder.AddProject<Projects.AppBlueprint_ApiService>("apiservice")
    .WithHttpEndpoint(port: 8091, name: "api")
    .WithEnvironment("SwaggerPath", "/swagger")
    .WithEnvironment("DATABASE_CONNECTION_STRING", railwayConnectionString);

builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpEndpoint(port: 5000, name: "web-http")
    .WithReference(apiService)
    .WithEnvironment("DATABASE_CONNECTION_STRING", railwayConnectionString)
    .WithEnvironment("Logto__Endpoint", "https://32nkyp.logto.app/oidc")
    .WithEnvironment("Logto__AppId", "uovd1gg5ef7i1c4w46mt6")
    .WithEnvironment("Logto__AppSecret", Environment.GetEnvironmentVariable("LOGTO_APP_SECRET") ?? "");

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
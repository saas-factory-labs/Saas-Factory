using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

var postgresPasswordValue = Environment.GetEnvironmentVariable("POSTGRES_DEV_PASSWORD") ?? "password";
var postgresPassword = builder.AddParameter("postgres-password", postgresPasswordValue, secret: true);
var postgresServer = builder.AddPostgres("postgres-server", password: postgresPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("appblueprint-postgres-data");
var appblueprintdb = postgresServer.AddDatabase("appblueprintdb");

builder.AddProject<Projects.AppBlueprint_AppGateway>("appgw")
    .WithHttpEndpoint(port: 8087, name: "gateway");
    
var apiService = builder.AddProject<Projects.AppBlueprint_ApiService>("apiservice")
    .WithReference(appblueprintdb)
    .WithHttpEndpoint(port: 8090, name: "api")
    .WithEnvironment("SwaggerPath", "/swagger");

builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpEndpoint(port: 8080, name: "web")
    .WithReference(apiService);

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
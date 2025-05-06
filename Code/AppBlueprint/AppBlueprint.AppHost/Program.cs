var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_DEV_PASSWORD");
var postgresServer = builder.AddPostgres("postgres-server")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", postgresPassword!)
    .WithEnvironment("POSTGRES_DB", "appblueprintdb")
    .WithDataVolume("appblueprint-postgres-data");
postgresServer.AddDatabase("appblueprintdb");

var AppGw = builder.AddProject<Projects.AppBlueprint_AppGateway>("appgw");


var apiService = builder.AddProject<Projects.AppBlueprint_ApiService>("apiservice")
.WithReference(postgresServer);

builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
    .WithExternalHttpEndpoints()
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
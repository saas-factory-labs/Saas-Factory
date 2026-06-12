# Migrating DeploymentManager to AppBlueprint Framework

## DeploymentManagerDbContext and BaselineDbContext

`DeploymentManagerDbContext : BaselineDbContext` — this is intentional (Option A design), but the context is registered with a plain `AddDbContext<DeploymentManagerDbContext>()` instead of going through any AppBlueprint infrastructure setup. The `BaselineDbContext` constructor takes `IConfiguration` and `ILogger` which are injected fine, but AppBlueprint's own row-level-security, data seeding, and health check wiring (from `AddAppBlueprintInfrastructure`) never runs.
